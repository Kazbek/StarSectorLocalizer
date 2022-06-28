using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Utils.Json
{
    public class CsvUtils
    {
        public static DataTable Read(string path)
        {
            DataTable dt;
            using (var reader = new StreamReader(path/*,System.Text.Encoding.ASCII*/))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Do any configuration to `CsvReader` before creating CsvDataReader.
                using (var dr = new CsvDataReader(csv))
                {
                    dt = new DataTable();
                    dt.Load(dr);
                }
            }
            return dt;
        }
        public static void Save(DataTable dataTable, string path)
        {
            using (var textWriter = File.CreateText(path))
            using (var csv = new CsvWriter(textWriter, new CsvConfiguration(CultureInfo.InvariantCulture) { Encoding = Encoding.UTF8 }))
            {
                // Write columns
                foreach (DataColumn column in dataTable.Columns)
                {
                    csv.WriteField(column.ColumnName);
                }
                csv.NextRecord();

                // Write row values
                foreach (DataRow row in dataTable.Rows)
                {
                    for (var i = 0; i < dataTable.Columns.Count; i++)
                    {
                        csv.WriteField(row[i]);
                    }
                    csv.NextRecord();
                }
            }
        }
    }
}
