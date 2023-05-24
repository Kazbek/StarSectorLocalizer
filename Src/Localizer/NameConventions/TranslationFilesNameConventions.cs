namespace Localizer.NameConventions
{
    public static class TranslationFilesNameConventions
    {
        public const string TranslationSuffix = ".translation";

        public static readonly TranslationNameConvention JarTranslation = new() { PostfixPattern = $".jar{TranslationSuffix}.json", DeletePattern = $"{TranslationSuffix}.json" };
        public static readonly TranslationNameConvention CsvTranslation = new() { PostfixPattern = $".csv{TranslationSuffix}.json", DeletePattern = $"{TranslationSuffix}.json" };
        public static readonly TranslationNameConvention TxtTranslation = new() { PostfixPattern = $".txt{TranslationSuffix}.txt", DeletePattern = $"{TranslationSuffix}.txt" };
        public static readonly TranslationNameConvention JavaTranslation = new() { PostfixPattern = $".java{TranslationSuffix}.txt", DeletePattern = $"{TranslationSuffix}.txt" };
        public static readonly TranslationNameConvention JsonTranslation = new() { PostfixPattern = $".json{TranslationSuffix}.json", DeletePattern = $"{TranslationSuffix}.json" };

        public static readonly TranslationNameConvention ReplaceFileConvention = new();

        public static readonly IReadOnlyList<TranslationNameConvention> TranslationTypes = new List<TranslationNameConvention> { JarTranslation, CsvTranslation, TxtTranslation, JavaTranslation, JsonTranslation };
    }

    public class TranslationNameConvention
    {
        public string PostfixPattern;
        public string DeletePattern;
    }
}
