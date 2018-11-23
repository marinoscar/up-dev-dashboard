using System;
using System.IO;
using System.Security.Cryptography;

namespace data_collector
{
    public class QAReviewInfo
    {
        public QAReviewInfo() : this(default(FileInfo))
        {
            
        }

        public QAReviewInfo(FileInfo file)
        {
            Date = new DateTime(1900, 1, 1);
            ExtractedOnUTC = DateTime.UtcNow;
            Type = "None";
            Source = "Excel";
            if (file == null) return;
            FileName = file.Name;
            FullFileName = file.FullName;
            Hash = CalculateMD5(file.FullName);


        }

        private string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public string FileName { get; set; }
        public string FullFileName { get; set; }
        public string Type { get; set; }
        public string ProcessName { get; set; }
        public DateTime Date { get; set; }
        public string DeveloperName { get; set; }
        public string ReviewerName { get; set; }
        public bool Failed { get; set; }
        public string Message { get; set; }
        public DateTime ExtractedOnUTC { get; set; }
        public string Source { get; set; }
        public string Hash { get; set; }
    }
}