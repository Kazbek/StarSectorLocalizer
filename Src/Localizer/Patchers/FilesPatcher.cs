using Localizer.Data;
using Localizer.Localizers;
using Localizer.NameConventions;

namespace Localizer.Patchers
{
    public class FilesPatcher
    {
        public IProgress<string> ProgressLogger { get; set; }
        public string DecompilerPath { get; set; }
        // TODO: To crate patch
        //string patchFolder = $"D:\\StarSectorPlayground\\Translation Patch\\{DateTime.Now.ToString("d")}";
        public void Patch(string gameFolder, string translationFolder, bool processJar = true, string patchFolder = null)
        {
            //string translationFolder = @"Languages\ru\";
            //string gameFolder = @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6 Game\original\Starsector\";

            bool createPatch = patchFolder != null;

            NameConventionFileChecker conventionFileChecker = new NameConventionFileChecker(translationFolder, gameFolder);

            int replaced = 0;
            foreach (string translationFilePath in Directory.GetFiles(translationFolder, "*", SearchOption.AllDirectories))
            {

                //string targetFilePath = translationFilePath.Replace(translationPath, string.Empty);

                //if (translationFilePath.Contains(TranslationFilesNameConventions.TranslationSuffix))
                //    targetFilePath = targetFilePath.Replace(TranslationFilesNameConventions.TranslationPostfix, string.Empty);
                //targetFilePath = Path.Combine(targetFolder, targetFilePath);

                //if (!File.Exists(targetFilePath))
                //    throw new FileNotFoundException($"File not found: {targetFilePath}");

                if (!conventionFileChecker.TryCheckPatternFileExist(translationFilePath, out TranslationNameConvention convention, out string targetFilePath))
                    continue;

                if (convention == TranslationFilesNameConventions.JarTranslation)
                {
                    if (!processJar)
                        continue;

                    int translated = JarGeneralLocalizer.Localize(targetFilePath, JsonToTranslationDictionary.Parse(translationFilePath), DecompilerPath);
                    ProgressLogger.Report($"[{translated}] \"{targetFilePath}\"");
                }
                else if (convention == TranslationFilesNameConventions.CsvTranslation)
                {
                    int translated = CsvGeneralLocalizer.Localize(targetFilePath, JsonToCsvTranslationDictionary.Parse(translationFilePath));
                    ProgressLogger.Report($"[{translated}] \"{targetFilePath}\"");
                }
                else if (convention == TranslationFilesNameConventions.TxtTranslation)
                {
                    bool translated = TxtGeneralLocalizer.Localize(targetFilePath, translationFilePath);
                    ProgressLogger.Report($"[TXT][{translated}] \"{targetFilePath}\"");
                }
                else if (convention == TranslationFilesNameConventions.JsonTranslation)
                {
                    int translated = JsonGeneralLocalizer.Localize(targetFilePath, JsonToTranslationDictionary.Parse(translationFilePath));
                    ProgressLogger.Report($"[JSON][{translated}] \"{targetFilePath}\"");
                }
                else if (convention == TranslationFilesNameConventions.ReplaceFileConvention)
                {
                    File.Copy(translationFilePath, targetFilePath, true);
                    replaced++;
                    //ProgressLogger.Report($"[REPLACED] \"{targetFilePath}\"");
                }
                else
                {
                    Console.Write($"UNMATCHED CONVENTION: {translationFilePath} - {convention.PostfixPattern}");
                    throw new ArgumentException($"UNMATCHED CONVENTION: {translationFilePath} - {convention.PostfixPattern}");
                }

                if (createPatch) //TODO: To create patch
                {
                    string relativePath = targetFilePath.Replace(gameFolder, string.Empty);
                    string absolutePatchPath = patchFolder + relativePath;

                    Directory.CreateDirectory(Path.GetDirectoryName(absolutePatchPath));
                    File.Copy(targetFilePath, absolutePatchPath, true);
                }
            }

            ProgressLogger.Report($"[REPLACED FILES][{replaced}]");
        }
    }
}
