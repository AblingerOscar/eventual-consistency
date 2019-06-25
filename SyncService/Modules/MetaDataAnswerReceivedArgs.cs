using SharedClasses.DataObjects;

namespace SyncService.Modules
{
    public class MetaDataAnswerReceivedArgs
    {
        public MetaDataRequestAnswer MetaDataAnswer;

        public MetaDataAnswerReceivedArgs(MetaDataRequestAnswer metaDataAnswer)
        {
            MetaDataAnswer = metaDataAnswer;
        }
    }
}