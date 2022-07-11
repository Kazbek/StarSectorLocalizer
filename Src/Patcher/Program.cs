using Localizer.Data;
using Localizer.Localizers;
using Localizer.NameConventions;

namespace Patcher
{
    internal class Program
    {
        //Непонятного качества вышел шрифт Orbitron12 (3 файла). Посмотреть позже может есть получше шрифты.
        static void Main(string[] args)
        {

            string translationPath = @"Languages\ru\";
            string targetFolder = @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6 Game\original\Starsector\";

            NameConventionFileChecker conventionFileChecker = new NameConventionFileChecker(translationPath, targetFolder);

            foreach (string translationFilePath in Directory.GetFiles(translationPath, "*", SearchOption.AllDirectories))
            {
                //string targetFilePath = translationFilePath.Replace(translationPath, string.Empty);

                //if (translationFilePath.Contains(TranslationFilesNameConventions.TranslationSuffix))
                //    targetFilePath = targetFilePath.Replace(TranslationFilesNameConventions.TranslationPostfix, string.Empty);
                //targetFilePath = Path.Combine(targetFolder, targetFilePath);

                //if (!File.Exists(targetFilePath))
                //    throw new FileNotFoundException($"File not found: {targetFilePath}");

                if (!conventionFileChecker.TryCheckPatternFileExist(translationFilePath, out TranslationNameConvention convention, out string targetFilePath))
                    continue;

                if(convention == TranslationFilesNameConventions.JarTranslation)
                {
                    int translated = JarGeneralLocalizer.Localize(targetFilePath, JsonToTranslationDictionary.Parse(translationFilePath));
                    Console.WriteLine($"[{translated}] \"{targetFilePath}\"");
                }
                else if (convention == TranslationFilesNameConventions.CsvTranslation)
                {
                    int translated = CsvGeneralLocalizer.Localize(targetFilePath, JsonToCsvDictionary.Parse(translationFilePath));
                    Console.WriteLine($"[{translated}] \"{targetFilePath}\"");
                }
                else if (convention == TranslationFilesNameConventions.TxtTranslation)
                {

                }
                else if (convention == TranslationFilesNameConventions.ReplaceFileConvention)
                {
                    File.Replace(translationFilePath, targetFilePath, null);
                    Console.WriteLine($"[REPLACED] \"{targetFilePath}\"");
                }
                else
                {
                    Console.Write($"UNMATHER CONVENTION: {translationFilePath} - {convention.PostfixPattern}");
                    throw new ArgumentException($"UNMATHER CONVENTION: {translationFilePath} - {convention.PostfixPattern}");
                }
            }
        }
    }
}