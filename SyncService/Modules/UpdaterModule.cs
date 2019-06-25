using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyncService.Modules
{
    public class UpdaterModule : IUpdaterModule
    {
        private const int INTERVAL = 10000;

        public bool IsActive { get; set; }

        public event Action<IUpdaterModule, IList<string>> OnUpdateAlienChanges;

        private List<string> outdatedServices;
        private Action debouncedInvokeUpdateAlienChanges;

        public UpdaterModule()
        {
            IsActive = false;
            outdatedServices = new List<string>();
            debouncedInvokeUpdateAlienChanges = Debounce(INTERVAL, () =>
            {
                lock (outdatedServices)
                {
                    if (outdatedServices.Count == 0)
                        return;

                    OnUpdateAlienChanges?.Invoke(this, outdatedServices);
                    outdatedServices.Clear();
                }
            });
        }

        public void Activate(string serviceId)
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void NotifyAboutOutdatedAlienChanges(IList<string> outdatedChanges)
        {
            lock(outdatedServices)
            {
                outdatedServices.AddRange(outdatedChanges);
            }

            debouncedInvokeUpdateAlienChanges();
        }

        private Action Debounce(int interval, Action action)
        {
            var last = 0;
            return () =>
            {
                var current = Interlocked.Increment(ref last);
                Task.Delay(interval).ContinueWith(task =>
                {
                    // Note that this means that if the module gets activated and
                    //   deactivated during the interval, this will still trigger
                    //   This is a small inaccuracy and deemed not worth fixing,
                    //   as that would require a lot more code
                    if (IsActive && current == last)
                        action();
                    task.Dispose();
                });
            };
        }
    }
}
