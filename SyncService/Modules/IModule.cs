namespace SyncService.Modules
{
    public interface IModule
    {
        bool IsActive { get; }
        void Activate(string serviceId);
        void Deactivate();
    }
}
