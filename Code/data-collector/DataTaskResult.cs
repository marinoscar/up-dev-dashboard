using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector
{
    public class DataTaskResult
    {
        public DataTaskResult()
        {
            Data = new Dictionary<string, object>(); 
        }
        public bool Sucess { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
