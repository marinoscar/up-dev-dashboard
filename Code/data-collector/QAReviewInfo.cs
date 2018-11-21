using System;

namespace data_collector
{
    public class QAReviewInfo
    {
        public QAReviewInfo()
        {
            Date = new DateTime(1900, 1, 1);
            ExtractedOnUTC = DateTime.UtcNow;
            Type = "None";
            Source = "Excel";
        }
        public string FileName { get; set; }
        public string Type { get; set; }
        public string ProcessName { get; set; }
        public DateTime Date { get; set; }
        public string DeveloperName { get; set; }
        public string ReviewerName { get; set; }
        public bool Failed { get; set; }
        public string Message { get; set; }
        public DateTime ExtractedOnUTC { get; set; }
        public string Source { get; set; }
    }
}