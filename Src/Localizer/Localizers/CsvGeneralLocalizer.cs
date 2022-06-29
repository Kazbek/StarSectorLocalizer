using Localizer.Dictionaries;
using Localizer.Utils.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Localizers
{
    public static class CsvGeneralLocalizer
    {
        public static int Localize(string csvPath, CsvDictionary dictionary)
        {
            int translationCount = 0;
            DataTable dataTable = CsvUtils.Read(csvPath);
            List<int> cols = new List<int>();
            {
                List<string> notFound = dictionary.TranslatedColumns.ToList();
                foreach(var s in dictionary.TranslatedColumns)
                {
                    for(int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        if (dataTable.Columns[i].ColumnName == s)
                        {
                            dataTable.Columns[i].ReadOnly = false;
                            cols.Add(i);
                            notFound.Remove(s);
                        }
                    }
                }

                if (notFound.Any())
                {
                    Console.WriteLine($"[CSV][COL NOT FOUND][{string.Join(", ", notFound)}] \"{csvPath}\"");
                    throw new Exception($"[CSV][COL NOT FOUND][{string.Join(", ", notFound)}] \"{csvPath}\"");
                }
            }

            for(int i = 1; i < dataTable.Rows.Count; i++)
            {
                foreach(var j in cols)
                {
                    string key = dataTable.Rows[i][j]?.ToString();
                    if (!string.IsNullOrWhiteSpace(key) && dictionary.Translations.TryGetValue(key, out string translation) && translation != null)
                    {
                        dataTable.Rows[i][j] = translation;
                        translationCount++;
                    }
                    
                }
            }

            CsvUtils.Save(dataTable, csvPath);

            return translationCount;
        }
    }
}
