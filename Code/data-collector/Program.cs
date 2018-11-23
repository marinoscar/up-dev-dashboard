using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_collector.Tasks;

namespace data_collector
{
    class Program
    {
        static void Main(string[] args)
        {
            var deCompress = new DecompressFilesInFolder();
            var copyAll = new CopyAllExcelFilesToDestination();
            var classify = new ClassifyExcelFiles();
            var importQA = new ImportRulesResult();
            var archive = new ArchiveExcelFiles();
            var clearStaging = new ClearStagingTables();
            var peerRev = new ImportReviewCheckList() { ImportType = "PeerReview" };
            var taRev = new ImportReviewCheckList() { ImportType = "TAReview" };
            var tfsData = new ImportTFSData();

            var jsonFileName = string.Format(@"C:\QA-Quality\Excel\output-{0}.json", DateTime.Today.ToString("yyyy-MM-dd"));

            ExecuteTask(deCompress, 
                new Dictionary<string, object> {
                    { "sourceFolder", @"C:\QA-Quality\Source" },
                    { "outputFolder", @"C:\QA-Quality\Decompressed" }
                });
            ExecuteTask(copyAll,
                new Dictionary<string, object> {
                    { "sourceFolder", @"C:\QA-Quality\Source" },
                    { "outputFolder", @"C:\QA-Quality\Excel" }
                });
            ExecuteTask(copyAll,
                new Dictionary<string, object> {
                    { "sourceFolder", @"C:\QA-Quality\Decompressed" },
                    { "outputFolder", @"C:\QA-Quality\Excel" }
                });
            ExecuteTask(classify,
                new Dictionary<string, object> {
                    { "sourceFolder", @"C:\QA-Quality\Excel" },
                    { "resultFile", jsonFileName }
                });
            ExecuteTask(clearStaging, new Dictionary<string, object>());
            ExecuteTask(importQA,
               new Dictionary<string, object> {
                    { "fileName", jsonFileName }
               });
            ExecuteTask(peerRev,
               new Dictionary<string, object> {
                    { "fileName", jsonFileName }
               });
            ExecuteTask(taRev,
               new Dictionary<string, object> {
                    { "fileName", jsonFileName }
               });
            ExecuteTask(tfsData,
               new Dictionary<string, object> {
                    { "fileName", jsonFileName }
               });
            ExecuteTask(archive,
                new Dictionary<string, object> {
                    { "sourceFolder", @"C:\QA-Quality\Excel" },
                    { "outputFolder", @"C:\QA-Quality\Archive" }
                });
        }

        private static void ExecuteTask(IDataTask task, Dictionary<string, object> input)
        {
            task.Status += Task_Status;
            Console.WriteLine("Executing {0}", task.GetType().Name);
            var res = task.Execute(input);
            if (!res.Sucess)
                Console.WriteLine("Failed: {0}", res.Message);
        }

        private static void Task_Status(object sender, DataTaskEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
