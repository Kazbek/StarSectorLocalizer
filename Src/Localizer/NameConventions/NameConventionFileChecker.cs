using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.NameConventions
{
    public class NameConventionFileChecker
    {
        public readonly string TranslationFolderPath;
        public readonly string TargetFolerPath;

        public NameConventionFileChecker(string translationFolderPath, string targetFolerPath)
        {
            
            TranslationFolderPath = translationFolderPath;
            TargetFolerPath = targetFolerPath;
        }

        public bool TryCheckPatternFileExist(string translationFilePath, out TranslationNameConvention convention, out string targetFilePath)
        {
            convention = TranslationFilesNameConventions.TranslationTypes.SingleOrDefault(t => translationFilePath.EndsWith(t.PostfixPattern));

            if(convention == null)
            {
                convention = TranslationFilesNameConventions.ReplaceFileConvention;

                targetFilePath = translationFilePath.Replace(TranslationFolderPath, null);
                //targetFilePath = targetFilePath.Replace(convention.DeletePattern, string.Empty);
                targetFilePath = Path.Combine(TargetFolerPath, targetFilePath);
            }
            else
            {
                targetFilePath = translationFilePath.Replace(TranslationFolderPath, null);
                targetFilePath = targetFilePath.Replace(convention.DeletePattern, string.Empty);
                targetFilePath = Path.Combine(TargetFolerPath, targetFilePath);
            }

            if (File.Exists(targetFilePath))
                return true;

            Console.WriteLine($"[FILE NOT FOUND]: \"{targetFilePath}\" for \"{translationFilePath}\"");
            targetFilePath = null;
            return false;
        }
    }
}
