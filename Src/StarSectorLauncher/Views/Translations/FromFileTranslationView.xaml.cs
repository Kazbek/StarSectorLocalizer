using Localizer.Patchers;
using Localizer.Validators;

namespace StarSectorLauncher.Views.Translations;

public partial class FromFileTranslationView : ContentView
{
	public FromFileTranslationView()
	{
		InitializeComponent();
	}

    void WriteLog(string text)
    {
        LogLabel.Text = $"{DateTime.Now.ToString("T")}: {text}\n{LogLabel.Text}";
    }

    void ClearLog() { LogLabel.Text = string.Empty; }

    async void OnStartTranslateButtonClicked(object sender, EventArgs args)
    {
        ClearLog();
        await Permissions.RequestAsync<Permissions.StorageWrite>();
        await Permissions.RequestAsync<Permissions.StorageRead>();
        string decompilerPath = Path.Combine(FileSystem.Current.AppDataDirectory, "CFRZip-0.152.jar");
        using (var fileStream = File.Create(decompilerPath))
        {
            var stream = await FileSystem.Current.OpenAppPackageFileAsync("Decompilers\\CFRZip-0.152.jar");
            //myOtherObject.InputStream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fileStream);
        }

        string gamePath = GamePathEntry.Text;
        WriteLog($"Game path: {gamePath}");
        if (!GamePathValidator.IsValid(gamePath))
        {
            WriteLog($"Неверный путь к папке игры: \"{gamePath}\"");
            return;
        }

        string translationPath = LocalizationPathEntry.Text;
        WriteLog($"Translation path: {LocalizationPathEntry.Text}");
        if (!TranslationPathValidator.IsValidFolder(translationPath))
        {
            WriteLog($"Неверный путь к папке с переводом: \"{translationPath}\"");
            return;
        }

        FilesPatcher patcher = new FilesPatcher
        {
            ProgressLogger = new Progress<string>(WriteLog),
            DecompilerPath = decompilerPath
        };

        try
        {
            patcher.Patch(gamePath, translationPath, ProcessJarCheckBox.IsChecked);
        }catch(Exception e)
        {
            WriteLog(e.ToString());
        }
    }
}