namespace SyncService.Modules
{
    public interface IModule
    {
        bool IsActive();
        void Activate();
        void Deactivate();
    }
}
