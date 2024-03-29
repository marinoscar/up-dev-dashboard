﻿using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace data_collector.Tasks
{
    public class ClassifyExcelFiles : DataTaskBase
    {
        public override Dictionary<string, object> DoExecute(Dictionary<string, object> input)
        {
            return DoExecute(Convert.ToString(input["sourceFolder"]), Convert.ToString(input["resultFile"]));
        }

        public Dictionary<string, object> DoExecute(string sourceFolder, string resultFile)
        {
            var fileData = new List<QAReviewInfo>();
            var dirInfo = new DirectoryInfo(sourceFolder);
            var files = dirInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                OnStatus("Reading file {0}", file.Name);
                try
                {
                    using (var excelPack = new ExcelPackage(file))
                    {
                        if (IsQAOutPut(excelPack))
                        {
                            OnStatus("File {0} is QA output", file.Name);
                            fileData.Add(GetQAFileInformation(excelPack));
                            continue;
                        }
                        if (IsReviewType(excelPack, "peer"))
                        {
                            OnStatus("File {0} is Peer review", file.Name);
                            fileData.Add(GetReviewInfo(excelPack, "PeerReview"));
                            continue;
                        }
                        if (IsReviewType(excelPack, "technical"))
                        {
                            OnStatus("File {0} is T.A review", file.Name);
                            fileData.Add(GetReviewInfo(excelPack, "TAReview"));
                            continue;
                        }
                        if (IsTFS(excelPack))
                        {
                            OnStatus("File {0} is TFS", file.Name);
                            fileData.Add(GetTFSInfo(excelPack));
                            continue;
                        }
                        fileData.Add(new QAReviewInfo() { Type = "None", FullFileName = file.FullName });
                    }
                }
                catch (Exception ex)
                {
                    fileData.Add(new QAReviewInfo()
                    {
                        FullFileName = file.FullName,
                        Failed = true,
                        Message = ex.ToString()
                    });
                }
            }
            var json = JsonConvert.SerializeObject(fileData, Formatting.Indented);
            File.WriteAllText(resultFile, json);
            return new Dictionary<string, object>() {
                { "FileData", fileData }
            };
        }

        private bool IsTFS(ExcelPackage excelPack)
        {
            var singleSheet = excelPack.Workbook.Worksheets.Count == 2 && excelPack.Workbook.Worksheets[1].Name == "Sheet1";
            if (!singleSheet) return false;
            var firstRow = Convert.ToString(excelPack.Workbook.Worksheets[1].Cells[1, 1].Value);
            var secondRow = Convert.ToString(excelPack.Workbook.Worksheets[1].Cells[2, 1].Value);
            return !string.IsNullOrWhiteSpace(firstRow) && firstRow.ToLowerInvariant().StartsWith("project: fl") &&
                !string.IsNullOrWhiteSpace(secondRow) && secondRow.ToLowerInvariant().StartsWith("id");
        }

        private bool IsReviewType(ExcelPackage excelPack, string keyword)
        {
            var nameCheck = excelPack.Workbook.Worksheets.Count == 1;
            if (!nameCheck) return false;
            var sheet = excelPack.Workbook.Worksheets[1];
            return ReviewFindTitle(sheet, keyword);
        }

        private QAReviewInfo GetTFSInfo(ExcelPackage excelPack)
        {
            var sheet = excelPack.Workbook.Worksheets[1];
            return new QAReviewInfo(excelPack.File)
            {
                DeveloperName = "NA",
                ProcessName = "NA",
                ReviewerName = "NA",
                Type = "TFS",
                Date = DateTime.Today
            };
        }

        private QAReviewInfo GetReviewInfo(ExcelPackage excelPack, string type)
        {
            var sheet = excelPack.Workbook.Worksheets[1];
            var res = new QAReviewInfo(excelPack.File)
            {
                DeveloperName = FindStringValue("Developer", sheet),
                ProcessName = FindStringValue("Process Name", sheet),
                ReviewerName = FindStringValue("Reviewer", sheet),
                Type = type,
                Date = FinDateValue("Review date", sheet)
            };
            if (res.Date <= new DateTime(2000, 1, 1))
                res.Date = File.GetLastWriteTime(excelPack.File.FullName);
            return res;
        }



        private string FindStringValue(string startWith, ExcelWorksheet sheet)
        {
            return CleanText(Convert.ToString(FindValue(startWith, sheet)));
        }

        private object FindValue(string startWith, ExcelWorksheet sheet)
        {
            var look = CleanText(startWith.ToLowerInvariant());
            for (int i = 0; i < 20; i++)
            {
                var val = Convert.ToString(sheet.Cells[i + 1, 1].Value);
                if (string.IsNullOrWhiteSpace(val)) continue;
                val = CleanText(val.ToLowerInvariant());
                if (val.StartsWith(look))
                    return sheet.Cells[i + 1, 2].Value;
            }
            return null;
        }

        private DateTime FinDateValue(string startWith, ExcelWorksheet sheet)
        {
            var dt = new DateTime(1900, 1, 1);
            var val = FindValue(startWith, sheet);
            if (val == null) return dt;
            if (val is DateTime) return Convert.ToDateTime(val);
            return dt;
        }

        private string CleanText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            return string.Join(" ", text.Split(" ".ToCharArray()).Select(i => i.Trim()));
        }

        private bool ReviewFindTitle(ExcelWorksheet sheet, string keyword)
        {
            var text = "";
            for (int i = 0; i < 10; i++)
            {
                text = Convert.ToString(sheet.Cells[i + 1, 1].Value);
                if (string.IsNullOrWhiteSpace(text)) continue;
                text = text.Trim().ToLowerInvariant();
                var words = text.Split(" ".ToCharArray()).Select(w => w.Trim()).ToList();
                if (words.Contains(keyword.ToLowerInvariant()) && words.Contains("review")) return true;
            }
            return false;
        }

        private bool IsQAOutPut(ExcelPackage excelPack)
        {
            var nameCheck = excelPack.Workbook.Worksheets.Count > 5 && excelPack.Workbook.Worksheets[1].Name == "Overview";
            if (!nameCheck) return false;
            var sheet = excelPack.Workbook.Worksheets[1];
            var valuecheck = !string.IsNullOrWhiteSpace(Convert.ToString(sheet.Cells["A1"].Value)) &&
                Convert.ToString(sheet.Cells["A1"].Value).Trim() == "No." &&
                !string.IsNullOrWhiteSpace(Convert.ToString(sheet.Cells["B1"].Value)) &&
                Convert.ToString(sheet.Cells["B1"].Value).Trim() == "Check";
            return nameCheck && valuecheck;
        }

        private QAReviewInfo GetQAFileInformation(ExcelPackage excelPack)
        {
            var file = excelPack.File;
            return new QAReviewInfo(excelPack.File)
            {
                ProcessName = GetQAProcessName(file.Name),
                DeveloperName = "NA",
                ReviewerName = "NA",
                Type = "QAFile",
                Date = GetDate(file.Name),
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
                Convert.ToInt32(ds.Substring(4, 2)),
                Convert.ToInt32(ds.Substring(6, 2)));
        }
    }
}
