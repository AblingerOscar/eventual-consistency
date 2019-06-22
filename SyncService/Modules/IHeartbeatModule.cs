namespace SyncService.Modules
{
    public delegate void HeartbeatAnswerReceivedHandler(IHeartbeatModule source, HeartbeatAnswerReceivedArgs args);
    public delegate void OutdatedLocalChangesHandler(IHeartbeatModule source, OutdatedLocalChangesHandler args);

    public interface IHeartbeatModule: IModule
    {
        event HeartbeatAnswerReceivedHandler OnHeartbeatAnswerReceived;
        event OutdatedLocalChangesHandler OnOutdatedLocalChanges;

        void SendHeartbeat();
    }
}