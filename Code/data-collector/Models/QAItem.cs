using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector.Models
{
    [Table(Name = "Staging_RuleImport")]
    public class QAItem
    {
        public QAItem()
        {
            UtcCreatedOn = DateTime.UtcNow;
        }

        [Luval.Orm.DataAnnotations.AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string FileName { get; set; }
        public string ProcessName { get; set; }
        public string DeveloperName { get; set; }
        public int RuleNo { get; set; }
        public string RuleName { get; set; }
        public string Status { get; set; }
        public string Analysis { get; set; }
        public string Criteria { get; set; }
        public DateTime UtcCreatedOn { get; set; }

    }
}
