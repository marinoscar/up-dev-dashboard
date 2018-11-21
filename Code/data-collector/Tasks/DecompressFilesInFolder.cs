using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_collector.Tasks
{
    public class DecompressFilesInFolder : DataTaskBase
    {
        public Dictionary<string, object> DoExecute(string sourceFolder, string outputFolder)
        {
            var dirInfo = new DirectoryInfo(sourceFolder);
            var outputDir = new DirectoryInfo(outputFolder);
            PrepareOutput(outputDir);
            var files = dirInfo.GetFiles("*.zip", SearchOption.AllDirectories);
            foreach(var file in files)
            {
                OnStatus("Decompressing {0}", file.Name);
                ZipFile.ExtractToDirectory(file.FullName, Path.Combine(outputFolder, file.Name.Replace(file.Extension, "")));
            }
            return new Dictionary<string, object>();
        }

        private void PrepareOutput(DirectoryInfo dir)
        {
            if(dir.Exists)
                dir.Delete(true);
            dir.Create();
        }

        public override Dictionary<string, object> DoExecute(Dictionary<string, object> input)
        {
            return DoExecute(Convert.ToString(input["sourceFolder"]), Convert.ToString(input["outputFolder"]));
        }
    }
}
