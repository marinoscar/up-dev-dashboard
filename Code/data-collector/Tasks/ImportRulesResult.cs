using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector.Tasks
{
    public class ImportRulesResult : DataTaskBase
    {
        public override Dictionary<string, object> DoExecute(Dictionary<string, object> input)
        {
            return DoExecute(Convert.ToString(input["fileName"]));
        }

        public Dictionary<string, object> DoExecute(string fileName)
        {
            var helper = new SqlHelper();
            var data = JsonConvert.DeserializeObject<List<QAReviewInfo>>(File.ReadAllText(fileName));
            var items = data.Where(i => !string.IsNullOrWhiteSpace(i.Type) && i.Type == "QAFile").ToList();
            var res = new List<QAItem>();
            foreach (var item in items)
            {
                OnStatus("Importing data from {0}", item.FileName);
                res.AddRange(GetData(item));
            }
            helper.InsertItems(res);
            return new Dictionary<string, object>();
        }

        private IEnumerable<QAItem> GetData(QAReviewInfo info)
        {
            var res = new List<QAItem>();
            var fileInfo = new FileInfo(info.FileName);
            using (var pack = new ExcelPackage(fileInfo))
            {
                using (var sheet = pack.Workbook.Worksheets[1])
                {

                    for (int i = 2; i < 100; i++)
                    {
                        var val = sheet.Cells[i, 1].Value;
                        if (val == null || string.IsNullOrWhiteSpace(Convert.ToString(val))) return res;
                        res.Add(LoadFromSheet(sheet, i, info));
                    }
                }
            }
            return res;
        }

        private QAItem LoadFromSheet(ExcelWorksheet sheet, int idx, QAReviewInfo info)
        {
            return new QAItem()
            {
                DeveloperName = info.DeveloperName,
                Date = info.Date,
                FileName = info.FileName,
                ProcessName = info.ProcessName,
                UtcCreatedOn = DateTime.UtcNow,
                RuleNo = Convert.ToInt32(sheet.Cells[idx, 1].Value),
                RuleName = Convert.ToString(sheet.Cells[idx, 2].Value),
                Status = Convert.ToString(sheet.Cells[idx, 3].Value),
                Analysis = Convert.ToString(sheet.Cells[idx, 4].Value),
                Criteria = Convert.ToString(sheet.Cells[idx, 5].Value),
            };
        }
    }

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
