using Localizer.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

            if (!Validate(dict.Translations, out string message))
                throw new Exception("Probablycorrupted translation!" + message);

            return dict;
        }

        public static bool Validate(Dictionary<string, string> translation, out string message)
        {
            foreach (var pair in translation.Where(t => t.Value != null))
            {
                foreach (var sc in ServiceCommands)
                {
                    if (CountSubstring(pair.Key, sc) != CountSubstring(pair.Value, sc))
                    {
                        message = $"[SC NOT MATCH][{CountSubstring(pair.Key, sc)}][{CountSubstring(pair.Value, sc)}] \"{pair.Key}\" - \"{pair.Value}\"";
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
        };
    }
}
