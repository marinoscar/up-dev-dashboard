using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector.Tasks
{
    public abstract class ExcelBasedImporter : DataTaskBase
    {

        public virtual Dictionary<string, object> DoExecute<TEntity>(string fileName, string type)
        {
            var helper = new SqlHelper();
            var data = JsonConvert.DeserializeObject<List<QAReviewInfo>>(File.ReadAllText(fileName));
            var items = data.Where(i => !string.IsNullOrWhiteSpace(i.Type) && i.Type == type).ToList();
            var res = new List<TEntity>();
            foreach (var item in items)
            {
                OnStatus("Importing {0} data from {1}", type, item.FullFileName);
                res.AddRange(GetData<TEntity>(item));
            }
            helper.InsertItems(res.Select(i => (object)i));
            return new Dictionary<string, object>();
        }

        protected abstract List<TEntity> GetData<TEntity>(QAReviewInfo item);
    }
}
