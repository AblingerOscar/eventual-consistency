using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SharedClasses.DataObjects;
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

        public SyncService()
        {
            SetUpHeartbeatModule();
            SetUpMetaDataShareModule();
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
            metaDataModule.OnMetaDataExchangeReceived += (sender, args) =>
            {
                var changes = new List<ChangeSet>(args.MetaDataAnswer.AlienChanges)
                {
                    args.MetaDataAnswer.DomesticChanges
                };

                foreach(var changeSet in changes)
                {
                    var changeData = changeSet.Changes.OrderBy(c => c.TimeStamp).ToList();
                    data.AddSortedAlienChanges(changeSet.ServiceUID, changeData);
                }
            };
        }

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

        public void StartUp(string uid, string syncPath, string savePath)
        {
            IsRunning = true;
            UID = uid;
            SyncPath = syncPath;
            SavePath = savePath;
            LoadSavedData();
            ActivateModules();
            heartbeatModule.SendHeartbeat();
        }

        private IDictionary<string, DateTime> GetKnownChanges()
        {
            return new Dictionary<string, DateTime>(LastKnownChangeTime)
            {
                { UID, LastDomesticChangeTime }
            };
        }

        private void LoadSavedData()
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

        private void UpdateLastChangesFromData()
        {
            LastDomesticChangeTime = data.DomesticChanges[data.DomesticChanges.Count - 1].TimeStamp;

            foreach (var alienChanges in data.AlienChanges)
            {
                LastKnownChangeTime[alienChanges.Key] = alienChanges.Value[alienChanges.Value.Count - 1].TimeStamp;
            }
        }

        private void SaveData()
        {
            if (data != null)
            {
                try
                {
                    File.WriteAllText(SavePath, JsonConvert.SerializeObject(data));
                }
                catch (Exception)
                {
                    OnLog?.Invoke(this, new OnLogHandlerArgs("Could not save data", LogReason.DEBUG, OutputLevel.ERROR));
                }
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
            var changesSince = data.DomesticChanges.SkipWhile(ch => ch.TimeStamp < timestamp).ToList();

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
            var changes = data.AlienChanges[serviceId].SkipWhile(ch => ch.TimeStamp < timestamp).ToList();

            return new ChangeSet()
            {
                Timestamp = changes.Last().TimeStamp,
                ServiceUID = serviceId,
                Changes = changes
            };
        }
    }
}
