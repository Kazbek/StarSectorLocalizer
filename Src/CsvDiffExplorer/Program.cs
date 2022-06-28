using CsvDiffExplorer.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Localizer.DataExtractors;
using Localizer.Utils.Json;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO.Compression;
using System.Text.Json;

namespace CsvDiffExplorer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<(string Orig91, string Trans91, string Orig95, string TemplatePath)> fileTuples = new List<(string Orig91, string Trans91, string Orig95, string TemplatePath)>()
            {

            };

            foreach (var tuple in fileTuples)
            {
                
            }
            var dtOriginal = CsvUtils.Read(@"C:\StarSectorPlayground\Starsector v9.1\original\starsector-core\data\strings\descriptions.csv");
            var dtTranslated = CsvUtils.Read(@"C:\StarSectorPlayground\Starsector v9.1\translation\starsector-core\data\strings\descriptions.csv");

            if (dtOriginal.Rows.Count != dtTranslated.Rows.Count)
                throw new Exception();

            if (dtOriginal.Columns.Count != dtTranslated.Columns.Count)
                throw new Exception();

            int equal = 0;
            int diff = 0;

            CsvOrderedDictionary dict = new CsvOrderedDictionary() {
                TranslatedColumns = new List<string> { "text1", "text2", "text3", "notes" }
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
                        }

                        if (!dict.Translations.Contains(key))
                        {
                            dict.Translations.Add(key, value);
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
            string templatePath = @"C:\StarSectorPlayground\descriptions.csv.translation.json";
            Directory.CreateDirectory(Path.GetDirectoryName(templatePath));
            File.WriteAllText(templatePath, JsonUtil.Serialize(dict));
            Console.WriteLine($"{ equal} -- {diff}");
            var z = 1;
        }

        static void EXPORT_CSV(DataTable CHAIN)
        {
            using (var textWriter = File.CreateText(@"C:\StarSectorPlayground\descriptions.csv"))
            using (var csv = new CsvWriter(textWriter, new CsvConfiguration(CultureInfo.InvariantCulture) { Encoding = System.Text.Encoding.UTF8 }))
            {
                // Write columns
                foreach (DataColumn column in CHAIN.Columns)
                {
                    csv.WriteField(column.ColumnName);
                }
                csv.NextRecord();

                // Write row values
                foreach (DataRow row in CHAIN.Rows)
                {
                    for (var i = 0; i < CHAIN.Columns.Count; i++)
                    {
                        csv.WriteField(row[i]);
                    }
                    csv.NextRecord();
                }
            }
        }
    }
}