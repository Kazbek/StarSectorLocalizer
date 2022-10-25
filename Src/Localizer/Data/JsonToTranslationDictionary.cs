using Localizer.Dictionaries;
using System.Text.Json;

namespace Localizer.Data
{
    public static class JsonToTranslationDictionary
    {
        public static GeneralDictionary Parse(string path, bool includeNonTranslated = false)
        {
            string json = File.ReadAllText(path);

            var dict = JsonSerializer.Deserialize<GeneralDictionary>(json);
            if (!includeNonTranslated)
                dict.DeleteNotTranslated();

            if (!Validate(dict))
                throw new Exception("Probablycorrupted translation!");

            return dict;
        }

        public static bool Validate(Dictionary<string, string> translation)
        {
            foreach(var pair in translation.Where(t => t.Value != null))
            {
                foreach(var sc in ServiceCommands)
                {
                    if(CountSubstring(pair.Key, sc) != CountSubstring(pair.Value, sc))
                    {
                        Console.WriteLine($"[SC NOT MATCH][{CountSubstring(pair.Key, sc)}][{CountSubstring(pair.Value, sc)}] \"{pair.Key}\" - \"{pair.Value}\"");
                        return false;
                    }
                }
            }

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
            "\"",
            "%s",
            "%d"
        };
    }
}
