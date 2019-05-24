using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public interface IClient
    {
        /// <summary>
        /// On setup an uid for a service is defined. Therefore, one client
        /// will only connect to a single service. The reason for this behavior
        /// is the ability, to request (add a view) periodically.
        /// </summary>
        /// <param name="uid"></param>
        void Setup(string uid);

        /// <summary>
        /// A request to the Gateway is initiated that will cause the service
        /// defined in the setup to increase the number of views.
        /// </summary>
        void AddViews(int number = 1);

        /// <summary>
        /// With this methods the client can add periodically view to a service.
        /// The number of views added and the time between requests can be set.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="intervalInSeconds"></param>
        void StartPeriodicRequests(int number = 1, int intervalInMilliseconds = 5000);

        /// <summary>
        /// To stop periodic requests this method can be called.
        /// </summary>
        void StopPeriodicRequests();
    }
}
