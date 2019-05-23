using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class Client : IClient
    {
        public void AddViews(int number = 1)
        {
            
        }

        public void Setup(string uid)
        {
            throw new NotImplementedException();
        }

        public void StartPeriodicRequests(int number = 1, int intervalInSeconds = 5)
        {
            throw new NotImplementedException();
        }

        public void StopPeriodicRequests()
        {
            throw new NotImplementedException();
        }
    }
}
