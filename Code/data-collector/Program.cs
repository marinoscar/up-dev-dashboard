﻿using System;
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

            ExecuteTask(deCompress, 
                new Dictionary<string, object> {
                    { "sourceFolder", @"C:\Users\oscar.marin\Desktop\TMP\TooTest" },
                    { "outputFolder", @"C:\Users\oscar.marin\Desktop\TMP\ToolTestOuput" }
                });
            ExecuteTask(copyAll,
                new Dictionary<string, object> {
                    { "sourceFolder", @"C:\Users\oscar.marin\Desktop\TMP\ToolTestOuput" },
                    { "outputFolder", @"C:\Users\oscar.marin\Desktop\TMP\ExcelOutput" }
                });
        }

        private static void ExecuteTask(IDataTask task, Dictionary<string, object> input)
        {
            task.Status += Task_Status;
            Console.WriteLine("Executing {0}", task.GetType().Name);
            var res = task.Execute(input);
        }

        private static void Task_Status(object sender, DataTaskEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}