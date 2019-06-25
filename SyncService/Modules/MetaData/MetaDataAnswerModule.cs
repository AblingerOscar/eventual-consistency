using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client.Events;
using SharedClasses;
using SharedClasses.DataObjects;

namespace SyncService.Modules.MetaData
{
    public class MetaDataAnswerModule : RabbitMQNode, IModule
    {
        public delegate void ModuleMetaDataAnswerReceivedHandler(MetaDataAnswerModule source, MetaDataAnswerReceivedArgs args);
        public ModuleMetaDataAnswerReceivedHandler OnMetaDataAnswerReceived;

        public bool IsActive { get; set; }

        public MetaDataAnswerModule():
            base(ConnectionConfiguration.METADATA_ANSWER_EXCHANGE_NAME, "direct")
        {
            IsActive = false;
        }

        public void SendMetaDataAnswer(string serviceId, MetaDataRequestAnswer metaDataAnswer)
        {
            SendBasicPublish(serviceId, metaDataAnswer.ToByte());
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

        protected override void OnReceiveConsumer(object sender, BasicDeliverEventArgs ea)
        {
            var metaDataAnswer = MetaDataRequestAnswer.FromBytes(ea.Body);
            OnMetaDataAnswerReceived?.Invoke(this, new MetaDataAnswerReceivedArgs(metaDataAnswer));
        }
    }
}
