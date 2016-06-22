using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetWikidataRelations.Out;

namespace GetWikidataRelations.Worker
{
    public class WorkersPool
    {
        private readonly string _id;
        private readonly int _categoryWorkers;
        private readonly int _propertyWorkers;
        private readonly Iio _io;
        private WorkersPoolInfo _configuration;

        public WorkersPool(string id, int categoryWorkers, int propertyWorkers, Iio io)
        {
            _id = id;
            _io = io;

            _categoryWorkers = categoryWorkers;
            _propertyWorkers = propertyWorkers;
        }

        public void Start()
        {
            _configuration = _io.GetInfo(_id);
        }
    }
}
