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

        [JsonConstructor]
        public FileChangeUpdateMetaData() : base(ChangeType.UPDATE)
        {
        }

        public FileChangeUpdateMetaData(string fileName, string domesticServiceId, int patchId) :
            base(ChangeType.NEW, fileName, domesticServiceId, patchId)
        {
        }

        public FileChangeUpdateMetaData(string fileName, string domesticServiceId, int patchId, DateTime timestamp) :
            base(ChangeType.NEW, fileName, domesticServiceId, patchId, timestamp)
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

}
