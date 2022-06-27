using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.DataExtractors
{
    public static class JavaSourceCodeExtractor
    {
        public static List<string> GetStopWords(string path)
        {
            List<string> stopWords = new List<string>();

            if(path.EndsWith(".class"))
                path = path.Substring(0, path.Length - ".class".Length) + ".java";

            if (!File.Exists(path))
                return stopWords;

            foreach (string s in File.ReadAllLines(path))
            {
                if(s.Contains(".alias", StringComparison.OrdinalIgnoreCase))
                {
                    stopWords.AddRange(s.Split('"').Where((item, index) => index % 2 != 0));
                }
            }

            return stopWords;
        }

        public static List<string> GetStopWordsFromAdvices(string path, List<string> advices)
        {
            List<string> stopWords = new List<string>();

            if (path.EndsWith(".class"))
                path = path.Substring(0, path.Length - ".class".Length) + ".java";

            if (!File.Exists(path))
                return stopWords;

            foreach (string s in File.ReadAllLines(path))
            {
                foreach (var a in advices)
                    if (s.Contains($".{a}") && !IsIgnoredLine(s))
                        stopWords.Add(a);
            }

            return stopWords;
        }

        private static bool IsIgnoredLine(string s)
        {
            foreach (var start in ignoredStartLines)
                if (s.StartsWith(start))
                    return true;

            return false;
        }

        private static List<string> ignoredStartLines = new List<string>
        {
            "import com",
            "package com"
        };
    }
}
