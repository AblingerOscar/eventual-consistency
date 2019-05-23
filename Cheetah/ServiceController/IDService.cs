using System;
using System.Collections.Generic;
using System.Text;

namespace Cheetah.ServiceController
{
    internal static class IDService
    {
        private static int highestId = -1;
        private readonly static object highestIdLock = new object();

        /// <summary>
        /// Generates a new ID.
        /// The IDs generated will be unique for the runtime an will be sequential. (For convenience in the CLI)
        /// </summary>
        /// <returns>A new ID</returns>
        internal static int GenerateNextID()
        {
            int newId = 0;
            lock(highestIdLock)
            {
                newId = ++highestId;
            }
            return newId;
        }

        /// <summary>
        /// Returns the service's uid for a given ServiceInformation id
        /// </summary>
        /// <param name="id">ServiceInformation id</param>
        /// <returns>The service's uid</returns>
        internal static string GetServiceUIDForId(int id)
        {
            return "service-" + id;
        }
    }
}
