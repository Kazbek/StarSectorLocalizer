using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.ValueChangeDetectors.Fonts
{
    public class FontsChangeDetector
    {
        public string OldVersionPath { get; init; }
        public string NewVersionPath { get; init; }
        private const string _charPref = "char id=";
        private const string _charSumPref = "chars count=";

        public void ExploreDiff()
        {
            foreach (string newFilePath in Directory.GetFiles(NewVersionPath, "*.fnt", SearchOption.AllDirectories))
            {
                string oldFilePath = newFilePath.Replace(NewVersionPath, OldVersionPath);
                if (File.Exists(oldFilePath))
                {
                    try
                    {
                        int newCheskSum = 0;
                        int oldCheckSum = 0;
                        List<int> newChars = new List<int>(2 ^ 8);
                        List<int> oldChars = new List<int>(2 ^ 8);

                        foreach (var line in File.ReadLines(newFilePath))
                        {
                            if(line.StartsWith(_charPref))
                            {
                                var text = line.Skip(_charPref.Length).SkipWhile(t => !char.IsDigit(t)).TakeWhile(t => char.IsDigit(t)).ToArray();
                                int value = int.Parse(new string(text));
                                newChars.Add(value);
                            }
                            else if (line.StartsWith(_charSumPref))
                            {
                                var text = line.Skip(_charPref.Length).SkipWhile(t => !char.IsDigit(t)).TakeWhile(t => char.IsDigit(t)).ToArray();
                                int value = int.Parse(new string(text));
                                newCheskSum = value;
                            }
                        }

                        foreach (var line in File.ReadLines(oldFilePath))
                        {
                            if (line.StartsWith(_charPref))
                            {
                                var text = line.Skip(_charPref.Length).SkipWhile(t => !char.IsDigit(t)).TakeWhile(t => char.IsDigit(t)).ToArray();
                                int value = int.Parse(new string(text));
                                oldChars.Add(value);
                            }
                            else if (line.StartsWith(_charSumPref))
                            {
                                var text = line.Skip(_charPref.Length).SkipWhile(t => !char.IsDigit(t)).TakeWhile(t => char.IsDigit(t)).ToArray();
                                int value = int.Parse(new string(text));
                                oldCheckSum = value;
                            }
                        }

                        if(oldChars.Count != oldCheckSum || newChars.Count != newCheskSum)
                        {
                            Console.WriteLine($"[CHECKSUM] \"{newFilePath}\"");
                            continue;
                        }

                        List<int> added = newChars.Except(oldChars).ToList();
                        List<int> deleted = oldChars.Except(newChars).ToList();

                        if(added.Count != 0 || deleted.Count != 0)
                        {
                            Console.WriteLine($"[MOD][+{added.Count}-{deleted.Count}] \"{newFilePath}\"");
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"[ERR] \"{newFilePath}\"");
                    }
                }
                else
                {
                    Console.WriteLine($"[NEW] \"{newFilePath}\"");
                }
            }

            foreach (string newFilePath in Directory.GetFiles(OldVersionPath, "*.fnt", SearchOption.AllDirectories))
            {
                string oldFilePath = newFilePath.Replace(OldVersionPath, NewVersionPath);
                if (!File.Exists(oldFilePath))
                {
                    Console.WriteLine($"[DEL] \"{newFilePath}\"");
                }
            }
        }
    }
}
