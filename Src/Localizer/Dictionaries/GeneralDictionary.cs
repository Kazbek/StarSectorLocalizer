using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Dictionaries
{
    public class GeneralDictionary : Dictionary<string, string>
    {
        public void DeleteNotTranslated()
        {
            List<string> toDeleteKeys = this.Where(t => t.Value == null).Select(t => t.Key).ToList();
            foreach(var key in toDeleteKeys)
                this.Remove(key);
        }
    }
}
