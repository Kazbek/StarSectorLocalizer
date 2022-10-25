using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Dictionaries
{
    public class CsvDictionary
    {
        public List<string> TranslatedColumns { get; set; }
        public Dictionary<string, string> Translations { get; set; }

        public void DeleteNotTranslated()
        {
            List<string> toDeleteKeys = Translations.Where(t => t.Value == null).Select(t => t.Key).ToList();
            foreach (var key in toDeleteKeys)
                Translations.Remove(key);
        }
    }
}
