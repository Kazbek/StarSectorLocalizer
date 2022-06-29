using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Utils.Json
{
    public static class XlsxUtils
    {
        public static void SaveNotTranslated(OrderedDictionary orderedDictionary, string path)
        {
            //MemoryStream ms = new MemoryStream();
            using (SLDocument sl = new SLDocument())
            {
                int row = 1;
                foreach(var key in orderedDictionary.Keys)
                {
                    if (orderedDictionary[key] == null)
                    {
                        sl.SetCellValue(row++, 1, (string)key);
                    }
                }
                
                //sl.SaveAs(ms);
                sl.SaveAs(path);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            //ms.Position = 0;

            //using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            //    ms.CopyTo(file);

        }

        public static Dictionary<string, string> ReadDictionary(string path){
            Dictionary<string, string> dict = new Dictionary<string, string>();

            using (SLDocument sl = new SLDocument(path))
            {
                for(int i = 1; ; i++)
                {
                    var key = sl.GetCellValueAsString(i, 1);
                    if (string.IsNullOrWhiteSpace(key))
                        break;
                    var value = sl.GetCellValueAsString(i, 2);
                    dict.Add(key, value);
                }
            }

            return dict;
        }
    }
}
