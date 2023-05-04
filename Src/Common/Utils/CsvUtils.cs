using CsvHelper;
using CsvHelper.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace Common.Utils
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

        public static DataTable Read(Stream stream)
        {
            DataTable dt;
            using (var reader = new StreamReader(stream))
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

        public static Stream SaveToStream(DataTable dataTable)
        {
            MemoryStream ms = new MemoryStream();
            using (var textWriter = new StreamWriter(ms))
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

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
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
