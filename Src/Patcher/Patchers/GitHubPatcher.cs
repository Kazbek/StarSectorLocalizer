using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Patcher.Patchers
{
    public class GitHubPatcher
    {
        private readonly string _gitBasePath = @"Translation/Languages/ru/";


        private string GetFolderUrl( string path)
        {
            return $"https://api.github.com/repos/Kazbek/StarSectorLocalizer/contents/{path}?ref=main";
        }

        public async Task UpdateLocalCacheAsync(string baseTranslationFilesFolder, CancellationToken cancellationToken = default)
        {
            HttpClient client = new HttpClient();
            List<GitHubContent> gitHubContents = await client.GetFromJsonAsync<List<GitHubContent>>(GetFolderUrl(_gitBasePath), cancellationToken);

        }

        private async Task UpdateLocalCacheAsync(List<GitHubContent> contents, string translationFilesFolder, CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(translationFilesFolder);

            //delete unneccessary files
            List<string> files = contents.Where(t => t.IsFile()).Select(t => t.Name).ToList();
            foreach (string checkFile in Directory.GetFiles(translationFilesFolder, "*", SearchOption.TopDirectoryOnly).ToList())
            {
                if (!files.Contains(Path.GetFileName(checkFile)))
                    File.Delete(checkFile);
            }

            foreach (GitHubContent content in contents)
            {
                if (content.IsFile())
                {
                    string targetPath = Path.Combine(translationFilesFolder, content.Name);
                    if (File.Exists(targetPath))
                    {
                        
                        
                    }
                    else
                    {
                        //do
                    }
                }
                else if (content.IsDir())
                {
                    
                }
                throw new NotImplementedException("Не задано действия для типа контента: " + content?.Type);
            }

        }

        private string SHA256CheckSum(string filePath)
        {
            using (SHA256 SHA256 = SHA256Managed.Create())
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                    return Convert.ToBase64String(SHA256.ComputeHash(fileStream));
            }
        }

        static string Hash(string input)
        {
            using var sha1 = SHA1.Create();
            return Convert.ToHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }

        private class GitHubContent
        {
            public string Name { get; set; }

            public string Path { get; set; }

            public string Sha { get; set; }

            public long Size { get; set; }
            public Uri Url { get; set; }

            public Uri HtmlUrl { get; set; }

            public Uri GitUrl { get; set; }

            public Uri DownloadUrl { get; set; }

            public string Type { get; set; }

            public GitHubLinks Links { get; set; }

            public bool IsDir() => Type == "dir";
            public bool IsFile() => Type == "file";
        }

        private class GitHubLinks
        {
            public Uri Self { get; set; }

            public Uri Git { get; set; }

            public Uri Html { get; set; }
        }
    }
}
