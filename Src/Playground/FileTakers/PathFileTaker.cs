using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.FileTakers
{
    public class PathFileTaker
    {
        public string SourceFolder { get; init; }
        public string DestinationFolder { get; init; }

        public void CopyFiles(List<string> relativeFilePaths)
        {
            Directory.CreateDirectory(DestinationFolder);
            ClearFolder(DestinationFolder);

            foreach (string filePath in relativeFilePaths)
            {
                string absoluteSourceFilePath = Path.Combine(SourceFolder, filePath);
                string absoluteTargetFilePath = Path.Combine(DestinationFolder, filePath);

                Directory.CreateDirectory(Path.GetDirectoryName(absoluteTargetFilePath));
                File.Copy(absoluteSourceFilePath, absoluteTargetFilePath);
                Console.WriteLine($"Copied: {absoluteTargetFilePath}");
            }
        }

        public void ClearFolder(string path)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}
