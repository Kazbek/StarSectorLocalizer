using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Localizers
{
    public static class TxtGeneralLocalizer
    {
        public static string LocalizationAnchorLine = "\r\n[###StarSectorLocalization###]\r\n";
        public static bool Localize(string targetTxtPath, string localizationTxtPath)
        {
            string originalContent = File.ReadAllText(targetTxtPath);
            string localizationContent = File.ReadAllText(localizationTxtPath);

            int pos = localizationContent.IndexOf(LocalizationAnchorLine);
            if(pos == -1)
            {
                Console.WriteLine($"Corrupted txt translation: {localizationTxtPath}");
                return false;
            }

            if (originalContent.Length != pos || originalContent != localizationContent[0..pos])
            {
                Console.WriteLine($"[TXT UNM][{originalContent.Length}][{pos}] {targetTxtPath}");
                return false;
            }

            File.WriteAllText(targetTxtPath, localizationContent[(pos + LocalizationAnchorLine.Length)..]);
            return true;
        }
    }
}
