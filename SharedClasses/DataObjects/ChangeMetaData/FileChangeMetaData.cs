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
        public string DomesticServiceId;
        public int PatchId;
        public DateTime TimeStamp = DateTime.Now;

        protected FileChangeMetaData(ChangeType type)
        {
            Type = type;
        }

        public FileChangeMetaData(ChangeType type, string fileName, string domesticServiceId, int patchId) : this(type)
        {
            FileName = fileName;
            PatchId = patchId;
        }

        public FileChangeMetaData(ChangeType type, string fileName, string domesticServiceId, int patchId, DateTime timestamp) :
            this(type, fileName, domesticServiceId, patchId) {
            TimeStamp = timestamp;
        } 

        public override string ToString()
        {
            return $"FileChangeAbstractDO:" +
                $"\ttype: {Type.ToString()}\n" +
                $"\tfile name: {FileName}" +
                $"\tdomestic on service: {DomesticServiceId}" +
                $"\tpatch id: {PatchId}";
        }
    }
}