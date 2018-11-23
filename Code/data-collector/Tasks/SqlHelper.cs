using Luval.Orm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector.Tasks
{
    public class SqlHelper
    {
        public Database GetDb()
        {
            return new Database(@"Server=.\SQLEXPRESS;Database=XOM-DASH;Trusted_Connection=True;");
        }


        public void InsertItems(IEnumerable<object> items)
        {
            var dbContext = new DbContext(GetDb());
            foreach (var item in items)
            {
                dbContext.Add(item);
            }
            dbContext.SaveChanges() ;
        }
    }
}
