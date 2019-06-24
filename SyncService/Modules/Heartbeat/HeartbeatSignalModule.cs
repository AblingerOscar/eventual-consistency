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
    class HeartbeatSignalModule : IModule
    {
        public delegate void HeartbeatSignalReceivedHandler(HeartbeatSignalModule source, HeartbeatSignalReceivedArgs args);
        public event HeartbeatSignalReceivedHandler OnHeartbeatSignalReceived;

        private string serviceUID;
        private IConnection receiveConnection;
        private IModel receiveChannel;
        private string consumerTag;
        private Func<IDictionary<string, DateTime>> getKnownChanges;

        public HeartbeatSignalModule(string serviceUID, Func<IDictionary<string, DateTime>> getKnownChanges)
        {
            IsActive = false;
            this.serviceUID = serviceUID;
            this.getKnownChanges = getKnownChanges;
        }

        public bool IsActive { get; private set; }

        public void SendHeartbeat()
        {
            if (!IsActive)
                throw new InvalidOperationException("Module 'HeartbeatSignalModule' is not active");

            var heartbeatDO = CreateHeartbeatRequestDO();
            SendHeartbeatDO(heartbeatDO);
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
        private HeartbeatRequest CreateHeartbeatRequestDO()
        {
            return new HeartbeatRequest()
            {
                SenderUID = serviceUID,
                KnownChanges = getKnownChanges.Invoke()
            };
        }

        private void SendHeartbeatDO(HeartbeatRequest heartbeatDO)
        {
            var factory = new ConnectionFactory();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.BasicPublish(
                    exchange: ConnectionConfiguration.HEARTBEAT_SIGNAL_EXCHANGE_NAME,
                    routingKey: "",
                    basicProperties: null,
                    body: heartbeatDO.ToBytes()
                    );
            }
        }

        #region StartListeningForHeartbeat
        private void StartListening()
        {
            DeclareConnectionAndChannel();
            DeclareExchange();
            var queueName = DeclareQueue();
            RegisterConsumer(queueName);
        }

        private void DeclareConnectionAndChannel()
        {
            var factory = new ConnectionFactory();
            receiveConnection = factory.CreateConnection();
            receiveChannel = receiveConnection.CreateModel();
        }

        private void DeclareExchange()
        {
            receiveChannel.ExchangeDeclare(
                exchange: ConnectionConfiguration.HEARTBEAT_SIGNAL_EXCHANGE_NAME,
                type: "fanout"
                );
        }

        private string DeclareQueue()
        {
            var queueName = receiveChannel.QueueDeclare().QueueName;
            receiveChannel.QueueBind(
                queue: queueName,
                exchange: ConnectionConfiguration.HEARTBEAT_SIGNAL_EXCHANGE_NAME,
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

                OnHeartbeatSignalReceived?.Invoke(this, new HeartbeatSignalReceivedArgs(heartbeatReq));
            };
;
            consumerTag = receiveChannel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }
        #endregion

        #region StopListeningForHeartbeat
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
        #endregion
    }
}
