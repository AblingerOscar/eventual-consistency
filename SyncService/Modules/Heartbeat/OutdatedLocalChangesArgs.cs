using System.Collections.Generic;

namespace SyncService.Modules.Heartbeat
{
    public class OutdatedLocalChangesArgs
    {
        public List<string> OutdatedLocalChanges;

        public OutdatedLocalChangesArgs(List<string> outdatedLocalChanges)
        {
            this.OutdatedLocalChanges = outdatedLocalChanges;
        }
    }
}