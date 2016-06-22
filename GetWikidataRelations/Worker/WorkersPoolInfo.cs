using System.Collections.Generic;

namespace GetWikidataRelations.Worker
{
    public class WorkersPoolInfo
    {
        public List<string> Properties { get; set; }
        public List<string> Categories { get; set; }

        public List<CategoryWorkerInfo> CategoryWorkers { get; set; }
        public List<PropertyWorkerInfo> PropertyWorkers { get; set; }
    }

    public class CategoryWorkerInfo : WorkerInfo
    {
        public int StartCategory { get; set; }
        public int EndCategory { get; set; }
    }

    public class PropertyWorkerInfo : WorkerInfo
    {
        public int StartProperty { get; set; }
        public int EndProperty { get; set; }
    }

    public class WorkerInfo
    {
        public int CategoryIndex { get; set; }
        public int PropertyIndex { get; set; }
        public long Offset { get; set; }

        public bool Started { get; set; }
        public bool Done { get; set; }
    }
}
