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

            foreach (string translationFilePath in Directory.GetFiles(translationPath, "*", SearchOption.AllDirectories))
            {
                string targetFilePath = translationFilePath.Replace(translationPath, string.Empty);
                if (translationFilePath.EndsWith(TranslationFilesNameConventions.TranslationPostfix))
                    targetFilePath = targetFilePath.Replace(TranslationFilesNameConventions.TranslationPostfix, string.Empty);
                targetFilePath = Path.Combine(targetFolder, targetFilePath);

                if (!File.Exists(targetFilePath))
                    throw new FileNotFoundException($"File not found: {targetFilePath}");

                if (translationFilePath.EndsWith(TranslationFilesNameConventions.JarTranslation.PostfixPattern))
                {
                    //int translated = JarGeneralLocalizer.Localize(targetFilePath, JsonToTranslationDictionary.Parse(translationFilePath));
                    //Console.WriteLine($"[{translated}] \"{targetFilePath}\"");
                }
                else if (translationFilePath.EndsWith(TranslationFilesNameConventions.CsvTranslation.PostfixPattern))
                {
                    int translated = CsvGeneralLocalizer.Localize(targetFilePath, JsonToCsvDictionary.Parse(translationFilePath));
                    Console.WriteLine($"[{translated}] \"{targetFilePath}\"");
                }
                else
                {
                    //File.Replace(translationFilePath, targetFilePath, null);
                    //Console.WriteLine($"[REPLACED] \"{targetFilePath}\"");
                }
            }
        }
    }
}