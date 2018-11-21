using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector.Tasks
{
    public class CopyAllExcelFilesToDestination : DataTaskBase
    {
        public override Dictionary<string, object> DoExecute(Dictionary<string, object> input)
        {
            return DoExecute(Convert.ToString(input["sourceFolder"]), Convert.ToString(input["outputFolder"]));
        }

        public Dictionary<string, object> DoExecute(string sourceFolder, string outputFolder)
        {
            var dirInfo = new DirectoryInfo(sourceFolder);
            var files = dirInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);
            foreach(var file in files)
            {
                OnStatus("Copying file {0} to {1}", file.Name, outputFolder);
                File.Copy(file.FullName, Path.Combine(outputFolder, file.Name), true);
            }
            return new Dictionary<string, object>();
        }
    }
}
