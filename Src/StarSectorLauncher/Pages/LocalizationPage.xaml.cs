using CommunityToolkit.Mvvm.ComponentModel;
using StarSectorLauncher.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace StarSectorLauncher.Pages;

public partial class LocalizationPage : ContentPage
{
	public LocalizationPage()
	{
		LocalizationPageViewModel viewModel = new LocalizationPageViewModel();
        VSLayout.Children.RemoveAt(VSLayout.Children.Count - 1);
		InitializeComponent();
		BindingContext = viewModel;
		//viewModel.On
		
	}
}

public partial class LocalizationPageViewModel : BaseViewModel
{
	public ObservableCollection<SourceType> TranslationSources { get; set; } = new ObservableCollection<SourceType> { SourceType.Files, SourceType.GitHub };

	[ObservableProperty]
	//[]
    SourceType selectedTranslation = SourceType.Files;

    public enum SourceType 
	{
		[Display(Name = "װאיכû")]
		Files,
        [Display(Name = "GitHub")]
        GitHub,
    }
}