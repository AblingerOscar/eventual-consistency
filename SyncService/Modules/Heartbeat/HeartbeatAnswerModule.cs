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
    class HeartbeatAnswerModule : IModule
    {
        public delegate void ModuleOutdatedLocalChangesHandler(HeartbeatAnswerModule source, OutdatedLocalChangesArgs args);
        public event ModuleOutdatedLocalChangesHandler OnOutdatedLocalChanges;

        public delegate void ModuleHeartbeatAnswerReceivedHandler(HeartbeatAnswerModule source, HeartbeatAnswerReceivedArgs args);
        public event ModuleHeartbeatAnswerReceivedHandler OnHeartbeatAnswerReceived;

        public bool IsActive { get; private set; }

        private string serviceUID;
        private IConnection receiveConnection;
        private IModel receiveChannel;
        private string consumerTag;
        private Func<IDictionary<string, DateTime>> getKnownChanges;


        public HeartbeatAnswerModule(string serviceUID, Func<IDictionary<string, DateTime>> getKnownChanges)
        {
            IsActive = false;
            this.serviceUID = serviceUID;
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

        public void Activate()
        {
            IsActive = true;
            StartListening();
        }

        public void Deactivate()
        {
            IsActive = false;
            StopListening();
        }

        #region Private Methods
        #region StartListening
        private void StartListening()
        {
            DeclareConnectionAndChannel();
            DeclareExchange();
            var queueName = DeclareQueue();
            RegisterConsumer(queueName);
        }

        private void DeclareConnectionAndChannel()
        {
            var factory = new ConnectionFactory() { HostName = ConnectionConfiguration.HOSTNAME };
            receiveConnection = factory.CreateConnection();
            receiveChannel = receiveConnection.CreateModel();
        }

        private void DeclareExchange()
        {
            receiveChannel.ExchangeDeclare(
                exchange: ConnectionConfiguration.HEARTBEAT_ANSWER_EXCHANGE_NAME,
                type: "fanout"
                );
        }

        private string DeclareQueue()
        {
            var queueName = receiveChannel.QueueDeclare().QueueName;
            receiveChannel.QueueBind(
                queue: queueName,
                exchange: ConnectionConfiguration.HEARTBEAT_ANSWER_EXCHANGE_NAME,
                routingKey: ""
                );

            return queueName;
        }

        private void RegisterConsumer(string queueName)
        {
            var consumer = new EventingBasicConsumer(receiveChannel);
            consumer.Received += (sender, ea) =>
            {
                var heartbeatReq = HeartbeatRequest.FromBytes(ea.Body);
                if (heartbeatReq.SenderUID == serviceUID)
                    return;

                OnHeartbeatAnswerReceived?.Invoke(this, new HeartbeatAnswerReceivedArgs(heartbeatReq));
            };
;
            consumerTag = receiveChannel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }
        #endregion

        #region StopListening
        private void StopListening()
        {
            CancelConsumer();
            DisposeConnectionAndChannel();
        }

        private void CancelConsumer()
        {
            receiveChannel.BasicCancel(consumerTag);
        }

        private void DisposeConnectionAndChannel()
        {
            receiveChannel.Dispose();
            receiveConnection.Dispose();
        }
        #endregion

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
                SenderUID = serviceUID,
                NewerChanges = newerChanges
            };

            SendHeartbeatAnswer(answer);
        }

        private void SendHeartbeatAnswer(HeartbeatAnswer heartbeatAnswer)
        {
            var factory = new ConnectionFactory() { HostName = ConnectionConfiguration.HOSTNAME };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.BasicPublish(
                    exchange: ConnectionConfiguration.HEARTBEAT_ANSWER_EXCHANGE_NAME,
                    routingKey: "",
                    basicProperties: null,
                    body: heartbeatAnswer.ToBytes()
                    );
            }
        }
        #endregion
        #endregion
    }
}
