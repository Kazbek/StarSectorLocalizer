using System.Collections.Specialized;

namespace StarSectorWebTools.Models.Dictionaries
{
    internal class CsvOrderedDictionary
    {
        public List<string> TranslatedColumns { get; set; }
        public OrderedDictionary Translations { get; set; } = new OrderedDictionary();
    }
}
