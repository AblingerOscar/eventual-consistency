using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses.DataObjects.ChangeMetaData.Serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FileChangeTypeAttribute: Attribute
    {
        public FileChangeMetaData.ChangeType ChangeType;

        public FileChangeTypeAttribute(FileChangeMetaData.ChangeType changeType)
        {
            ChangeType = changeType;
        }
    }
}
