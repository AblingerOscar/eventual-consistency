using SharedClasses.DataObjects;

namespace SyncService.Modules.Heartbeat
{
    public class HeartbeatSignalReceivedArgs
    {
        public HeartbeatRequest HeartbeatReq { get; }

        public HeartbeatSignalReceivedArgs(HeartbeatRequest heartbeatReq)
        {
            HeartbeatReq = heartbeatReq;
        }
    }
}