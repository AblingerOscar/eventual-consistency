using SharedClasses.DataObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncService.Modules
{
    public delegate void MetaDataExchangeHandler(IMetaDataShareModule sender, MetaDataAnswerReceivedArgs args);

    public interface IMetaDataShareModule: IModule
    {
        event MetaDataExchangeHandler OnMetaDataExchangeReceived;

        void SendMetaDataShareRequest(string serviceId, MetaDataRequest metaDataRequest);
    }
}
