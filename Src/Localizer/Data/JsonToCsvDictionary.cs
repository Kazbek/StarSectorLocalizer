﻿using Localizer.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Localizer.Data
{
    public static class JsonToCsvTranslationDictionary
    {
        public static CsvDictionary Parse(string path, bool includeNonTranslated = false)
        {
            string json = File.ReadAllText(path);

            var dict = JsonSerializer.Deserialize<CsvDictionary>(json);
            if (!includeNonTranslated)
                dict.DeleteNotTranslated();

            if(dict.Translations.Any(p => dict.Translations.ContainsKey(p.Value)))
                throw new Exception($"{path}\nLooped translation!" + string.Join(", ",dict.Translations.Where(p => dict.Translations.ContainsKey(p.Value)).Select(t => $"[{t.Key}]=[{t.Value}]")));;

            if (!Validate(dict.Translations, out string message))
                throw new Exception($"{path}\nProbably corrupted translation!" + message);

            return dict;
        }

        public static bool Validate(Dictionary<string, string> translation, out string message)
        {
            foreach (var pair in translation.Where(t => t.Value != null))
            {
                if(pair.Key == pair.Value)
                {
                    message = $"[Translation same as original]\n=======\n[{pair.Key}]\n=======\n";
                    Console.WriteLine(message);
                    return false;
                }

                foreach (var sc in ServiceCommands)
                {
                    if (CountSubstring(pair.Key, sc) != CountSubstring(pair.Value, sc))
                    {
                        message = $"[SC NOT MATCH][{HttpUtility.JavaScriptStringEncode(sc)}][{CountSubstring(pair.Key, sc)}][{CountSubstring(pair.Value, sc)}]\n=======\n{pair.Key}\n=======\n{pair.Value}\n=======\n";
                        Console.WriteLine(message);
                        return false;
                    }
                }
            }
            message = string.Empty;
            return true;
        }

        public static int CountSubstring(this string text, string value)
        {
            int count = 0, minIndex = text.IndexOf(value, 0);
            while (minIndex != -1)
            {
                minIndex = text.IndexOf(value, minIndex + value.Length);
                count++;
            }
            return count;
        }

        private static readonly List<string> ServiceCommands = new List<string>
        {
            "\t",
            "\n",
            "\r",
        };
    }
}
