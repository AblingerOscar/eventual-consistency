using System;
using System.Collections.Generic;
using System.Text;

namespace ViewService
{
    public interface IViewService
    {
        /// <summary>
        /// Starts the client in the current Thread
        /// </summary>
        /// <param name="uid">The unique identifier for the Client. Used to map views to (other) clients</param>
        /// <param name="contextPath">The folder in which the Client operates (e.g. saves their views)</param>
        void StartUp(string uid, string contextPath);

        /// <summary>
        /// Shuts down the Client gracefully. It persists its data and might send a last update to other Clients
        /// </summary>
        void ShutDown();

        /// <summary>
        /// Shuts down the Client simulating an unhandled exception or a physical failure.
        /// </summary>
        void Abort();

        /// <summary>
        /// Returns the current total amount of views that the service knows of.
        /// </summary>
        /// <returns>Current total amount of views that the service knows of</returns>
        int GetViewCount();

        /// <summary>
        /// Increase the number of views for the service.
        /// </summary>
        void AddViews(int number = 1);

        /// <summary>
        /// Returns whether the service is running or stopped (not started, abort, or shutdown).
        /// </summary>
        bool IsRunning();
    }
}
