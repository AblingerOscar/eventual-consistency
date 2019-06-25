using SharedClasses.DataObjects.ChangeMetaData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncService.Modules
{
    public delegate void ConflictArisedHandler(IFileManagerModule source, ConflictArisedArgs args);

    public interface IFileManagerModule : IModule
    {
        event ConflictArisedHandler OnConflictArises;

        void ApplyChanges(IList<FileChangeMetaData> changes);

        FileChangeMetaData AddFile(string fileName, string content);
        FileChangeMetaData DeleteFile(string fileName);
        FileChangeMetaData UpdateFile(string fileName, string content);

        IList<Tuple<string, string>> GetAllFiles();
        string GetFile(string name);
        string GetHistoryFile(string fileName, string serviceId, int patchId);
        bool FileExists(string name);
    }
}
