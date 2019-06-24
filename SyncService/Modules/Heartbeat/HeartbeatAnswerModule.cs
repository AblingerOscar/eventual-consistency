using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedClasses;
using SharedClasses.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SyncService.Modules.Heartbeat
{
    public class HeartbeatAnswerModule : RabbitMQNode, IModule
    {
        public delegate void ModuleOutdatedLocalChangesHandler(HeartbeatAnswerModule source, OutdatedLocalChangesArgs args);
        public event ModuleOutdatedLocalChangesHandler OnOutdatedLocalChanges;

        public delegate void ModuleHeartbeatAnswerReceivedHandler(HeartbeatAnswerModule source, HeartbeatAnswerReceivedArgs args);
        public event ModuleHeartbeatAnswerReceivedHandler OnHeartbeatAnswerReceived;

        public bool IsActive { get; private set; }

        private string serviceID;
        private Func<IDictionary<string, DateTime>> getKnownChanges;


        public HeartbeatAnswerModule(Func<IDictionary<string, DateTime>> getKnownChanges) :
            base(ConnectionConfiguration.HEARTBEAT_ANSWER_EXCHANGE_NAME, "fanout")
        {
            IsActive = false;
            this.getKnownChanges = getKnownChanges;
        }

        public void AnswerHeartbeatRequest(HeartbeatRequest heartbeatReq)
        {
            if (!IsActive)
                throw new InvalidOperationException("Module 'HeartbeatAnswerModule' is not active");

            var localChanges = getKnownChanges();
            GetOutdatedChanges(localChanges, heartbeatReq.KnownChanges, out var outdatedLocalChanges, out var outdatedRequestChanges);
            NotifyOfOutdatedLocalChanges(outdatedLocalChanges);
            NotifyOfOutdatedRequestChanges(outdatedRequestChanges, localChanges);
        }

        public void Activate(string serviceId)
        {
            this.serviceID = serviceId;
            IsActive = true;
            StartListeningFanout();
        }

        public void Deactivate()
        {
            serviceID = null;
            IsActive = false;
            StopListening();
        }

        protected override void OnReceiveConsumer(object sender, BasicDeliverEventArgs ea)
        {
            var heartbeatReq = HeartbeatRequest.FromBytes(ea.Body);
            if (heartbeatReq.SenderUID == serviceID)
                return;

            OnHeartbeatAnswerReceived?.Invoke(this, new HeartbeatAnswerReceivedArgs(heartbeatReq));
        }

        #region AnswerHeartbeatRequest
        private void GetOutdatedChanges(IDictionary<string, DateTime> localChanges, IDictionary<string, DateTime> requestChanges, out List<string> outdatedLocalChanges, out List<string> outdatedRequestChanges)
        {
            var allServiceIds = localChanges.Keys.Union(requestChanges.Keys);

            outdatedLocalChanges = new List<string>();
            outdatedRequestChanges = new List<string>();

            foreach (var serviceId in allServiceIds)
            {
                CompareChanges(serviceId, localChanges, requestChanges, outdatedLocalChanges, outdatedRequestChanges);
            }
        }

        private void CompareChanges(string serviceId, IDictionary<string, DateTime> localChanges, IDictionary<string, DateTime> requestChanges,
            List<string> outdatedLocalChanges, List<string> outdatedRequestChanges)
        {
            var isInLocal = localChanges.TryGetValue(serviceId, out var localTimestamp);
            var isInRequest = requestChanges.TryGetValue(serviceId, out var requestTimestamp);

            if (RequestChangesAreOutdated(isInLocal, localTimestamp, isInRequest, requestTimestamp))
                outdatedRequestChanges.Add(serviceId);

            if (LocalChangesAreOutdated(isInLocal, localTimestamp, isInRequest, requestTimestamp))
                outdatedLocalChanges.Add(serviceId);
        }

        private bool RequestChangesAreOutdated(bool isInLocal, DateTime localTimestamp, bool isInRequest, DateTime requestTimestamp)
        {
            return !isInRequest || (isInLocal && DateTime.Compare(localTimestamp, requestTimestamp) > 0);
        }

        private bool LocalChangesAreOutdated(bool isInLocal, DateTime localTimestamp, bool isInRequest, DateTime requestTimestamp)
        {
            return !isInLocal || (isInRequest && DateTime.Compare(localTimestamp, requestTimestamp) < 0);
        }

        private void NotifyOfOutdatedLocalChanges(List<string> outdatedLocalChanges)
        {
            if (outdatedLocalChanges.Count > 0)
                OnOutdatedLocalChanges?.Invoke(this, new OutdatedLocalChangesArgs(outdatedLocalChanges));
        }

        private void NotifyOfOutdatedRequestChanges(List<string> outdatedRequestChanges, IDictionary<string, DateTime> localChanges)
        {
            if (outdatedRequestChanges.Count > 0)
                SendHeartbeatAnswerWithLocalChanges(outdatedRequestChanges.ToDictionary(k => k, k => localChanges[k]));
        }

        private void SendHeartbeatAnswerWithLocalChanges(Dictionary<string, DateTime> newerChanges)
        {
            var answer = new HeartbeatAnswer()
            {
                SenderUID = serviceID,
                NewerChanges = newerChanges
            };

            SendBasicPublishFanout(answer.ToBytes());
        }
        #endregion
    }
}
