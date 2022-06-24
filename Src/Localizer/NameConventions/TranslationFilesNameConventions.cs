using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.NameConventions
{
    public static class TranslationFilesNameConventions
    {
        public const string TranslationPostfix = ".translation.json";

        public static TranslationNameConvention JarTranslation = new() { PostfixPattern = $".jar{TranslationPostfix}" };
        public static TranslationNameConvention CsvTranslation = new() { PostfixPattern = $".csv{TranslationPostfix}" };

    }

    public struct TranslationNameConvention
    {
        public string PostfixPattern;
    }
}
