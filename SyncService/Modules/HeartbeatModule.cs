using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedClasses;
using SharedClasses.DataObjects;
using SyncService.Modules.Heartbeat;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncService.Modules
{
    public class HeartbeatModule : IHeartbeatModule
    {
        public bool IsActive { get; private set; }
        public event HeartbeatAnswerReceivedHandler OnHeartbeatAnswerReceived;
        public event OutdatedLocalChangesHandler OnOutdatedLocalChanges;
        public ISyncService Service { get; }

        private HeartbeatSignalModule signalModule;
        private HeartbeatAnswerModule answerModule;

        public HeartbeatModule(ISyncService service)
        {
            IsActive = false;
            Service = service;
            AddHeartbeatAnswerModule();
            AddHeartbeatRequestModule();
        }

        private void AddHeartbeatAnswerModule()
        {
            answerModule = new HeartbeatAnswerModule(Service.UID, GetKnownChanges);
            answerModule.OnOutdatedLocalChanges += OnOutdatedLocalChangesHandler;
            answerModule.OnHeartbeatAnswerReceived += OnHeartbeatAnswerReceivedHandler;
        }

        private void AddHeartbeatRequestModule()
        {
            signalModule = new HeartbeatSignalModule(Service.UID, GetKnownChanges);
            signalModule.OnHeartbeatSignalReceived += (source, args) =>
            {
                answerModule.AnswerHeartbeatRequest(args.HeartbeatReq);
            };
        }

        private void OnHeartbeatAnswerReceivedHandler(HeartbeatAnswerModule source, HeartbeatAnswerReceivedArgs args)
        {
            OnHeartbeatAnswerReceived?.Invoke(this, args);
        }

        private void OnOutdatedLocalChangesHandler(HeartbeatAnswerModule source, OutdatedLocalChangesArgs args)
        {
            OnOutdatedLocalChanges?.Invoke(this, args);
        }

        private IDictionary<string, DateTime> GetKnownChanges()
        {
            return new Dictionary<string, DateTime>(Service.LastKnownChangeTime)
            {
                { Service.UID, Service.LastDomesticChangeTime }
            };
        }

        public void SendHeartbeat()
        {
            signalModule.SendHeartbeat();
        }

        public void Activate()
        {
            IsActive = true;
            signalModule.Activate();
            answerModule.Activate();
        }

        public void Deactivate()
        {
            IsActive = false;
            signalModule.Deactivate();
            answerModule.Deactivate();
        }
    }
}
