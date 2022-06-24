using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.PatchExplorer
{
    public class TwoVersionPatchExplorer
    {
        public string OldVersionPath { get; init; }
        public string NewVersionPath { get; init; }
        public string TargetFolder { get; init; }

        public void ExploreDiff()
        {
            string newFilesPath = Path.Combine(TargetFolder, "new");
            string modifiedFilesPath = Path.Combine(TargetFolder, "modified");
            string deleteFilesPath = Path.Combine(TargetFolder, "delete");

            ClearFolder(TargetFolder);

            foreach (string newFilePath in Directory.GetFiles(NewVersionPath, "*", SearchOption.AllDirectories).Where(t => TranlatePossibleTypes.AvailableTypes.Any(p => t.EndsWith(p, StringComparison.OrdinalIgnoreCase))))
            {
                string oldFilePath = newFilePath.Replace(NewVersionPath, OldVersionPath);
                if (File.Exists(oldFilePath))
                {
                    if (!FileCompare(newFilePath, oldFilePath))
                    {
                        string copyFilePath = newFilePath.Replace(NewVersionPath, modifiedFilesPath);
                        Console.WriteLine($"[MODIFIED] \"{newFilePath}\"");
                        Directory.CreateDirectory(Path.GetDirectoryName(copyFilePath));
                        File.Copy(newFilePath, copyFilePath);
                    }
                }
                else
                {
                    string copyFilePath = newFilePath.Replace(NewVersionPath, newFilesPath);
                    Console.WriteLine($"[NEW] \"{newFilePath}\"");
                    Directory.CreateDirectory(Path.GetDirectoryName(copyFilePath));
                    File.Copy(newFilePath, copyFilePath);
                }
            }

            foreach (string newFilePath in Directory.GetFiles(OldVersionPath, "*", SearchOption.AllDirectories).Where(t => TranlatePossibleTypes.AvailableTypes.Any(p => t.EndsWith(p, StringComparison.OrdinalIgnoreCase))))
            {
                string oldFilePath = newFilePath.Replace(OldVersionPath, NewVersionPath);
                if (!File.Exists(oldFilePath))
                {
                    string copyFilePath = newFilePath.Replace(OldVersionPath, deleteFilesPath);
                    Console.WriteLine($"[DELETE] \"{newFilePath}\"");
                    Directory.CreateDirectory(Path.GetDirectoryName(copyFilePath));
                    File.Copy(newFilePath, copyFilePath);
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
    }
}
