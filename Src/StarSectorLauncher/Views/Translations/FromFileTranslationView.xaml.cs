using Localizer.Patchers;
using Localizer.Validators;

namespace StarSectorLauncher.Views.Translations;

public partial class FromFileTranslationView : ContentView
{
	public FromFileTranslationView()
	{
		InitializeComponent();

        GamePathEntry.Text = Preferences.Default.Get("GAME_PATH", string.Empty);
        LocalizationPathEntry.Text = Preferences.Default.Get("LOCALIZATION_PATH", string.Empty);
        PatchPathEntry.Text = Preferences.Default.Get("PATCH_PATH", string.Empty);
        ProcessJarCheckBox.IsChecked = Preferences.Default.Get("PROCESS_JAR", true);
        ProcessJarCheckBox.IsChecked = Preferences.Default.Get("CREATE_PATCH", false);
	}

    void WriteLog(string text)
    {
        LogLabel.Text = $"{DateTime.Now.ToString("T")}: {text}\n{LogLabel.Text}";
    }

    void ClearLog() { LogLabel.Text = string.Empty; }
    private char[] _trims = new char[] { ' ', '\t', '\n', '\t' };
    async void OnStartTranslateButtonClicked(object sender, EventArgs args)
    {
        Preferences.Default.Set("PROCESS_JAR", ProcessJarCheckBox.IsChecked);

        ClearLog();
        await Permissions.RequestAsync<Permissions.StorageWrite>();
        await Permissions.RequestAsync<Permissions.StorageRead>();

        //var path = Directory.GetCurrentDirectory();

        string decompilerPath = Path.Combine(Directory.GetCurrentDirectory(), "Decompilers\\CFRZip-0.152.jar");
        /*string decompilerPath = Path.Combine(FileSystem.Current.AppDataDirectory, "CFRZip-0.152.jar");
        using (var fileStream = File.Create(decompilerPath))
        {
            var stream = await FileSystem.Current.OpenAppPackageFileAsync("Decompilers\\CFRZip-0.152.jar");
            //myOtherObject.InputStream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fileStream);
        }*/

        string gamePath = GamePathEntry.Text.Trim(_trims);
        WriteLog($"Game path: {gamePath}");
        if (!GamePathValidator.IsValid(gamePath))
        {
            WriteLog($"Неверный путь к папке игры: \"{gamePath}\"");
            return;
        }

        Preferences.Default.Set("GAME_PATH", gamePath);

        string translationPath = LocalizationPathEntry.Text.Trim(_trims);
        WriteLog($"Translation path: {LocalizationPathEntry.Text}");
        if (!TranslationPathValidator.IsValidFolder(translationPath))
        {
            WriteLog($"Неверный путь к папке с переводом: \"{translationPath}\"");
            return;
        }

        Preferences.Default.Set("LOCALIZATION_PATH", translationPath);

        if(!CreatePatchFileCheckBox.IsChecked || string.IsNullOrWhiteSpace(PatchPathEntry.Text))
        {
            CreatePatchFileCheckBox.IsChecked = false;
        }

        PatchPathEntry.Text = PatchPathEntry.Text.Trim();
        Preferences.Default.Set("PATCH_PATH", PatchPathEntry.Text);
        Preferences.Default.Set("CREATE_PATCH", CreatePatchFileCheckBox.IsChecked);

        FilesPatcher patcher = new FilesPatcher
        {
            ProgressLogger = new Progress<string>(WriteLog),
            DecompilerPath = decompilerPath
        };

        string patchPath = CreatePatchFileCheckBox.IsChecked ? Path.Combine(PatchPathEntry.Text, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}{(ProcessJarCheckBox.IsChecked ? "_jar" : string.Empty)}") : null;

        try
        {
            patcher.Patch(gamePath, translationPath, ProcessJarCheckBox.IsChecked, patchPath);
        }
        catch(Exception e)
        {
            WriteLog(e.ToString());
        }
    }
}