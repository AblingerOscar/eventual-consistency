using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SharedClasses
{
    public static class PersistenceConfiguration
    {
        static PersistenceConfiguration() {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            DBDirectory = Path.Combine(
                Path.GetDirectoryName(assemblyLocation),
                "..", // Debug
                "..", // bin
                "..", // Cheetah
                "..", // root
                "database");
            Directory.CreateDirectory(DBDirectory);
        }

        public static readonly string DBDirectory;
    }
}
