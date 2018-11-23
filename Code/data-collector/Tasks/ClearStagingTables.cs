using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector.Tasks
{
    public class ClearStagingTables : DataTaskBase
    {
        public override Dictionary<string, object> DoExecute(Dictionary<string, object> input)
        {
            var db = new SqlHelper().GetDb();
            var sql = @"
delete from Staging_CheckListData;
delete from Staging_RuleImport;
delete from Staging_TFS;
";
            var res = db.ExecuteNonQuery(sql);
            OnStatus("Cleared {0} rows from staging table data", res);
            return new Dictionary<string, object>();
        }
    }
}
