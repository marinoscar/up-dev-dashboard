using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector.Models
{
    public class CheckListData
    {
        public CheckListData()
        {
            UtcCreatedOn = DateTime.UtcNow;
        }
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; }
        public string ProcessName { get; set; }
        public string DeveloperName { get; set; }
        public string RuleName { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public DateTime UtcCreatedOn { get; set; }
    }
}
