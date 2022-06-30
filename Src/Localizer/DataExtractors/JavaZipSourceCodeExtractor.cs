using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.DataExtractors
{
    public class JavaZipSourceCodeExtractor : IDisposable
    {
        private ZipArchive _archive;
        public JavaZipSourceCodeExtractor(string pathToZip)
        {
            _archive = ZipFile.OpenRead(pathToZip);
        }

        public List<string> GetAllowedToReplaceByAdvices(string path, List<string> advices)
        {
            List<string> lines = new List<string>();
            path = path.Replace('/', '\\');
            var entry = _archive.Entries.SingleOrDefault(t => t.FullName == path);
            if(entry == null || advices.Count == 0)
            {
                Console.WriteLine($"[CLASS NOT DECOMPILED] \"{path}\"");
                return lines;
            }

            using (StreamReader reader = new StreamReader(entry.Open()))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    bool notIgnored = true;
                    foreach (var s in ignoredStartLines)
                        if (line.StartsWith(s) || advices.All(a => !s.Contains(a)))
                        {
                            notIgnored = false;
                            break;
                        }
                    if (notIgnored)
                        lines.Add(line);
                }
            }

            List<string> allowed = advices.Distinct().ToList();

            FilterWordsByAdvices(lines, allowed, advices);
            FilterSensetiveConstantStrings(lines, allowed, advices);
            FilterSpecific(lines, allowed, advices);

            return allowed;
        }



        private void FilterWordsByAdvices(List<string> code, List<string> allowed, List<string> advices)
        {
            foreach (var line in code)
            {
                foreach (var advice in advices)
                {
                    for (int index = 0; ; index += advice.Length)
                    {
                        index = line.IndexOf(advice, index);
                        if (index == -1)
                            break;
                        
                        bool startCleared = index == 0 || fieldSurround.Contains(line[index-1]);
                        bool endCleared = index + advice.Length == line.Length || fieldSurround.Contains(line[index + advice.Length]);

                        if(startCleared && endCleared)
                        {
                            allowed.Remove(advice);
                            break;
                        }
                    }
                }
            }
        }

        private void FilterSensetiveConstantStrings(List<string> code, List<string> allowed, List<string> advices)
        {
            foreach (var line in code)
            {
                foreach (var advice in advices)
                {
                    foreach (string t in adviceSearchTemplates)
                        if (line.Contains(string.Format(t, advice)))
                        {
                            allowed.Remove(advice);
                            continue;
                        }
                }
            }
        }

        private void FilterSpecific(List<string> code, List<string> allowed, List<string> advices)
        {
            foreach (string s in code)
            {
                if (forAllQuotesDetect.Any(t => s.Contains(t, StringComparison.OrdinalIgnoreCase)))
                {
                    foreach (var r in s.Split('"').Where((item, index) => index % 2 != 0))
                        allowed.Remove(r);
                }
                //StarfarerSettings
                else if (s.Contains("StarfarerSettings", StringComparison.OrdinalIgnoreCase))
                {
                    string clear = s[s.IndexOf("StarfarerSettings", StringComparison.OrdinalIgnoreCase)..];
                    foreach (var r in clear.Split('"').Where((item, index) => index % 2 != 0).Take(2))
                        allowed.Remove(r);
                }
            }
        }

        private static List<char> fieldSurround = new List<char>
        {
            '.',
            ',',
            ' ',
            '(',
            ')',
            '<',
            '>',
            '=',
            ';',
            '{',
            '}'

        };

        private static List<string> adviceSearchTemplates = new List<string>
        {
            //".{0}",
            //"{0}.",
            //" {0} =",
            ".contains(\"{0}\"",
            ".equals(\"{0}\"",
            "\"{0}\".toLowerCase().equals(",
            "\"{0}\".toUpperCase().equals(",
            ".endsWith(\"{0}\"",
            ".endsWith(\"{0}\"",
            ".startsWith(\"{0}\"",

            ".getAbility(\"{0}\"", //get set
            ".setSkillLevel(\"{0}\"",

            ".hasHullMod(\"{0}\""
        };

        private static List<string> forAllQuotesDetect = new List<string>
        {
            ".alias",
            //jSONObject
            "jSONObject.",
            ".optDouble",
            ".optInt",
            ".optString",
            ".optBoolean",
            //LoadingUtils
            "LoadingUtils"
        };

        private static List<string> ignoredStartLines = new List<string>
        {
            "import ",
            "package ",
            " *"
        };


        public void Dispose()
        {
            _archive.Dispose();
        }
    }
}
