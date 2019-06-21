using Newtonsoft.Json;
using SharedClasses.DataObjects.ChangeMetaData.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses.DataObjects.ChangeMetaData
{
    [FileChangeType(ChangeType.DELETE)]
    public class FileChangeDeleteMetaData : FileChangeMetaData
    {
        [JsonConstructor]
        public FileChangeDeleteMetaData() : base(ChangeType.DELETE)
        { }

        public FileChangeDeleteMetaData(string fileName, string fileHash) : base(ChangeType.DELETE, fileName, fileHash)
        { }

        public FileChangeDeleteMetaData(string fileName, string fileHash, DateTime timestamp) : base(ChangeType.NEW, fileName, fileHash, timestamp)
        { }
    }
}
