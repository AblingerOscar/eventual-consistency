using SharedClasses.DataObjects.ChangeMetaData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClasses.DataObjects
{
    [Serializable]
    public class MetaDataRequestAnswer
    {
        public ChangeSet DomesticChanges;
        public IList<ChangeSet> AlienChanges;

        public MetaDataRequestAnswer() { }

        public MetaDataRequestAnswer(ChangeSet domesticChanges, IList<ChangeSet> alienChanges)
        {
            DomesticChanges = domesticChanges;
            AlienChanges = alienChanges;
        }
    }
}
