using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace data_collector.Tasks
{
    public class ClassifyExcelFiles : DataTaskBase
    {
        public override Dictionary<string, object> DoExecute(Dictionary<string, object> input)
        {
            return new Dictionary<string, object>();
        }

        public Dictionary<string, object> DoExecute(string sourceFolder)
        {
            var dirInfo = new DirectoryInfo(sourceFolder);
            var files = dirInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                OnStatus("Reading file {0}", file.Name);
                var excelPack = new ExcelPackage(file);


            }
            return new Dictionary<string, object>();
        }

        private bool IsQAOutPut(ExcelPackage excelPack)
        {
            var nameCheck = excelPack.Workbook.Worksheets.Count > 5 && excelPack.Workbook.Worksheets[0].Name == "Overview";
            if (!nameCheck) return false;
            var sheet = excelPack.Workbook.Worksheets[0];
            var valuecheck =  !string.IsNullOrWhiteSpace(Convert.ToString(sheet.Cells["A1"].Value)) && 
                Convert.ToString(sheet.Cells["A1"].Value).Trim() == "No." &&
                !string.IsNullOrWhiteSpace(Convert.ToString(sheet.Cells["B!"].Value)) &&
                Convert.ToString(sheet.Cells["B1"].Value).Trim() == "Check";
            return nameCheck && valuecheck;
        }

        private InputInformation GetQAFileInformation(ExcelPackage excelPack)
        {
            var file = excelPack.File;
            var res = new InputInformation()
            {
                ProcessName = GetQAProcessName(file.Name),
                FileName = file.FullName,
                DeveloperName = "NA",
                ReviwerName = "NA",
                Type = "QAFile",
                Date = GetDate(file.Name)
            };
        }

        private string GetQAProcessName(string name)
        {
            var res = Regex.Match(name, @".*?_[1-9]");
            if (!res.Success) return null;
            return res.Value.Remove(res.Value.Length - 2, 2);
        }

        private DateTime GetDate(string name)
        {
            var res = new DateTime(1900, 1, 1);
            var pro = Regex.Match(name, @".*?_[1-9]");
            if (!pro.Success || string.IsNullOrWhiteSpace(pro.Value)) return res;
            var work = name.Replace(pro.Value, "");
            var items = work.Split("_".ToCharArray());
            if (items.Count() < 2) return res;
            var ds = items[1].Split("-".ToCharArray())[0];
            return new DateTime(Convert.ToInt32(ds.Substring(0, 4)),
                Convert.ToInt32(ds.Substring(3, 2)),
                Convert.ToInt32(ds.Substring(5, 2)));
        }
    }

    public class InputInformation
    {
        public string FileName { get; set; }
        public string Type { get; set; }
        public string ProcessName { get; set; }
        public DateTime Date { get; set; }
        public string DeveloperName { get; set; }
        public string ReviwerName { get; set; }
    }
}
