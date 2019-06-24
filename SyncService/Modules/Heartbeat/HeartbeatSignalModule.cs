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
    public class HeartbeatSignalModule : RabbitMQNode, IModule
    {
        public delegate void HeartbeatSignalReceivedHandler(HeartbeatSignalModule source, HeartbeatSignalReceivedArgs args);
        public event HeartbeatSignalReceivedHandler OnHeartbeatSignalReceived;

        private string serviceId;
        private Func<IDictionary<string, DateTime>> getKnownChanges;

        public HeartbeatSignalModule(Func<IDictionary<string, DateTime>> getKnownChanges) :
            base(ConnectionConfiguration.HEARTBEAT_SIGNAL_EXCHANGE_NAME, "fanout")
        {
            IsActive = false;
            this.getKnownChanges = getKnownChanges;
        }

        public bool IsActive { get; private set; }

        public void SendHeartbeat()
        {
            if (!IsActive)
                throw new InvalidOperationException("Module 'HeartbeatSignalModule' is not active");

            var heartbeatDO = CreateHeartbeatRequestDO();
            SendBasicPublishFanout(heartbeatDO.ToBytes());
        }

        public void Activate(string serviceId)
        {
            this.serviceId = serviceId;
            IsActive = true;
            StartListeningFanout();
        }

        public void Deactivate()
        {
            serviceId = null;
            IsActive = false;
            StopListening();
        }


        protected override void OnReceiveConsumer(object sender, BasicDeliverEventArgs ea)
        {
            var heartbeatReq = HeartbeatRequest.FromBytes(ea.Body);
            if (heartbeatReq.SenderUID == serviceId)
                return;

            OnHeartbeatSignalReceived?.Invoke(this, new HeartbeatSignalReceivedArgs(heartbeatReq));
        }

        private HeartbeatRequest CreateHeartbeatRequestDO()
        {
            return new HeartbeatRequest()
            {
                SenderUID = serviceId,
                KnownChanges = getKnownChanges.Invoke()
            };
        }
    }
}
