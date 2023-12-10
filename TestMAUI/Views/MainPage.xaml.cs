using Microsoft.Extensions.Logging;
using TestMAUI.Services.Interfaces;
using TestMAUI.ViewModels;

namespace TestMAUI.Views;

public partial class MainPage : BaseContentPage
{
	public MainPage(MainViewModel viewModel) : base(viewModel)
	{
        InitializeComponent();
    }
}


