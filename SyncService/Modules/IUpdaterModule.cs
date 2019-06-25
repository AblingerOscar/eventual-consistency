using System;
using System.Collections.Generic;
using System.Text;

namespace SyncService.Modules
{
    public interface IUpdaterModule: IModule
    {
        event Action<IUpdaterModule, IList<string>> OnUpdateAlienChanges;

        void NotifyAboutOutdatedAlienChanges(IList<string> outdatedChanges);
    }
}
