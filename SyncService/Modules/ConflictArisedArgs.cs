using SharedClasses.DataObjects.ChangeMetaData;

namespace SyncService.Modules
{
    public class ConflictArisedArgs
    {
        public FileChangeMetaData ContextMetaData;
        public string Message;

        public ConflictArisedArgs(FileChangeMetaData metaData, string message)
        {
            ContextMetaData = metaData;
            Message = message;
        }
    }
}