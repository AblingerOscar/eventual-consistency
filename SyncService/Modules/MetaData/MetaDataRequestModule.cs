using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedClasses;
using SharedClasses.DataObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncService.Modules.MetaData
{
    public class MetaDataRequestModule : RabbitMQNode, IModule
    {
        public delegate void OnMetaDataRequestReceivedHandler(MetaDataRequestModule source, MetaDataRequestReceivedArgs args);
        public OnMetaDataRequestReceivedHandler OnMetaDataRequestReceived;
        public bool IsActive { get; private set; }

        public MetaDataRequestModule() :
            base(ConnectionConfiguration.METADATA_REQUEST_EXCHANGE_NAME, "direct")
        {
            IsActive = false;
        }

        public void Activate(string serviceId)
        {
            IsActive = true;
            StartListening(new string[] { serviceId });
        }

        public void Deactivate()
        {
            IsActive = false;
            StopListening();
        }

        public void SendRequest(string receiverService, MetaDataRequest metaDataReq)
        {
            SendBasicPublish(receiverService, metaDataReq.ToBytes());
        }

        protected override void OnReceiveConsumer(object sender, BasicDeliverEventArgs ea)
        {
            var metaDataReq = MetaDataRequest.FromBytes(ea.Body);
            OnMetaDataRequestReceived?.Invoke(this, new MetaDataRequestReceivedArgs(metaDataReq));
        }
    }
}
