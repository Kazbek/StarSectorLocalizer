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
            try
            {
                return GetStopWordsAuto(path);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public static List<string> GetStopWordsAuto(string path)
        {
            if (path.EndsWith(".class"))
                path = path.Substring(0, path.Length - ".class".Length);

            string testPath = path + ".java";
            if (FileExistsCaseSensitive(testPath))
                return GetStopWordsInternal(testPath);

            string folder = Path.GetDirectoryName(testPath);
            List<string> files = Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly)
                .Where(t =>
                {
                    string fileName = Path.GetFileName(t);
                    if (fileName.Contains("_cfr_"))
                    {
                        string shorted = Path.Combine(folder, fileName.Substring(0, fileName.IndexOf("_cfr_")));
                        return path.StartsWith(shorted);
                    }

                    if (fileName.Contains("_"))
                    {
                        string shorted = Path.Combine(folder, fileName.Substring(0, fileName.IndexOf("_")));
                        return path.StartsWith(shorted);
                    }
                    return false;
                }).ToList();

            if(files.Count > 0)
            {
                List<string> unitedStopWords = new List<string>();
                foreach (var f in files)
                    unitedStopWords.AddRange(GetStopWordsInternal(f));

                return unitedStopWords;
            }

            return null;

        }


        private static List<string> GetStopWordsInternal(string path)
        {
            List<string> stopWords = new List<string>();

            //if(path.EndsWith(".class"))
            //    path = path.Substring(0, path.Length - ".class".Length) + ".java";

            //TODO: Искать по разбивкам файлов и тд, лучше взять объём больше стоп слов (с избытком из других файлов), чем пропустить файл целиком.
            //if (!FileExistsCaseSensitive(path))
            //    return null;

            foreach (string s in File.ReadAllLines(path))
            {
                if(s.Contains(".alias", StringComparison.OrdinalIgnoreCase))
                {
                    stopWords.AddRange(s.Split('"').Where((item, index) => index % 2 != 0));
                }
                //jSONObject
                else if (s.Contains("jSONObject.", StringComparison.OrdinalIgnoreCase))
                {
                    stopWords.AddRange(s.Split('"').Where((item, index) => index % 2 != 0));
                }
                else if (s.Contains(".optDouble", StringComparison.OrdinalIgnoreCase))
                {
                    stopWords.AddRange(s.Split('"').Where((item, index) => index % 2 != 0));
                }
                else if (s.Contains(".optInt", StringComparison.OrdinalIgnoreCase))
                {
                    stopWords.AddRange(s.Split('"').Where((item, index) => index % 2 != 0));
                }
                else if (s.Contains(".optString", StringComparison.OrdinalIgnoreCase))
                {
                    stopWords.AddRange(s.Split('"').Where((item, index) => index % 2 != 0));
                }else if (s.Contains(".optBoolean", StringComparison.OrdinalIgnoreCase))
                {
                    stopWords.AddRange(s.Split('"').Where((item, index) => index % 2 != 0));
                }
                //LoadingUtils
                else if (s.Contains("LoadingUtils", StringComparison.OrdinalIgnoreCase))
                {
                    stopWords.AddRange(s.Split('"').Where((item, index) => index % 2 != 0));
                }
                //StarfarerSettings
                else if (s.Contains("StarfarerSettings", StringComparison.OrdinalIgnoreCase))
                {
                    string clear = s[s.IndexOf("StarfarerSettings", StringComparison.OrdinalIgnoreCase)..];
                    stopWords.AddRange(clear.Split('"').Where((item, index) => index % 2 != 0).Take(2));
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

        public static bool FileExistsCaseSensitive(string filename)
        {
            try
            {
                string name = Path.GetDirectoryName(filename);

                return name != null
                       && Array.Exists(Directory.GetFiles(name), s => s == Path.GetFullPath(filename));
            }
            catch(Exception e)
            {
                return false;
            }
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
