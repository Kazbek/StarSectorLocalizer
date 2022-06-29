using Localizer.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Localizer.Data
{
    public static class JsonToCsvDictionary
    {
        public static CsvDictionary Parse(string path/*, bool includeNonTranslated = false*/)
        {
            string json = File.ReadAllText(path);

            var dict = JsonSerializer.Deserialize<CsvDictionary>(json);
            //if (!includeNonTranslated)
            //    dict.DeleteNotTranslated();

            //if (!Validate(dict))
            //    throw new Exception("Probablycorrupted translation!");

            return dict;
        }
    }
}
