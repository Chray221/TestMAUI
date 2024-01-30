using TestMAUI.ViewModels;

namespace TestMAUI.Views;

public partial class TicTacToePage : BaseContentPage
{
	public TicTacToePage(TicTacToeViewModel viewModel):base(viewModel)
	{
		InitializeComponent();
	}
}
