using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector
{
    public interface IDataTask
    {
        DataTaskResult Execute(Dictionary<string, object> input);
        event EventHandler<DataTaskEventArgs> Status;
    }
}
