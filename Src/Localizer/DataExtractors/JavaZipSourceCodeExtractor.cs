using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.DataExtractors
{
    public class JavaZipSourceCodeExtractor : IDisposable
    {
        private ZipArchive _archive;
        public JavaZipSourceCodeExtractor(string pathToZip)
        {
            _archive = ZipFile.OpenRead(pathToZip);
        }

        public List<string> GetAllowedToReplaceByAdvices(string path, List<string> advices)
        {
            List<string> allCode = new List<string>();
            List<string> lines = new List<string>();
            if (advices.Count == 0)
                return lines;

            path = path.Replace('/', '\\');
            var entry = _archive.Entries.SingleOrDefault(t => t.FullName == path);
            if(entry == null)
            {
                Console.WriteLine($"[CLASS NOT DECOMPILED] \"{path}\"");
                return lines;
            }

            using (StreamReader reader = new StreamReader(entry.Open()))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    bool notIgnored = true;
                    foreach (var s in ignoredStartLines)
                        if (line.StartsWith(s) || advices.All(a => !line.Contains(a)))
                        {
                            notIgnored = false;
                            break;
                        }
                    allCode.Add(line);
                    if (notIgnored)
                        lines.Add(line);
                }
            }

            List<string> allowed = advices.Distinct().ToList();

            FilterWordsByAdvices(lines, allowed, advices);
            FilterSensetiveConstantStrings(lines, allowed, advices);
            FilterSpecific(lines, allowed, advices, allCode);

            return allowed;
        }



        private void FilterWordsByAdvices(List<string> code, List<string> allowed, List<string> advices)
        {
            foreach (var line in code)
            {
                foreach (var advice in advices)
                {
                    for (int index = 0; ; index += advice.Length)
                    {
                        index = line.IndexOf(advice, index);
                        if (index == -1)
                            break;
                        
                        bool startCleared = index == 0 || fieldSurround.Contains(line[index-1]);
                        bool endCleared = index + advice.Length == line.Length || fieldSurround.Contains(line[index + advice.Length]);

                        if(startCleared && endCleared)
                        {
                            allowed.Remove(advice);
                            break;
                        }
                    }
                }
            }
        }

        private void FilterSensetiveConstantStrings(List<string> code, List<string> allowed, List<string> advices)
        {
            foreach (var line in code)
            {
                foreach (var advice in advices)
                {
                    foreach (string t in adviceSearchTemplates)
                        if (line.Contains(string.Format(t, advice)))
                        {
                            allowed.Remove(advice);
                            continue;
                        }
                }
            }
        }

        private void FilterSpecific(List<string> code, List<string> allowed, List<string> advices, List<string> allCode)
        {
            foreach (string s in code)
            {
                if (forAllQuotesDetect.Any(t => s.Contains(t, StringComparison.OrdinalIgnoreCase)))
                {
                    foreach (var r in s.Split('"').Where((item, index) => index % 2 != 0))
                        allowed.Remove(r);
                }
                //StarfarerSettings
                else if (s.Contains("StarfarerSettings", StringComparison.OrdinalIgnoreCase))
                {
                    string clear = s[s.IndexOf("StarfarerSettings", StringComparison.OrdinalIgnoreCase)..];
                    foreach (var r in clear.Split('"').Where((item, index) => index % 2 != 0).Take(2))
                        allowed.Remove(r);
                }
            }

            for(int i = 0; i < allCode.Count - 1; i++)
            {
                if (allCode[i] == "    public String getId() {")
                    foreach(var a in advices)
                        if(allCode[i + 1] == $"        return \"{a}\";")
                        {
                            allowed.Remove(a);
                            break;
                        }

            }
        }

        private static List<char> fieldSurround = new List<char>
        {
            '.',
            ',',
            ' ',
            '(',
            ')',
            '<',
            '>',
            '=',
            ';',
            '{',
            '}'

        };

        private static List<string> adviceSearchTemplates = new List<string>
        {
            //".{0}",
            //"{0}.",
            //" {0} =",
            ".contains(\"{0}\"",
            ".equals(\"{0}\"",
            "\"{0}\".toLowerCase().equals(",
            "\"{0}\".toUpperCase().equals(",
            ".endsWith(\"{0}\"",
            ".endsWith(\"{0}\"",
            ".startsWith(\"{0}\"",

            ".getAbility(\"{0}\"", //get set
            ".setSkillLevel(\"{0}\"",

            //com.fs.starfarer.api.SettingsAPI
            ".getBonusXP(\"{0}\")",
            ".getFloat(\"{0}\")",
            ".getBoolean(\"{0}\")",
            ".getBoolean(\"{0}\")",
            ".getColor(\"{0}\")",
            ".getSprite(\"{0}\")",
            ".openStream(\"{0}\")",
            ".loadText(\"{0}\")",
            ".loadJSON(\"{0}\")",
            ".loadCSV(\"{0}\")",
            ".getDescription(\"{0}\"",
            ".getCodeFor(\"{0}\")",
            ".getWeaponSpec(\"{0}\")",
            ".loadTexture(\"{0}\")",
            ".getVariant(\"{0}\")",
            ".getPlugin(\"{0}\")",
            ".getSkillSpec(\"{0}\")",
            ".getString(\"{0}\")",
            ".getAbilitySpec(\"{0}\")",
            ".getTerrainSpec(\"{0}\")",
            ".getEventSpec(\"{0}\")",
            ".getCustomEntitySpec(\"{0}\")",
            ".getDefaultEntriesForRole(\"{0}\")",
            ".getCommoditySpec(\"{0}\")",
            ".getHullSpec(\"{0}\")",
            ".getNewPluginInstance(\"{0}\")",
            ".getControlStringForEnumName(\"{0}\")",
            ".getIndustrySpec(\"{0}\")",
            ".getInt(\"{0}\")",
            ".getSpecialItemSpec(\"{0}\")",
            ".readTextFileFromCommon(\"{0}\")",
            ".fileExistsInCommon(\"{0}\")",
            ".deleteTextFileFromCommon(\"{0}\")",
            ".getDesignTypeColor(\"{0}\")",
            ".doesVariantExist(\"{0}\")",
            ".getJSONObject(\"{0}\")",
            ".getJSONArray(\"{0}\")",
            ".createBaseFaction(\"{0}\")",
            ".getShipSystemSpec(\"{0}\")",
            ".setFloat(\"{0}\"",
            ".setBoolean(\"{0}\"",
            ".getMissionSpec(\"{0}\")",
            ".getBarEventSpec(\"{0}\")",
            ".getFloatFromArray(\"{0}\"",
            ".getIntFromArray(\"{0}\"",
            ".getIntFromArray(\"{0}\")",
            ".getControlDescriptionForEnumName(\"{0}\")",
            ".unloadTexture(\"{0}\")",
            ".forceMipmapsFor(\"{0}\"",

            ".hasHullMod(\"{0}\""
        };

        private static List<string> forAllQuotesDetect = new List<string>
        {
            ".alias",
            //jSONObject
            "jSONObject.",
            ".optDouble",
            ".optInt",
            ".optString",
            ".optBoolean",
            //LoadingUtils
            "LoadingUtils",

            "= new thisnew(",

            //com.fs.starfarer.api.SettingsAPI
            "loadJSON",
            "loadCSV",
            "loadText",
            "getEntriesForRole",
            "addEntryForRole",
            "removeEntryForRole",
            "addDefaultEntryForRole",
            "removeDefaultEntryForRole",
            "getSpec",
            "putSpec",
            "getSpr",
            "writeTextFileToCommon",
        };

        private static List<string> ignoredStartLines = new List<string>
        {
            "import ",
            "package ",
            " *"
        };


        public void Dispose()
        {
            _archive.Dispose();
        }
    }
}
