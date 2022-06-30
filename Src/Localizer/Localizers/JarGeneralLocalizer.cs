using Localizer.DataExtractors;
using Localizer.Decompilers;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Localizers
{
    public static class JarGeneralLocalizer
    {
        public static int Localize(string zipPath, IDictionary<string, string> dictionary)
        {
            int localizedStrings = 0;
            Console.WriteLine($"Process: {zipPath}");

            string tempDecompiledPath = Path.Combine(Path.GetTempPath(),"Decompiled", Path.GetFileName(zipPath) + ".zip");
            if(Directory.Exists(Path.GetDirectoryName(tempDecompiledPath)))
                ClearFolder(tempDecompiledPath);

            Console.WriteLine(tempDecompiledPath);

            CFRZipDecompilerWrapper.Decompile(zipPath, tempDecompiledPath);

            using var javaZipSourceCodeExtractor = new JavaZipSourceCodeExtractor(tempDecompiledPath);

            using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
            {
                /*Dictionary<string, List<string>> stopWords = new Dictionary<string, List<string>>();
                foreach (ZipArchiveEntry entry in archive.Entries.Where(t => t.FullName.EndsWith(".class", StringComparison.OrdinalIgnoreCase)).ToList())
                {
                    string group = GetClassGroupName(entry.Name);
                    if(!stopWords.ContainsKey(group))
                        stopWords[group] = new List<string>();

                    var stream = entry.Open();
                    byte[] sourceData = ReadFully(stream);
                    var javaClassExtractor = new JavaClassExtractor(sourceData);
                    var utf8commandStrings = javaClassExtractor.GetProgramUtf8Entries();
                    stream.Close();
                    stopWords[group].AddRange(utf8commandStrings);
                }*/

                foreach (ZipArchiveEntry entry in archive.Entries.Where(t => t.FullName.EndsWith(".class", StringComparison.OrdinalIgnoreCase)).ToList())
                {
                    if (entry.FullName.EndsWith(".class", StringComparison.OrdinalIgnoreCase))
                    {
                        var stream = entry.Open();
                        byte[] sourceData = ReadFully(stream);
                        var javaClassExtractor = new JavaClassExtractor(sourceData);

                        string className = entry.FullName;
                        if (className.Contains('$'))
                            className = className[0..className.IndexOf('$')] + ".class";

                        /*var fileStopWords = JavaSourceCodeExtractor.GetStopWords(Path.Combine(tempDecompiledPath, className.Replace('/','\\')));
                        if (fileStopWords == null)
                        {
                            stream.Close();
                            continue;
                        }

                        if (stopWords.TryGetValue(GetClassGroupName(entry.Name), out var sws))
                            fileStopWords.AddRange(sws);*/

                        var utf8Strings = javaClassExtractor.GetUtf8Entries();

                        utf8Strings = javaZipSourceCodeExtractor.GetAllowedToReplaceByAdvices(className, utf8Strings);

                        //fileStopWords = JavaSourceCodeExtractor.GetStopWordsFromAdvices(Path.Combine(tempDecompiledPath, className.Replace('/', '\\')), utf8Strings);
                        //utf8Strings = utf8Strings.Except(fileStopWords).ToList();

                        int modified = 0;
                        foreach(var text in utf8Strings)
                            if (dictionary.TryGetValue(text, out string translate))
                            {
                                Console.WriteLine($"[LOCALIZED] \"{text}\" - \"{translate}\"");
                                var textBytes = Encoding.UTF8.GetBytes(text);
                                var translateBytes = Encoding.UTF8.GetBytes(translate);

                                textBytes = BitConverter.GetBytes(((ushort)textBytes.Length)).Reverse().Concat(textBytes).ToArray();
                                translateBytes = BitConverter.GetBytes(((ushort)translateBytes.Length)).Reverse().Concat(translateBytes).ToArray();

                                /*int pos = FindBytes(sourceData, textBytes);

                                var span = new ReadOnlySpan<byte>(sourceData, pos,2);
                                byte[] reverse = new byte[] { sourceData[pos + 1], sourceData[pos] };
                                ushort len = BitConverter.ToUInt16(reverse);
                                Console.WriteLine($"[LEN MATCH]: {len == textBytes.Length - 2}\"");*/

                                sourceData = ReplaceBytes(sourceData, textBytes, translateBytes);
                                modified++;
                            }


                        localizedStrings += modified;
                        if(modified > 0)
                        {
                            stream.Position = 0;
                            stream.SetLength(0);
                            stream.Write(sourceData, 0, sourceData.Length);
                            stream.Flush();
                        }
                        stream.Close();
                    }
                }
            }

            //ClearFolder(tempDecompiledPath);

            return localizedStrings;
        }

        private static string GetClassGroupName(string name)
        {
            if(!name.Contains('$'))
                return name;
            return name[0..name.IndexOf('$')];
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static int FindBytes(byte[] src, byte[] find, int startIndex = 0)
        {
            int index = -1;
            int matchIndex = 0;
            // handle the complete source array
            for (int i = startIndex; i < src.Length; i++)
            {
                if (src[i] == find[matchIndex])
                {
                    if (matchIndex == (find.Length - 1))
                    {
                        index = i - matchIndex;
                        break;
                    }
                    matchIndex++;
                }
                else if (src[i] == find[0])
                {
                    matchIndex = 1;
                }
                else
                {
                    matchIndex = 0;
                }

            }
            return index;
        }

        public static byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
        {
            byte[] dst = null;
            int index = FindBytes(src, search);
            if (index >= 0)
            {
                dst = new byte[src.Length - search.Length + repl.Length];
                // before found array
                Buffer.BlockCopy(src, 0, dst, 0, index);
                // repl copy
                Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
                // rest of src array
                Buffer.BlockCopy(
                    src,
                    index + search.Length,
                    dst,
                    index + repl.Length,
                    src.Length - (index + search.Length));
            }
            return dst;
        }

        public static void ClearFolder(string path)
        {
            path = Path.GetDirectoryName(path);
            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}
