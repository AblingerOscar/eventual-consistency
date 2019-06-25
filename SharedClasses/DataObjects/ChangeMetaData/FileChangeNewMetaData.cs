using Newtonsoft.Json;
using SharedClasses.DataObjects.ChangeMetaData.Serialization;
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

        public FileChangeNewMetaData(string fileName, string domesticServiceId, int patchId) :
            base(ChangeType.NEW, fileName, domesticServiceId, patchId)
        { }

        public FileChangeNewMetaData(string fileName, string domesticServiceId, int patchId, DateTime timestamp) :
            base(ChangeType.NEW, fileName, domesticServiceId, patchId, timestamp)
        { }
    }
}
