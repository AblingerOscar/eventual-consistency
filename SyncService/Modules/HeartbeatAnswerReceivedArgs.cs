using SharedClasses.DataObjects;

namespace SyncService.Modules
{
    public class HeartbeatAnswerReceivedArgs
    {
        public HeartbeatRequest HeartbeatReq { get; }

        public HeartbeatAnswerReceivedArgs(HeartbeatRequest heartbeatReq)
        {
            HeartbeatReq = heartbeatReq;
        }
    }
}