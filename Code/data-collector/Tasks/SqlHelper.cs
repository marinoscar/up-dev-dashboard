using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector.Tasks
{
    public class SqlHelper
    {
        public void InsertItems(IEnumerable<object> items)
        {
            var db = new Luval.Orm.Database(@"Server=.\SQLEXPRESS;Database=XOM-DASH;Trusted_Connection=True;", Luval.Orm.DatabaseProviderType.SqlServer);
            var dbContext = new Luval.Orm.DbContext(db);
            foreach (var item in items)
            {
                dbContext.Add(item);
            }
            dbContext.SaveChanges();
        }
    }
}
