using SharedClasses;
using System.Net.Http;
using System.Threading;

namespace Client
{
    public class Client : IClient
    {
        private static readonly HttpClient client = new HttpClient();

        private string uid;
        private bool isSetup = false;
        private Timer currTimer = null;

        private class PeriodicRequestArgs
        {
            public int numberOfViewsInABatch;

            public PeriodicRequestArgs(int numberOfViewsInABatch)
            {
                this.numberOfViewsInABatch = numberOfViewsInABatch;
            }
        }

        public async void AddViews(int number = 1)
        {
            if (!isSetup)
                throw new NotSetupException("Tried to access client methods before Setup(uid) was called");

            if (number == 1)
                await client.PostAsync(ConnectionConfiguration.HostName + $"api/{uid}/addView", null);
            else
                await client.PostAsync(ConnectionConfiguration.HostName + $"api/{uid}/addViews/{number}", null);
        }

        public void Setup(string uid)
        {
            this.uid = uid;
            isSetup = true;
        }

        public void StartPeriodicRequests(int number = 1, int intervalInMilliseconds = 5)
        {
            if (!isSetup)
                throw new NotSetupException("Tried to access client methods before Setup(uid) was called");

            if (currTimer != null)
                StopPeriodicRequests();

            currTimer = new Timer(PeriodicRequestIteration, new PeriodicRequestArgs(number), 0, intervalInMilliseconds);
        }

        private void PeriodicRequestIteration(object args)
        {
            var periodicRequestArgs = args as PeriodicRequestArgs;
            AddViews(periodicRequestArgs.numberOfViewsInABatch);
        }

        public void StopPeriodicRequests()
        {
            if (!isSetup)
                throw new NotSetupException("Tried to access client methods before Setup(uid) was called");

            if (currTimer == null)
                return;

            currTimer.Dispose();
            currTimer = null;
        }
    }
}
