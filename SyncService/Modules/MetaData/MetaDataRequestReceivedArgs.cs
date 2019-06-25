using SharedClasses.DataObjects;

namespace SyncService.Modules.MetaData
{
    public class MetaDataRequestReceivedArgs
    {
        public MetaDataRequest MetaDataReq { get; set; }

        public MetaDataRequestReceivedArgs(MetaDataRequest metaDataReq)
        {
            MetaDataReq = metaDataReq;
        }
    }
}