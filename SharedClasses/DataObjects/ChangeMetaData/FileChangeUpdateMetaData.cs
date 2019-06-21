using Newtonsoft.Json;
using SharedClasses.DataObjects.ChangeMetaData.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses.DataObjects.ChangeMetaData
{
    [FileChangeType(ChangeType.UPDATE)]
    public class FileChangeUpdateMetaData : FileChangeMetaData
    {
        public string OldFileHash;

        [JsonConstructor]
        public FileChangeUpdateMetaData(string oldFileHash) : base(ChangeType.UPDATE)
        {
            OldFileHash = oldFileHash;
        }

        public FileChangeUpdateMetaData(string fileName, string fileHash, string oldFileHash) : base(ChangeType.NEW, fileName, fileHash)
        {
            OldFileHash = oldFileHash;
        }

        public FileChangeUpdateMetaData(string fileName, string fileHash, string oldFileHash, DateTime timestamp) : base(ChangeType.NEW, fileName, fileHash, timestamp)
        {
            OldFileHash = oldFileHash;
        }

        public override string ToString()
        {
            return base.ToString() +
                $"\tOld File Hash: {OldFileHash.ToString()}";
        }
    }

}
