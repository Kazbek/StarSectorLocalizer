using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Dictionaries
{
    public class JarDictionary
    {
        public List<string> BlacklistNamespaces { get; set; }
        public GeneralDictionary Translations { get; set; }
    }
}
