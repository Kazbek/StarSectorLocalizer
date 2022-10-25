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
            //FileSpecifics.data.strings.descriptions.Process();
            FileSpecifics.data.campaign.rules.Process();
        }
    }
}