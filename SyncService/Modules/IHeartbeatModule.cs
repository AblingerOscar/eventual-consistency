using SyncService.Modules.Heartbeat;

namespace SyncService.Modules
{
    public delegate void HeartbeatAnswerReceivedHandler(IHeartbeatModule source, HeartbeatAnswerReceivedArgs args);
    public delegate void OutdatedLocalChangesHandler(IHeartbeatModule source, OutdatedLocalChangesArgs args);

    public interface IHeartbeatModule: IModule
    {
        event HeartbeatAnswerReceivedHandler OnHeartbeatAnswerReceived;
        event OutdatedLocalChangesHandler OnOutdatedLocalChanges;

        void SendHeartbeat();
    }
}