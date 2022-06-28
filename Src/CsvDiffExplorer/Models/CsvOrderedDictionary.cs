using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDiffExplorer.Models
{
    internal class CsvOrderedDictionary
    {
        public List<string> TranslatedColumns { get; set; }
        public OrderedDictionary Translations { get; set; } = new OrderedDictionary();
    }
}
