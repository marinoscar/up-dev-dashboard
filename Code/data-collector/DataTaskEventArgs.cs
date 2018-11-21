using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector
{
    public class DataTaskEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
        public string Message { get; set; }
    }
}
