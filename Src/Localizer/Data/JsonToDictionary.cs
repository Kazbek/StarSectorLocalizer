using Localizer.Dictionaries;
using System.Text.Json;

namespace Localizer.Data
{
    public static class JsonToDictionary
    {
        public static GeneralDictionary Parse(string path)
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<GeneralDictionary>(json);
        }
    }
}
