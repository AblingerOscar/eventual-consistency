using SharedClasses.DataObjects;
using SyncService.Modules.MetaData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncService.Modules
{
    public class MetaDataShareModule : IMetaDataShareModule
    {
        public bool IsActive { get; private set; }
        public event MetaDataExchangeHandler OnMetaDataExchangeReceived;

        private MetaDataRequestModule requestModule;
        private MetaDataAnswerModule answerModule;

        private Func<DateTime, ChangeSet> getDomesticChangesSince;
        private Func<IDictionary<string, DateTime>, IList<ChangeSet>> getAlienChangesSince;

        public MetaDataShareModule(Func<DateTime, ChangeSet> getDomesticChangesSince, Func<IDictionary<string, DateTime>, IList<ChangeSet>> getAlienChangesSince)
        {
            this.getDomesticChangesSince = getDomesticChangesSince;
            this.getAlienChangesSince = getAlienChangesSince;

            requestModule = new MetaDataRequestModule();
            requestModule.OnMetaDataRequestReceived += OnMetaDataRequestReceived;
            answerModule = new MetaDataAnswerModule();
            answerModule.OnMetaDataAnswerReceived += OnMetaDataAnswerReceived;
        }

        public void Activate(string serviceId)
        {
            IsActive = true;
            requestModule.Activate(serviceId);
            answerModule.Activate(serviceId);
        }

        public void Deactivate()
        {
            IsActive = false;
            requestModule.Deactivate();
            answerModule.Deactivate();
        }

        public void SendMetaDataShareRequest(string serviceId, MetaDataRequest metaDataRequest)
        {
            requestModule.SendRequest(serviceId, metaDataRequest);
        }

        private void OnMetaDataRequestReceived(MetaDataRequestModule source, MetaDataRequestReceivedArgs args)
        {
            var answer = CreateAnswerForRequest(args.MetaDataReq);
            answerModule.SendMetaDataAnswer(args.MetaDataReq.ServiceId, answer);
        }

        private MetaDataRequestAnswer CreateAnswerForRequest(MetaDataRequest metaDataReq)
        {
            return new MetaDataRequestAnswer()
            {
                DomesticChanges = getDomesticChangesSince(metaDataReq.DomesticChangesSince),
                AlienChanges = getAlienChangesSince(metaDataReq.AlienChangesSince)
            };
        }

        private void OnMetaDataAnswerReceived(MetaDataAnswerModule source, MetaDataAnswerReceivedArgs args)
        {
            OnMetaDataExchangeReceived?.Invoke(this, args);
        }
    }
}
