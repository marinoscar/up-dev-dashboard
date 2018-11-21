using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector
{
    public abstract class DataTaskBase : IDataTask
    {
        public event EventHandler<DataTaskEventArgs> Status;

        public virtual DataTaskResult Execute(Dictionary<string, object> input)
        {
            var res = new DataTaskResult() { Sucess = true, Message = "Success" };
            try
            {
                res.Data = DoExecute(input);
            }
            catch (Exception ex)
            {
                OnException(ex, res, input);
            }
            return res;
        }

        protected virtual void OnException(Exception ex, DataTaskResult res, Dictionary<string, object> input)
        {
            res.Sucess = false;
            res.Message = ex.ToString();
        }

        public abstract Dictionary<string, object> DoExecute(Dictionary<string, object> input);

        protected virtual bool OnStatus(string msg, params object[] args)
        {
            return OnStatus(string.Format(msg, args));
        }

        protected virtual bool OnStatus(string msg)
        {
            return OnStatus(new DataTaskEventArgs() { Message = msg });
        }

        protected virtual bool OnStatus(DataTaskEventArgs e)
        {
            Status?.Invoke(this, e);
            return e.Cancel;
        }

    }
}
