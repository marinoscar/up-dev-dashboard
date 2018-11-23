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
    public class ImportTFSData : DataTaskBase
    {
        public override Dictionary<string, object> DoExecute(Dictionary<string, object> input)
        {
            var fileName = Convert.ToString(input["fileName"]);
            var data = JsonConvert.DeserializeObject<List<QAReviewInfo>>(File.ReadAllText(fileName));
            var files = data.Where(i => i.Type == "TFS").ToList();
            foreach (var file in files)
            {
                using (var excelPack = new ExcelPackage(new FileInfo(file.FullFileName)))
                {
                    OnStatus("Importing TFS data from {0}", excelPack.File.Name);
                    ImportFromFile(excelPack);
                }
            }
            return new Dictionary<string, object>();
        }

        private void ImportFromFile(ExcelPackage excelPack)
        {
            var helper = new SqlHelper();
            var values = new List<TFSItem>();
            var sheet = excelPack.Workbook.Worksheets[1];
            var row = 3;
            long id = 99;
            while (id > 0)
            {
                var item = LoadFromRoad(sheet, row);
                if (item.Id > 0)
                    values.Add(item);
                id = item.Id;
                row++;
            }
            helper.InsertItems(values);
        }

        private TFSItem LoadFromRoad(ExcelWorksheet sheet, int row)
        {
            return new TFSItem()
            {
                Id = Convert.ToInt64(sheet.Cells[row, 1].Value),
                TeamProject = Convert.ToString(sheet.Cells[row, 2].Value),
                AreaPath = Convert.ToString(sheet.Cells[row, 3].Value),
                WorkItemType = Convert.ToString(sheet.Cells[row, 4].Value),
                Title = Convert.ToString(sheet.Cells[row, 5].Value),
                AssignedTo = Convert.ToString(sheet.Cells[row, 6].Value),
                State = Convert.ToString(sheet.Cells[row, 7].Value),
                Tags = Convert.ToString(sheet.Cells[row, 8].Value),
                BoardColumn = Convert.ToString(sheet.Cells[row, 16].Value),
                BoardLane = Convert.ToString(sheet.Cells[row, 18].Value),
                ChangedBy = Convert.ToString(sheet.Cells[row, 19].Value),
                ChangedDate = ToDate(sheet.Cells[row, 21].Value),
                ClosedDate = ToDate(sheet.Cells[row, 22].Value),
                ClosingComments = Convert.ToString(sheet.Cells[row, 24].Value),
                Description = Convert.ToString(sheet.Cells[row, 25].Value),
                Effort = Convert.ToSingle(sheet.Cells[row, 26].Value),
                FinishedDate = ToDate(sheet.Cells[row, 27].Value),
                Priority = Convert.ToString(sheet.Cells[row, 30].Value),
                Reason = Convert.ToString(sheet.Cells[row, 31].Value),
                RemainingWork = Convert.ToSingle(sheet.Cells[row, 32].Value),
                ReproSteps = Convert.ToString(sheet.Cells[row, 33].Value),
                Severity = Convert.ToString(sheet.Cells[row, 34].Value),
                StartDate = ToDate(sheet.Cells[row, 35].Value),
                Resolution = Convert.ToString(sheet.Cells[row, 36].Value),
                TargetDate = ToDate(sheet.Cells[row, 37].Value),
                ItearationPath = Convert.ToString(sheet.Cells[row, 41].Value),
            };
        }

        private DateTime? ToDate(object item)
        {
            if (item == null) return null;
            if(item is DateTime) return Convert.ToDateTime(item);
            if(item is double || item is float || item is decimal)
            {
                return DateTime.FromOADate(Convert.ToDouble(item));
            }
            return Convert.ToDateTime(item);
        }
    }
}
