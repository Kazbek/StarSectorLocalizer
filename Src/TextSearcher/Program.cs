using Localizer.DataExtractors;
using System.IO.Compression;

namespace TextSearcher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> jarFiles = new List<string>
            {
                //@"D:\Games\Starsector\starsector-core\starfarer_obf.jar",
                @"C:\StarSectorPlayground\StarSector-0.98a-RC5-Work\starsector-core\starfarer.api.jar"
            };

            string text = null;
            while (string.IsNullOrWhiteSpace(text))
            {
                Console.Write("Введите текст для поиска:");
                text = Console.ReadLine();
            }
            Console.WriteLine($"Выполняется поиск: \"{text}\"");

            foreach(var zipPath in jarFiles)
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries.ToList())
                    {
                        if (entry.FullName.EndsWith(".class", StringComparison.OrdinalIgnoreCase))
                        {
                            var javaClassExtractor = new JavaClassExtractor(entry.Open());
                            foreach(var find in javaClassExtractor.GetUtf8Entries().Where(t => t.Equals(text, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                Console.WriteLine(find + ":" + entry.FullName);
                            }
                        }
                    }
                }
            }
        }
    }
}