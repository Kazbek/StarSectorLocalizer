using Common.Utils;
using Localizer.DataExtractors;
using Localizer.Utils.Json;
using System.Collections.Specialized;
using System.IO.Compression;
using System.Text.Json;

namespace JarDiffExplorer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> filePairs = new Dictionary<string, string>
            {
                {
                    @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\original\starsector-core\starfarer_obf.jar",
                    @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\translated\starsector-core\starfarer_obf.jar"
                },
                {
                    @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\original\starsector-core\starfarer.api.jar",
                    @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\translated\starsector-core\starfarer.api.jar"
                },
            };

            Dictionary<string, string> filePairsRus = new Dictionary<string, string>
            {
                {
                    @"C:\StarSectorPlayground\Starsector v9.1\translated\starsector-core\starfarer_obf.jar",
                    @"C:\StarSectorPlayground\Starsector v9.1\original\starsector-core\starfarer_obf.jar"

                },
                {
                    @"C:\StarSectorPlayground\Starsector v9.1\translated\starsector-core\starfarer.api.jar",
                    @"C:\StarSectorPlayground\Starsector v9.1\original\starsector-core\starfarer.api.jar"
                },
            };

            GenerateTranslationTemplate(
                    @"C:\StarSectorPlayground\Starsector v9.1\translated\starsector-core\starfarer_obf.jar",
                    @"C:\StarSectorPlayground\Starsector v9.1\original\starsector-core\starfarer_obf.jar",
                    @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\file-tranalate\starsector-core\starfarer_obf.jar.translation.rus91.json"
                );

            GenerateTranslationTemplate(
                   @"C:\StarSectorPlayground\Starsector v9.1\translated\starsector-core\starfarer.api.jar",
                   @"C:\StarSectorPlayground\Starsector v9.1\original\starsector-core\starfarer.api.jar",
                   @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\file-tranalate\starsector-core\starfarer.api.jar.translation.rus91.json"
               );

            GenerateTranslationTemplate(
                    @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\original\starsector-core\starfarer_obf.jar",
                    @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\translated\starsector-core\starfarer_obf.jar",
                    @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\file-tranalate\starsector-core\starfarer_obf.jar.translation.json"
                );

            GenerateTranslationTemplate(
                   @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\original\starsector-core\starfarer.api.jar",
                   @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\translated\starsector-core\starfarer.api.jar",
                   @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\file-tranalate\starsector-core\starfarer.api.jar.translation.json"
               );




            return;

            foreach (var filePair in filePairsRus)
            {
                Console.WriteLine(Path.GetFileName(filePair.Key));
                var oldEntries = FindEntriesOrdered(filePair.Key);
                var newEntries = FindEntriesOrdered(filePair.Value);

                foreach(var entry in oldEntries)
                {
                    if (!newEntries.Any(t => t.Text == entry.Text))
                    {
                        Console.WriteLine($"[TRANSLATED][{entry.Count}] \"{entry.Text}\"");
                    }
                }

                List<(string text, int left, int deleted)> partChander = new List<(string text, int left, int deleted)>();
                foreach (var entry in oldEntries)
                {
                    var element = newEntries.FirstOrDefault(t => t.Text == entry.Text);
                    if (element != null && element.Count != entry.Count)
                    {
                        partChander.Add((entry.Text, element.Count, entry.Count - element.Count));
                    }
                }

                foreach(var p in partChander)
                {
                    Console.WriteLine($"[PART][{p.left}][{p.deleted}] \"{p.text}\"");
                }
            }
        }



        public static void GenerateTranslationTemplate(string originalPath, string translatedPath, string templatePath)
        {
            var oldEntries = FindEntriesOrdered(originalPath);
            var newEntries = FindEntriesOrdered(translatedPath);
            OrderedDictionary od = new OrderedDictionary();
            foreach (var entry in oldEntries)
            {
                if (!newEntries.Any(t => t.Text == entry.Text))
                {
                    od.Add(entry.Text, null);
                }
            }
            Directory.CreateDirectory(Path.GetDirectoryName(templatePath));
            File.WriteAllText(templatePath, JsonUtil.Serialize(od));
        }

        private static Dictionary<string, int> FindEntries(string path)
        {
            Dictionary<string, int> entries = new Dictionary<string, int>(2 ^ 10);
            
            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                foreach (ZipArchiveEntry entry in archive.Entries.ToList())
                {
                    if (entry.FullName.EndsWith(".class", StringComparison.OrdinalIgnoreCase))
                    {
                        var javaClassExtractor = new JavaClassExtractor(entry.Open());
                        foreach (var find in javaClassExtractor.GetUtf8Entries())
                        {
                            if (entries.ContainsKey(find))
                            {
                                entries[find]++;
                            }
                            else
                            {
                                entries.Add(find, 1);
                            }
                        }
                    }
                }
            }
            return entries;
        }

        private static List<TextEntry> FindEntriesOrdered(string path)
        {
            List<TextEntry> entries = new List<TextEntry>(2 ^ 10);
            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                foreach (ZipArchiveEntry entry in archive.Entries.ToList())
                {
                    if (entry.FullName.EndsWith(".class", StringComparison.OrdinalIgnoreCase))
                    {
                        var javaClassExtractor = new JavaClassExtractor(entry.Open());
                        foreach (var find in javaClassExtractor.GetUtf8Entries())
                        {
                            var element = entries.FirstOrDefault(t => t.Text == find);

                            if (element == null)
                            {
                                entries.Add(new TextEntry { Text = find, Count = 1});
                            }
                            else
                            {
                                element.Count++;
                            }
                        }
                    }
                }
            }
            return entries;
        }

        public class TextEntry
        {
            public string Text { get; set; }
            public int Count { get; set; }
        }
    }
}