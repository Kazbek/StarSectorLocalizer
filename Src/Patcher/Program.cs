using Localizer.Data;
using Localizer.Localizers;
using Localizer.NameConventions;
using Localizer.Patchers;

namespace Patcher
{
    internal class Program
    {
        //Непонятного качества вышел шрифт Orbitron12 (3 файла). Посмотреть позже может есть получше шрифты.

        static void Main(string[] args)
        {
            string translationPath = @"Languages\ru\";
            string targetFolder = @"C:\StarSectorPlayground\StarSector-0.98a-RC5-Work\";
            string patchFolder = $"C:\\StarSectorPlayground\\Translation Patch\\{DateTime.Now.ToString("d")}\\";

            FilesPatcher patcher = new FilesPatcher
            {
                DecompilerPath = Path.Combine(Directory.GetCurrentDirectory(), "Decompilers\\CFRZip-0.152.jar"),
                ProgressLogger = new Progress<string>(Console.WriteLine),
            };

            patcher.Patch(targetFolder, translationPath, true, patchFolder);
        }
        static void Main2(string[] args)
        {
            string translationPath = @"Languages\ru\";
            string targetFolder = @"C:\StarSectorPlayground\StarSector-0.98a-RC5-Work\";
            string patchFolder = $"C:\\StarSectorPlayground\\Translation Patch\\{DateTime.Now.ToString("d")}\\";

            bool processJar = true;
            bool createPatch = true;

            NameConventionFileChecker conventionFileChecker = new NameConventionFileChecker(translationPath, targetFolder);

            int replaced = 0;
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
                    if (!processJar)
                        continue;

                    int translated = JarGeneralLocalizer.Localize(targetFilePath, JsonToTranslationDictionary.ParseJarDictionary(translationFilePath), Path.GetFullPath("Decompilers\\CFRZip-0.152.jar"));
                    Console.WriteLine($"[{translated}] \"{targetFilePath}\"");
                }
                else if (convention == TranslationFilesNameConventions.CsvTranslation)
                {
                    int translated = CsvGeneralLocalizer.Localize(targetFilePath, JsonToCsvTranslationDictionary.Parse(translationFilePath));
                    Console.WriteLine($"[{translated}] \"{targetFilePath}\"");
                }
                else if (convention == TranslationFilesNameConventions.TxtTranslation)
                {
                    bool translated = TxtGeneralLocalizer.Localize(targetFilePath, translationFilePath);
                    Console.WriteLine($"[TXT][{translated}] \"{targetFilePath}\"");
                }
                else if (convention == TranslationFilesNameConventions.JsonTranslation)
                {
                    int translated = JsonGeneralLocalizer.Localize(targetFilePath, JsonToTranslationDictionary.ParseGeneralDictionary(translationFilePath));
                    Console.WriteLine($"[JSON][{translated}] \"{targetFilePath}\"");
                }
                else if (convention == TranslationFilesNameConventions.ReplaceFileConvention)
                {
                    File.Copy(translationFilePath, targetFilePath, true);
                    replaced++;
                    //Console.WriteLine($"[REPLACED] \"{targetFilePath}\"");
                }
                else
                {
                    Console.Write($"UNMATHER CONVENTION: {translationFilePath} - {convention.PostfixPattern}");
                    throw new ArgumentException($"UNMATHER CONVENTION: {translationFilePath} - {convention.PostfixPattern}");
                }

                if (createPatch)
                {
                    string relativePath = targetFilePath.Replace(targetFolder, string.Empty);
                    string absolutePatchPath = Path.Combine(patchFolder, relativePath);

                    Directory.CreateDirectory(Path.GetDirectoryName(absolutePatchPath));
                    File.Copy(targetFilePath, absolutePatchPath, true);
                }
            }

            Console.WriteLine($"[REPLACED FILES][{replaced}]");
        }
    }
}