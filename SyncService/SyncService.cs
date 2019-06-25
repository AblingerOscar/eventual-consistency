using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SharedClasses.DataObjects;
using SharedClasses.DataObjects.ChangeMetaData;
using SyncService.Modules;
using SyncService.Modules.Heartbeat;

namespace SyncService
{
    public class SyncService : ICheetahSyncService
    {
        public event OnLogHandler OnLog;

        public bool IsRunning { get; private set; }
        public string SavePath { get; private set; }
        public string UID { get; private set; }
        public string SyncPath { get; private set; }
        public DateTime LastDomesticChangeTime { get; private set; }
        public IDictionary<string, DateTime> LastKnownChangeTime { get; private set; }

        private SyncData data;
        private IHeartbeatModule heartbeatModule;
        private IMetaDataShareModule metaDataModule;
        private IUpdaterModule updaterModule;
        private IFileManagerModule fileManagerModule;

        #region Setup
        public SyncService(string uid, string syncPath, string savePath)
        {
            UID = uid;
            SyncPath = syncPath;
            SavePath = savePath;
            SetUpHeartbeatModule();
            SetUpMetaDataShareModule();
            SetUpUpdaterModule();
            SetUpFileManagerModule();
        }

        private void SetUpHeartbeatModule()
        {
            var heartbeatModule = new HeartbeatModule(GetKnownChanges);
            this.heartbeatModule = heartbeatModule;

            heartbeatModule.OnHeartbeatAnswerReceived += (sender, args) =>
            {
                OnLog?.Invoke(this, new OnLogHandlerArgs(
                    "Heartbeat answer received from " + args.HeartbeatReq.SenderUID + " with " + args.HeartbeatReq.KnownChanges.Count + " known changes",
                    LogReason.RABBITMQ_COMMUNICATION
                    ));
                metaDataModule.SendMetaDataShareRequest(args.HeartbeatReq.SenderUID, CreateMetaDataRequest());
            };

            heartbeatModule.OnOutdatedLocalChanges += (sender, args) =>
            {
                OnLog?.Invoke(this, new OnLogHandlerArgs(
                    $"Found {args.OutdatedLocalChanges.Count} outdated local changes from: " +
                        string.Join(", ", args.OutdatedLocalChanges),
                    LogReason.RABBITMQ_COMMUNICATION));
                updaterModule.NotifyAboutOutdatedAlienChanges(args.OutdatedLocalChanges);
            };
        }

        private MetaDataRequest CreateMetaDataRequest()
        {
            return new MetaDataRequest()
            {
                ServiceId = UID,
                DomesticChangesSince = LastDomesticChangeTime,
                AlienChangesSince = LastKnownChangeTime
            };
        }

        private void SetUpMetaDataShareModule()
        {
            metaDataModule = new MetaDataShareModule(GetDomesticChangesSince, GetAlienChangesSince);
            metaDataModule.OnMetaDataExchangeReceived += OnMetaDataExchangeReceived;
        }

        private void OnMetaDataExchangeReceived(IMetaDataShareModule sender, MetaDataAnswerReceivedArgs args)
        {
            var changes = new List<ChangeSet>(args.MetaDataAnswer.AlienChanges)
                {
                    args.MetaDataAnswer.DomesticChanges
                };

            foreach (var changeSet in changes)
            {
                var changeData = changeSet.Changes.OrderBy(c => c.TimeStamp).ToList();

                lock (data)
                {
                    fileManagerModule.ApplyChanges(changeData);
                    data.AddSortedAlienChanges(changeSet.ServiceUID, changeData);
                }
            }
        }

        private void SetUpUpdaterModule()
        {
            updaterModule = new UpdaterModule();
            updaterModule.OnUpdateAlienChanges += (sender, outDatedList) =>
            {
                heartbeatModule.SendHeartbeat();
            };
        }

        private void SetUpFileManagerModule()
        {
            fileManagerModule = new FileManagerModule(SyncPath, GetSortedSavedChanges);

            fileManagerModule.OnConflictArises += (source, args) =>
            {
                OnLog?.Invoke(this, new OnLogHandlerArgs(
                    $"({args.ContextMetaData.DomesticServiceId}:{args.ContextMetaData.FileName}) " + args.Message,
                    LogReason.FILE_CONFLICT));
            };
        }

        private IList<FileChangeMetaData> GetSortedSavedChanges()
        {
            List<FileChangeMetaData> changes;
            lock (data)
            {
                changes = new List<FileChangeMetaData>(data.DomesticChanges);

                foreach (var alienChangesKVP in data.AlienChanges)
                {
                    changes.AddRange(alienChangesKVP.Value);
                }
            }

            SyncData.Sort(changes);
            return changes;
        }
        #endregion

        #region ISyncService methods implementation
        public void Abort()
        {
            IsRunning = false;
            DeactivateModules();
            OnLog?.Invoke(this, new OnLogHandlerArgs("Service Aborted", LogReason.STATUSCHANGE));
        }

        public void ShutDown()
        {
            IsRunning = false;
            SaveData();
            DeactivateModules();
            OnLog?.Invoke(this, new OnLogHandlerArgs("Service Shut down", LogReason.STATUSCHANGE));
        }

        public void StartUp()
        {
            IsRunning = true;
            LoadSavedData();
            ActivateModules();
            heartbeatModule.SendHeartbeat();
        }
        #endregion

        #region Private Helper methods
        private IDictionary<string, DateTime> GetKnownChanges()
        {
            return new Dictionary<string, DateTime>(LastKnownChangeTime)
            {
                { UID, LastDomesticChangeTime }
            };
        }

        private void LoadSavedData()
        {
            lock (data)
            {
                try
                {
                    string json = File.ReadAllText(SavePath);
                    data = JsonConvert.DeserializeObject<SyncData>(json);
                    UpdateLastChangesFromData();
                }
                catch (Exception)
                {
                    OnLog?.Invoke(this, new OnLogHandlerArgs("Loading default data as save file could not be read", LogReason.USER_INFO));
                    data = new SyncData();
                    LastDomesticChangeTime = DateTime.Now;
                    LastKnownChangeTime = new Dictionary<string, DateTime>();
                }
            }
        }

        private void UpdateLastChangesFromData()
        {
            lock (data)
            {
                LastDomesticChangeTime = data.DomesticChanges[data.DomesticChanges.Count - 1].TimeStamp;

                foreach (var alienChanges in data.AlienChanges)
                {
                    LastKnownChangeTime[alienChanges.Key] = alienChanges.Value[alienChanges.Value.Count - 1].TimeStamp;
                }
            }
        }

        private void SaveData()
        {
            try
            {
                if (data != null)
                {
                    lock (data)
                    {
                        if (data != null)
                            File.WriteAllText(SavePath, JsonConvert.SerializeObject(data));
                    }
                }
            }
            catch (Exception)
            {
                OnLog?.Invoke(this, new OnLogHandlerArgs("Could not save data", LogReason.DEBUG, OutputLevel.ERROR));
            }
        }

        private void ActivateModules()
        {
            heartbeatModule.Activate(UID);
            metaDataModule.Activate(UID);
            updaterModule.Activate(UID);
        }

        private void DeactivateModules()
        {
            heartbeatModule.Deactivate();
            metaDataModule.Deactivate();
            updaterModule.Deactivate();
        }

        private ChangeSet GetDomesticChangesSince(DateTime timestamp)
        {
            lock (data)
            {
                var changesSince = data.DomesticChanges.SkipWhile(ch => ch.TimeStamp < timestamp).ToList();
            }

            return new ChangeSet()
            {
                Timestamp = DateTime.Now,
                ServiceUID = UID,
                Changes = changesSince
            };
        }

        private IList<ChangeSet> GetAlienChangesSince(IDictionary<string, DateTime> timestampDictionary)
        {
            var changeSetList = new List<ChangeSet>();

            foreach (var timestampKVP in timestampDictionary)
            {
                changeSetList.Add(
                    GetAlienChangeSetSince(timestampKVP.Key, timestampKVP.Value)
                    );
            }

            return changeSetList;
        }

        private ChangeSet GetAlienChangeSetSince(string serviceId, DateTime timestamp)
        {
            lock (data)
            {
                var changes = data.AlienChanges[serviceId].SkipWhile(ch => ch.TimeStamp < timestamp).ToList();
            }

            return new ChangeSet()
            {
                Timestamp = changes.Last().TimeStamp,
                ServiceUID = serviceId,
                Changes = changes
            };
        }
        #endregion

        #region ICheetahSyncService methods implemantation
        public void AddFile(string fileName, string content)
        {
            if (!IsRunning)
                throw new InvalidOperationException("Service is not running");

            lock(data)
            {
                var change = fileManagerModule.AddFile(fileName, content);
                data.AddSortedDomesticChanges(new List<FileChangeMetaData>() { change });
            }
        }

        public void DeleteFile(string fileName)
        {
            if (!IsRunning)
                throw new InvalidOperationException("Service is not running");

            lock(data)
            {
                var change = fileManagerModule.DeleteFile(fileName);
                data.AddSortedDomesticChanges(new List<FileChangeMetaData>() { change });
            }
        }

        public void UpdateFile(string fileName, string content)
        {
            if (!IsRunning)
                throw new InvalidOperationException("Service is not running");

            lock(data)
            {
                var change = fileManagerModule.UpdateFile(fileName, content);
                data.AddSortedDomesticChanges(new List<FileChangeMetaData>() { change });
            }
        }
        #endregion
    }
}
