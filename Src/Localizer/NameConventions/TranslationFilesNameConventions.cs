using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.NameConventions
{
    public static class TranslationFilesNameConventions
    {
        public const string TranslationSuffix = ".translation";

        public static readonly TranslationNameConvention JarTranslation = new() { PostfixPattern = $".jar{TranslationSuffix}.json", DeletePattern = $"{TranslationSuffix}.json" };
        public static readonly TranslationNameConvention CsvTranslation = new() { PostfixPattern = $".csv{TranslationSuffix}.json", DeletePattern = $"{TranslationSuffix}.json" };
        public static readonly TranslationNameConvention TxtTranslation = new() { PostfixPattern = $".csv{TranslationSuffix}.txt", DeletePattern = $"{TranslationSuffix}.txt" };

        public static readonly TranslationNameConvention ReplaceFileConvention = new();

        public static readonly IReadOnlyList<TranslationNameConvention> TranslationTypes = new List<TranslationNameConvention> { JarTranslation, CsvTranslation, TxtTranslation };
    }

    public class TranslationNameConvention
    {
        public string PostfixPattern;
        public string DeletePattern;
    }
}
