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
            var rootPath = Path.Combine(
                Path.GetDirectoryName(assemblyLocation),
                "..", // Debug
                "..", // bin
                "..", // Cheetah
                "..");

            DBDirectory = Path.Combine(rootPath, "database");
            SyncDirectory = Path.Combine(rootPath, "syncDir");
            ExampleFilesDirectory = Path.Combine(rootPath, "exampleFiles");

            Directory.CreateDirectory(DBDirectory);
        }

        public static readonly string DBDirectory;
        public static readonly string SyncDirectory;
        public static readonly string ExampleFilesDirectory;
    }
}
