using CommunityToolkit.Mvvm.ComponentModel;
using StarSectorLauncher.ViewModels;
using StarSectorLauncher.Views.Translations;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using static StarSectorLauncher.Pages.LocalizationPageViewModel;

namespace StarSectorLauncher.Pages;

public partial class LocalizationPage : ContentPage
{
	public LocalizationPage()
	{
		LocalizationPageViewModel viewModel = new LocalizationPageViewModel();
		InitializeComponent();
		BindingContext = viewModel;
		viewModel.OnSourceChanged += ChangeSource;
		
	}

	void ChangeSource(SourceType sourceType)
	{
        VSLayout.Children.RemoveAt(VSLayout.Children.Count - 1);

		IView view = sourceType switch
		{
			SourceType.Files => new FromFileTranslationView(),
			SourceType.GitHub => new FromGitHubTranslationView(),
			_ => throw new NotImplementedException($"Тип {sourceType} не поддерживается")
		};
		VSLayout.Add(view);
    }
}

public partial class LocalizationPageViewModel : BaseViewModel
{
	public ObservableCollection<SourceType> TranslationSources { get; set; } = new ObservableCollection<SourceType> { SourceType.Files, SourceType.GitHub };

	[ObservableProperty]
	//[]
    SourceType selectedTranslation = SourceType.Files;

    public delegate void SourceChanged(SourceType sourceType);
    public event SourceChanged OnSourceChanged;

    partial void OnSelectedTranslationChanged(SourceType value)
	{
        OnSourceChanged?.Invoke(value);
	}

	public enum SourceType 
	{
		[Display(Name = "Файлы")]
		Files,
        [Display(Name = "GitHub")]
        GitHub,
    }
}