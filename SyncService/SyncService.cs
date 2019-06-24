using System;
using System.Collections.Generic;
using System.Text;
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

        private IList<IModule> modules;

        public SyncService()
        {
            modules = new List<IModule>();
            SetUpHeartbeatModule();
        }

        private void SetUpHeartbeatModule()
        {
            var heartbeatModule = new HeartbeatModule(GetKnownChanges);
            modules.Add(heartbeatModule);

            heartbeatModule.OnHeartbeatAnswerReceived += (sender, args) =>
            {
                OnLog?.Invoke(this, new OnLogHandlerArgs(
                    "Heartbeat answer received from " + args.HeartbeatReq.SenderUID + " with " + args.HeartbeatReq.KnownChanges.Count + " known changes",
                    LogReason.RABBITMQ_COMMUNICATION
                    ));
            };

            heartbeatModule.OnOutdatedLocalChanges += (sender, args) =>
            {
                OnLog?.Invoke(this, new OnLogHandlerArgs(
                    $"Found {args.OutdatedLocalChanges.Count} outdated local changes from: " +
                        string.Join(", ", args.OutdatedLocalChanges),
                    LogReason.RABBITMQ_COMMUNICATION));
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
            // TODO: save everything
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
            (modules[0] as HeartbeatModule).SendHeartbeat();
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
            // TODO: Actually deserialize data
            LastDomesticChangeTime = DateTime.Now;
            LastKnownChangeTime = new Dictionary<string, DateTime>();
        }

        private void ActivateModules()
        {
            foreach(var module in modules)
            {
                module.Activate(UID);
            }
        }

        private void DeactivateModules()
        {
            foreach(var module in modules)
            {
                module.Deactivate();
            }
        }
    }
}
