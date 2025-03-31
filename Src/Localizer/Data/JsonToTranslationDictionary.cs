using Localizer.Dictionaries;
using System.Text.Json;

namespace Localizer.Data
{
    public static class JsonToTranslationDictionary
    {
        public static GeneralDictionary ParseGeneralDictionary(string path, bool includeNonTranslated = false)
        {
            string json = File.ReadAllText(path);

            var dict = JsonSerializer.Deserialize<GeneralDictionary>(json);
            InitializeGeneralDictionary(dict, path, includeNonTranslated);

            return dict;
        }

        public static JarDictionary ParseJarDictionary(string path, bool includeNonTranslated = false)
        {
            string json = File.ReadAllText(path);

            var dict = JsonSerializer.Deserialize<JarDictionary>(json);
            InitializeGeneralDictionary(dict.Translations, path, includeNonTranslated);

            return dict;
        }

        private static void InitializeGeneralDictionary(GeneralDictionary dict, string path, bool includeNonTranslated)
        {
            if (!includeNonTranslated)
                dict.DeleteNotTranslated();

            if (dict.Any(p => dict.ContainsKey(p.Value)))
                throw new Exception($"{path}\nLooped translation!" + string.Join(", ", dict.Where(p => dict.ContainsKey(p.Value)).Select(t => $"[{t.Key}]=[{t.Value}]")));

            if (!Validate(dict, out string message))
                throw new Exception($"{path}\nProbably corrupted translation!{message}");
        }

        public static bool Validate(Dictionary<string, string> translation, out string message)
        {
            foreach(var pair in translation.Where(t => t.Value != null))
            {
                if (pair.Key == pair.Value)
                {
                    message = $"[Translation same as original]\n=======\n[{pair.Key}]\n=======\n";
                    Console.WriteLine(message);
                    return false;
                }

                foreach (var sc in ServiceCommands)
                {
                    if(CountSubstring(pair.Key, sc) != CountSubstring(pair.Value, sc))
                    {
                        message = $"[SC NOT MATCH][{CountSubstring(pair.Key, sc)}][{CountSubstring(pair.Value, sc)}] \"{pair.Key}\" - \"{pair.Value}\"";
                        Console.WriteLine(message);
                        return false;
                    }
                }
            }
            message = null;
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
