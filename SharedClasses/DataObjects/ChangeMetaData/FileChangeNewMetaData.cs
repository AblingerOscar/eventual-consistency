using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses.DataObjects.ChangeMetaData
{
    [FileChangeType(ChangeType.NEW)]
    public class FileChangeNewMetaData : FileChangeMetaData
    {
        public FileChangeNewMetaData() : base(ChangeType.NEW)
        { }

        public FileChangeNewMetaData(string fileName, string fileHash) : base(ChangeType.NEW, fileName, fileHash)
        { }

        public FileChangeNewMetaData(string fileName, string fileHash, DateTime timestamp) : base(ChangeType.NEW, fileName, fileHash, timestamp)
        { }
    }
}
