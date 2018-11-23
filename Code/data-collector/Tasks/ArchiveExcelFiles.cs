using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector.Tasks
{
    public class ArchiveExcelFiles : DataTaskBase
    {
        public override Dictionary<string, object> DoExecute(Dictionary<string, object> input)
        {
            return DoExecute(Convert.ToString(input["sourceFolder"]), Convert.ToString(input["outputFolder"]));
        }
        public Dictionary<string, object> DoExecute(string sourceFolder, string outputFolder)
        {
            var dirInfo = new DirectoryInfo(outputFolder);
            if (!dirInfo.Exists) dirInfo.Create();
            var file = new 
                FileInfo(Path.Combine(dirInfo.FullName, string.Format("Archive-{0}.zip", 
                DateTime.Today.ToString("yyyy-MM-dd"))));
            if (!file.Exists) file.Delete();
            ZipFile.CreateFromDirectory(sourceFolder, file.FullName);
            return new Dictionary<string, object>();
        }
    }
}
