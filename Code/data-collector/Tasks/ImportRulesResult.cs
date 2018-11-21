using data_collector.Models;
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
    public class ImportRulesResult : ExcelBasedImporter
    {
        public override Dictionary<string, object> DoExecute(Dictionary<string, object> input)
        {
            return DoExecute<QAItem>(Convert.ToString(input["fileName"]), "QAFile");
        }

        protected override List<TEntity> GetData<TEntity>(QAReviewInfo info)
        {
            var res = new List<TEntity>();
            var fileInfo = new FileInfo(info.FileName);
            using (var pack = new ExcelPackage(fileInfo))
            {
                using (var sheet = pack.Workbook.Worksheets[1])
                {

                    for (int i = 2; i < 100; i++)
                    {
                        var val = sheet.Cells[i, 1].Value;
                        if (val == null || string.IsNullOrWhiteSpace(Convert.ToString(val))) return res;
                        res.Add((TEntity)Convert.ChangeType(LoadFromSheet(sheet, i, info), typeof(TEntity)));
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
}
