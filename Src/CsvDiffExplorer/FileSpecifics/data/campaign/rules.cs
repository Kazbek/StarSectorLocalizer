using Common.Utils;
using CsvDiffExplorer.Models;
using Localizer.Utils.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDiffExplorer.FileSpecifics.data.campaign
{
    internal class rules
    {
        public static void Process()
        {
            var dtOriginal = CsvUtils.Read(@"D:\Games\Starsector\starsector-core_backup\data\campaign\rules.csv");
            var dtTranslated = CsvUtils.Read(@"D:\Games\Starsector\starsector-core\data\campaign\rules.csv");

            if (dtOriginal.Rows.Count != dtTranslated.Rows.Count)
                throw new Exception();

            if (dtOriginal.Columns.Count != dtTranslated.Columns.Count)
                throw new Exception();

            int equal = 0;
            int diff = 0;
            int translatedOld = 0;
            int translatedNew = 0;

            CsvOrderedDictionary dict = new CsvOrderedDictionary()
            {
                TranslatedColumns = new List<string> { "text", "options" }
            };

            CsvOrderedDictionary dictExtended = new CsvOrderedDictionary()
            {
                TranslatedColumns = new List<string> { "text", "options" }
            };

            for (int i = 0; i < dtOriginal.Rows.Count; i++)
            {
                if (dtOriginal.Rows[i][0].ToString() != dtTranslated.Rows[i][0].ToString())
                {
                    throw new Exception();
                }

                if (dtOriginal.Rows[i][1].ToString() != dtTranslated.Rows[i][1].ToString())
                {
                    throw new Exception();
                }

                for (int j = 2; j < dtOriginal.Columns.Count; j++)
                {
                    if (!string.IsNullOrWhiteSpace(dtOriginal.Rows[i][j]?.ToString()))
                    {
                        string key = dtOriginal.Rows[i][j].ToString();
                        string value = null;

                        if (dtTranslated.Rows[i][j].ToString() != key)
                        {
                            value = dtTranslated.Rows[i][j].ToString();
                            translatedOld++;

                            if (!dict.Translations.Contains(key))
                            {
                                dict.Translations.Add(key, value);
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(dtOriginal.Rows[i][j].ToString()) != string.IsNullOrWhiteSpace(dtTranslated.Rows[i][j].ToString()))
                    {
                        Console.WriteLine($"[{i}][{j}][{dtOriginal.Rows[i][j].ToString()}][{dtTranslated.Rows[i][j].ToString()}]");
                        var zzz = 1;
                    }

                    if (dtOriginal.Rows[i][j].ToString() == dtTranslated.Rows[i][j].ToString())
                    {
                        equal++;

                    }
                    else
                    {
                        diff++;
                    }
                }
            }

            var dtNewVersion = CsvUtils.Read(@"D:\Games\Starsector\starsector-core_backup\data\campaign\rules.csv");
            for (int i = 0; i < dtNewVersion.Rows.Count; i++)
            {
                for (int j = 2; j < dtNewVersion.Columns.Count; j++)
                {
                    if (!string.IsNullOrWhiteSpace(dtNewVersion.Rows[i][j]?.ToString()))
                    {
                        string key = dtNewVersion.Rows[i][j].ToString();
                        string value = null;

                        if (dict.Translations.Contains(key))
                        {
                            value = (string)dict.Translations[key];
                            translatedNew++;
                        }

                        if (!dictExtended.Translations.Contains(key))
                        {
                            dictExtended.Translations.Add(key, value);
                        }
                    }
                }
            }

            //XlsxUtils.SaveNotTranslated(dictExtended.Translations, @"C:\StarSectorPlayground\descriptions.csv.nottranslated.xlsx");
            /*
            var gTranslate = XlsxUtils.ReadDictionary(@"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\GoogleDocsTranslateHelpers\starsector-core\data\strings\descriptions.csv.translated.xlsx");
            int match = 0, miss = 0;
            foreach(var g in gTranslate)
            {
                //string key = Regex.Unescape(g.Key);
                string key = g.Key;
                string value = g.Value;

                if (key.Contains('\n') && !key.Contains('\r'))
                    key = key.Replace("\n", "\r\n");
                if (value.Contains('\n') && !value.Contains('\r'))
                    value = value.Replace("\n", "\r\n");

                if (dictExtended.Translations.Contains(key))
                {
                    match++;
                    if (dictExtended.Translations[key] != null)
                        throw new Exception("Already translated");
                    dictExtended.Translations[key] = g.Value;
                }
                else
                {
                    string test = dictExtended.Translations.Keys.Cast<string>().FirstOrDefault(t => t?.Contains("A small caliber autocannon") ?? false);
                    miss++;
                }
            }
            Console.WriteLine($"Google xlsx:match: {match}   miss: {miss}");
            */
            string templatePath = @"D:\StarSectorPlayground\rules.csv.translation.json";
            Directory.CreateDirectory(Path.GetDirectoryName(templatePath));
            File.WriteAllText(templatePath, JsonUtil.Serialize(dictExtended));
            Console.WriteLine($"{equal} -- {diff}");
            Console.WriteLine($"[Old translated-New translated]{translatedOld} -- {translatedNew}");

        }
    }
}
