using TestMAUI.ViewModels;

namespace TestMAUI.Views;

public partial class MineSwepperPage : BaseContentPage
{
	public MineSwepperPage(MineSweeperViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
