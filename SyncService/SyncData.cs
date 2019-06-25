using SharedClasses.DataObjects;
using SharedClasses.DataObjects.ChangeMetaData;
using System;
using System.Collections.Generic;

namespace SyncService
{
    [Serializable]
    internal class SyncData
    {
        public List<FileChangeMetaData> DomesticChanges;
        public Dictionary<string, List<FileChangeMetaData>> AlienChanges;

        public SyncData()
        {
            DomesticChanges = new List<FileChangeMetaData>();
            AlienChanges = new Dictionary<string, List<FileChangeMetaData>>();
        }

        public void Sort()
        {
            SortDomesticChanges();
            SortAlienChanges();
        }

        public void AddSortedDomesticChanges(IList<FileChangeMetaData> sortedChanges)
        {
            var needsSorting = LastElementTimeStampOf(DomesticChanges) < LastElementTimeStampOf(sortedChanges);

            DomesticChanges.AddRange(sortedChanges);

            if (needsSorting)
                SortDomesticChanges();
        }

        public void AddSortedAlienChanges(string serviceId, IList<FileChangeMetaData> sortedChanges)
        {
            var needsSorting = LastElementTimeStampOf(AlienChanges[serviceId]) < LastElementTimeStampOf(sortedChanges);

            AlienChanges[serviceId].AddRange(sortedChanges);

            if (needsSorting)
                SortAlienChanges();
        }

        private DateTime LastElementTimeStampOf(IList<FileChangeMetaData> list)
        {
            return list[list.Count - 1].TimeStamp;
        }

        private void SortDomesticChanges()
        {
            DomesticChanges.Sort((x, y) => DateTime.Compare(x.TimeStamp, y.TimeStamp));
        }

        private void SortAlienChanges()
        {
            foreach (var alienChange in AlienChanges)
            {
                SortAlienChanges(alienChange.Key);
            }
        }

        private void SortAlienChanges(string serviceId)
        {
            AlienChanges[serviceId].Sort((x, y) => DateTime.Compare(x.TimeStamp, y.TimeStamp));
        }
    }
}