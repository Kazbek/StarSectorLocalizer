using Localizer.Data;
using Localizer.Localizers;
using Localizer.ValueChangeDetectors.Fonts;
using Playground.DiffCreators;
using Playground.FileTakers;
using Playground.PatchExplorer;
using Playground.UncangedFilesList;

namespace Playground
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*Console.WriteLine("Hello, World!");
            var diffCreator = new FileDiffCreator();
            diffCreator.MakeDiff();
            Console.WriteLine("End.");*/

            /*var patchExplorer = new TwoVersionPatchExplorer {
                NewVersionPath = @"C:\StarSectorPlayground\Starsector 0.95.1a\original",
                OldVersionPath = @"C:\StarSectorPlayground\Starsector v9.1\original",
                TargetFolder = @"C:\StarSectorPlayground\Starsector 0.95.1a\patch"
            };
            patchExplorer.ExploreDiff();*/

            var unchangedFilesTranslation = UnchangedFilesFrom_0_91_To_0_95_1a.GetUnchangedFiles();
            /*Console.WriteLine($"Unchanged files: {unchangedFilesTranslation.Count}");
            foreach (var path in unchangedFilesTranslation)
                Console.WriteLine(path);

            Console.WriteLine();

            var changedFilesTranslation = UnchangedFilesFrom_0_91_To_0_95_1a.GetChangedFiles();
            Console.WriteLine($"Changed files: {changedFilesTranslation.Count}");
            foreach (var path in changedFilesTranslation)
                Console.WriteLine(path);*/

            //var unchangedCopier = new PathFileTaker
            //{
            //    SourceFolder = @"C:\StarSectorPlayground\Starsector v9.1\translation",
            //    DestinationFolder = @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\translation-unchanged"
            //};
            //unchangedCopier.CopyFiles(unchangedFilesTranslation);

            /*
            var fontsChangeDetectorRc6 = new FontsChangeDetector {
                OldVersionPath = @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\original",
                NewVersionPath = @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6 Game\original\Starsector"
                //NewVersionPath = @"C:\StarSectorPlayground\StarSector 0.95.1a-RC6\translated"
            };

            fontsChangeDetectorRc6.ExploreDiff();
            */
            
            //var dictionaty = JsonToTranslationDictionary.Parse("Data/JarDictionary.json");
            //
            //JarGeneralLocalizer.Localize(@"C:\StarSectorPlayground\StarSector 0.95.1a-RC6 Game\original\Starsector\starsector-core\starfarer_obf.jar", dictionaty);
            //JarGeneralLocalizer.Localize(@"C:\StarSectorPlayground\StarSector 0.95.1a-RC6 Game\original\Starsector\starsector-core\starfarer.api.jar", dictionaty);
            

        }

    }
}