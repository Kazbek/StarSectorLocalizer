using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.DiffCreators
{
    public class FileDiffCreator
    {
        public string originalPath = @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\original";
        public string translatedPath = @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\translated";
        public string diffPath = @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\translation";

        public void MakeDiff()
        {
            ClearFolder(diffPath);

            foreach (string filePath in Directory.GetFiles(originalPath, "*", SearchOption.AllDirectories))
            {
                string translatedFilePath = filePath.Replace(originalPath, translatedPath);
                if (File.Exists(translatedFilePath) && !FileCompareSize(filePath, translatedFilePath))
                {
                    string newFilePath = translatedFilePath.Replace(translatedPath, diffPath);
                    Console.WriteLine($"\"{newFilePath}\"");
                    Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));
                    File.Copy(translatedFilePath, newFilePath);
                }
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

        // This method accepts two strings the represent two files to
        // compare. A return value of 0 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the
        // files are not the same.
        private bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is
            // equal to "file2byte" at this point only if the files are
            // the same.
            return ((file1byte - file2byte) == 0);
        }

        private bool FileCompareSize(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files
            // are not the same.
            return Math.Abs(fs2.Length - fs1.Length) < ((double)fs1.Length) * 0.3;
        }
    }

    
}
