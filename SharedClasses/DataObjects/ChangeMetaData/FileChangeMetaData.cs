using Newtonsoft.Json;
using SharedClasses.DataObjects.ChangeMetaData.Serialization;
using System;

namespace SharedClasses.DataObjects.ChangeMetaData
{
    [JsonConverter(typeof(ChangeConverter))]
    public abstract class FileChangeMetaData
    {
        public enum ChangeType {
            NEW, UPDATE, DELETE
        }

        public ChangeType Type { get; }
        public string FileName;
        public string FileHash;
        public DateTime TimeStamp = DateTime.Now;

        protected FileChangeMetaData(ChangeType type)
        {
            Type = type;
        }

        public FileChangeMetaData(ChangeType type, string fileName, string fileHash) : this(type)
        {
            FileName = fileName;
            FileHash = fileHash;
        }

        public FileChangeMetaData(ChangeType type, string fileName, string fileHash, DateTime timestamp) : this(type, fileName, fileHash) {
            TimeStamp = timestamp;
        } 

        public override string ToString()
        {
            return $"FileChangeAbstractDO:" +
                $"\ttype: {Type.ToString()}\n" +
                $"\tfile name: {FileName}" +
                $"\tfile hash: {FileHash}";
        }
    }
}