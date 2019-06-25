using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using SharedClasses.DataObjects.ChangeMetaData;

namespace SyncService.Modules
{
    public class FileManagerModule : IFileManagerModule
    {
        public bool IsActive { get; private set; }
        public event ConflictArisedHandler OnConflictArises;

        private int currPatchId = 0;

        private Func<IList<FileChangeMetaData>> GetSortedSavedChanges;
        private string directory;
        private string serviceId;

        private string ActualDirectory {
            get {
                return Path.Combine(directory, "currentData");
            }
        }

        private string HistoryDirectory {
            get {
                return Path.Combine(directory, ".history");
            }
        }

        public FileManagerModule(string directory, Func<IList<FileChangeMetaData>> getSortedSavedChanges)
        {
            this.directory = directory;
            GetSortedSavedChanges = getSortedSavedChanges;
            Directory.CreateDirectory(ActualDirectory);
            Directory.CreateDirectory(HistoryDirectory);
        }

        public void Activate(string serviceId)
        {
            IsActive = true;
            this.serviceId = serviceId;
        }

        public void Deactivate()
        {
            IsActive = false;
            serviceId = null;
        }

        public void ApplyChanges(IList<FileChangeMetaData> newChanges)
        {
            if (!IsActive)
                throw new InvalidOperationException("Module 'FileManagerModule' is not active");

            var savedChanges = GetSortedSavedChanges();

            foreach (var change in newChanges)
            {
                ApplyChanges(change, savedChanges);
            }
        }

        private void ApplyChanges(FileChangeMetaData change, IList<FileChangeMetaData> savedChanges)
        {
            switch (change.Type)
            {
                case FileChangeMetaData.ChangeType.NEW:
                    ApplyChangeTypeNew(change as FileChangeNewMetaData, savedChanges);
                    break;
                case FileChangeMetaData.ChangeType.UPDATE:
                    ApplyChangeTypeUpdate(change as FileChangeUpdateMetaData, savedChanges);
                    break;
                case FileChangeMetaData.ChangeType.DELETE:
                    ApplyChangeTypeDelete(change as FileChangeDeleteMetaData, savedChanges);
                    break;
            }
        }

        private void ApplyChangeTypeNew(FileChangeNewMetaData metaData, IList<FileChangeMetaData> savedChanges)
        {
            var newerChanges = GetNewerSavedChanges(savedChanges, metaData.TimeStamp);
            var content = RequestContentOfChange(metaData);

            if (FileExists(metaData.FileName))
            {
                ThrowConflict(metaData, "File already existed, when a 'NEW' file change should be applied. File will be overwritten");
            }

            if (content == null)
            {
                ThrowConflict(metaData, "Could not request File contents");
                return;
            }

            if (newerChanges.Where(ch => ch.FileName == metaData.FileName).Any())
            {
                ThrowConflict(metaData, "'NEW' file change was overwritten in the existing changes. Change not applied");
                SaveFileInHistory(metaData.DomesticServiceId, metaData.PatchId, metaData.FileName, content);
            }
            else
            {
                SaveFile(metaData, content);
            }
        }

        private void ApplyChangeTypeUpdate(FileChangeUpdateMetaData metaData, IList<FileChangeMetaData> savedChanges)
        {
            var newerChanges = GetNewerSavedChanges(savedChanges, metaData.TimeStamp);
            var content = RequestContentOfChange(metaData);

            if (!FileExists(metaData.FileName))
            {
                ThrowConflict(metaData, "File doesn't existed, when a 'UPDATE' file change should be applied. File will be created");
            }

            if (content == null)
            {
                ThrowConflict(metaData, "Could not request File contents");
                return;
            }

            if (newerChanges.Where(ch => ch.FileName == metaData.FileName).Any())
            {
                ThrowConflict(metaData, "'UPDATE' file change was overwritten in the existing changes. Change not applied");
                SaveFileInHistory(metaData.DomesticServiceId, metaData.PatchId, metaData.FileName, content);
            }
            else
            {
                SaveFile(metaData, content);
            }
        }

        private void ApplyChangeTypeDelete(FileChangeDeleteMetaData metaData, IList<FileChangeMetaData> savedChanges)
        {
            var newerChanges = GetNewerSavedChanges(savedChanges, metaData.TimeStamp);
            var content = RequestContentOfChange(metaData);

            if (!FileExists(metaData.FileName))
            {
                ThrowConflict(metaData, "File doesn't existed, when a 'DELETE' file change should be applied");
                return;
            }

            if (newerChanges.Where(ch => ch.FileName == metaData.FileName).Any())
            {
                ThrowConflict(metaData, "'DELETE' file change was overwritten in the existing changes. File not deleted");
            }
            else
            {
                DeleteFile(metaData);
            }
        }

        private void ThrowConflict(FileChangeMetaData metaData, string message)
        {
            OnConflictArises?.Invoke(this, new ConflictArisedArgs(
                    metaData,
                    message
                ));
        }

        private IEnumerable<FileChangeMetaData> GetNewerSavedChanges(IList<FileChangeMetaData> savedChanges, DateTime timeStamp)
        {
            return savedChanges.SkipWhile(ch => ch.TimeStamp < timeStamp);
        }

        private string RequestContentOfChange(FileChangeMetaData metaData)
        {
            try
            {
                return File.ReadAllText(
                    GetServiceHistory(metaData.DomesticServiceId, metaData.PatchId, metaData.FileName)
                    );
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GetServiceHistory(string serviceId, int patchId, string fileName)
        {
            return Path.Combine(directory, "..", serviceId, ".history", serviceId, patchId.ToString(), fileName);
        }

        public FileChangeMetaData AddFile(string fileName, string content)
        {
            if (!IsActive)
                throw new InvalidOperationException("Module 'FileManagerModule' is not active");

            var patchId = GetNextPatchId();
            SaveFile(serviceId, patchId, fileName, content);
            return new FileChangeNewMetaData(
                fileName,
                serviceId,
                patchId,
                DateTime.Now
                );
        }

        public FileChangeMetaData DeleteFile(string fileName)
        {
            if (!IsActive)
                throw new InvalidOperationException("Module 'FileManagerModule' is not active");

            var patchId = GetNextPatchId();
            DeleteFile(serviceId, patchId, fileName);
            return new FileChangeDeleteMetaData(
                fileName,
                serviceId,
                patchId,
                DateTime.Now
                );
        }

        public FileChangeMetaData UpdateFile(string fileName, string content)
        {
            if (!IsActive)
                throw new InvalidOperationException("Module 'FileManagerModule' is not active");

            var patchId = GetNextPatchId();
            SaveFile(serviceId, patchId, fileName, content);
            return new FileChangeUpdateMetaData(
                fileName,
                serviceId,
                patchId,
                DateTime.Now
                );
        }

        public IList<Tuple<string, string>> GetAllFiles()
        {
            if (!IsActive)
                throw new InvalidOperationException("Module 'FileManagerModule' is not active");

            var fileNames = Directory.GetFiles(ActualDirectory);

            return fileNames.Select(name => new Tuple<string, string>(name, GetFile(name))).ToList();
        }

        public string GetFile(string name)
        {
            if (!IsActive)
                throw new InvalidOperationException("Module 'FileManagerModule' is not active");

            try
            {
                return File.ReadAllText(Path.Combine(ActualDirectory, name));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetHistoryFile(string fileName, string serviceId, int patchId)
        {
            if (!IsActive)
                throw new InvalidOperationException("Module 'FileManagerModule' is not active");

            try
            {
                return File.ReadAllText(
                    Path.Combine(HistoryDirectory, serviceId, patchId.ToString(), fileName)
                    );
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool FileExists(string name)
        {
            if (!IsActive)
                throw new InvalidOperationException("Module 'FileManagerModule' is not active");

            return File.Exists(Path.Combine(ActualDirectory, name));
        }

        private int GetNextPatchId()
        {
            return Interlocked.Increment(ref currPatchId);
        }

        private void SaveFile(FileChangeMetaData metaData, string content)
            => SaveFile(metaData.DomesticServiceId, metaData.PatchId, metaData.FileName, content);

        private void SaveFile(string serviceId, int patchId, string fileName, string content)
        {
            Directory.CreateDirectory(ActualDirectory);
            File.WriteAllText(Path.Combine(ActualDirectory, fileName), content);
            SaveFileInHistory(serviceId, patchId, fileName, content);
        }

        private void SaveFileInHistory(FileChangeMetaData metaData, string content)
            => SaveFileInHistory(metaData.DomesticServiceId, metaData.PatchId, metaData.FileName, content);

        private void SaveFileInHistory(string serviceId, int patchId, string fileName, string content)
        {
            var directory = Path.Combine(HistoryDirectory, serviceId, patchId.ToString());
            Directory.CreateDirectory(directory);
            File.WriteAllText(
                Path.Combine(directory, fileName),
                content
                );
        }

        private void DeleteFile(FileChangeMetaData metaData)
            => DeleteFile(metaData.DomesticServiceId, metaData.PatchId, metaData.FileName);

        private void DeleteFile(string serviceId, int patchId, string fileName)
        {
            File.Delete(Path.Combine(ActualDirectory, fileName));
        }
    }
}
