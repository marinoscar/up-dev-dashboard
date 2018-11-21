using data_collector.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector.Tasks
{
    public class ImportReviewCheckList : ExcelBasedImporter
    {
        public ImportReviewCheckList()
        {
        }

        public string ImportType { get; set; }

        public override Dictionary<string, object> DoExecute(Dictionary<string, object> input)
        {
            return DoExecute<CheckListData>(Convert.ToString(input["fileName"]), ImportType);
        }

        protected override List<TEntity> GetData<TEntity>(QAReviewInfo info)
        {
            var res = new List<TEntity>();
            var fileInfo = new FileInfo(info.FileName);
            using (var pack = new ExcelPackage(fileInfo))
            {
                using (var sheet = pack.Workbook.Worksheets[1])
                {
                    var start = FindDataIndex(sheet);
                    if (start < 0) return res;
                    for (int i = (start + 1); i < 200; i++)
                    {
                        var val = sheet.Cells[i, 1].Value;
                        if (val == null || string.IsNullOrWhiteSpace(Convert.ToString(val))) return res;
                        var item = (TEntity)Convert.ChangeType(LoadFromSheet(sheet, i, info, ImportType, 0), typeof(TEntity));
                        if (item == null) return res;
                        res.Add(item);
                    }
                }
            }
            return res;
        }

        private int FindDataIndex(ExcelWorksheet sheet)
        {
            for (int i = 1; i < 51; i++)
            {
                var val = Convert.ToString(sheet.Cells[i, 1].Value);
                if (string.IsNullOrWhiteSpace(val)) continue;
                val = val.ToLowerInvariant().Trim();
                if (val == "description") return i;
            }
            return -1;
        }

        private CheckListData LoadFromSheet(ExcelWorksheet sheet, int i, QAReviewInfo info, string type, int emptyLines)
        {
            if (emptyLines > 4)
                return null;
            var ruleText = Convert.ToString(sheet.Cells[i, 1].Value);
            if (string.IsNullOrWhiteSpace(ruleText))
            {
                emptyLines++;
                i++;
                return LoadFromSheet(sheet, i, info, type, emptyLines);
            }
            var result = Convert.ToString(sheet.Cells[i, 2].Value);
            if(string.IsNullOrWhiteSpace(result))
            {
                i++;
                LoadFromSheet(sheet, i, info, type, emptyLines);
            }
            return new CheckListData()
            {
                ProcessName = info.ProcessName,
                DeveloperName = info.DeveloperName,
                Date = info.Date,
                RuleName = ruleText.Trim(),
                FileName = info.FileName,
                Comment = Convert.ToString(sheet.Cells[i, 3].Value),
                Status = result,
                Type = type,
                UtcCreatedOn = DateTime.UtcNow
            };
        }
    }
}
