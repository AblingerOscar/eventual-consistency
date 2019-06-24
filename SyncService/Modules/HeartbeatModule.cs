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

        private Func<IDictionary<string, DateTime>> getKnownChanges;
        private HeartbeatSignalModule signalModule;
        private HeartbeatAnswerModule answerModule;

        public HeartbeatModule(Func<IDictionary<string, DateTime>> getKnownChanges)
        {
            IsActive = false;
            this.getKnownChanges = getKnownChanges;
            AddHeartbeatAnswerModule();
            AddHeartbeatRequestModule();
        }

        private void AddHeartbeatAnswerModule()
        {
            answerModule = new HeartbeatAnswerModule(getKnownChanges);
            answerModule.OnOutdatedLocalChanges += OnOutdatedLocalChangesHandler;
            answerModule.OnHeartbeatAnswerReceived += OnHeartbeatAnswerReceivedHandler;
        }

        private void AddHeartbeatRequestModule()
        {
            signalModule = new HeartbeatSignalModule(getKnownChanges);
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

        public void SendHeartbeat()
        {
            signalModule.SendHeartbeat();
        }

        public void Activate(string serviceId)
        {
            IsActive = true;
            signalModule.Activate(serviceId);
            answerModule.Activate(serviceId);
        }

        public void Deactivate()
        {
            IsActive = false;
            signalModule.Deactivate();
            answerModule.Deactivate();
        }
    }
}
