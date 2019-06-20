using SharedClasses.DataObjects.ChangeMetaData;
using System;
using System.Collections.Generic;

namespace SharedClasses.DataObjects
{
    public class ChangeSet
    {
        public DateTime Timestamp;
        public string ServiceUID;
        public IList<FileChangeMetaData> Changes;
    }
}
