using TestMAUI.ViewModels;

namespace TestMAUI.Views;

public partial class TextTwistPage : BaseContentPage
{
	public TextTwistPage(TextTwistViewModel viewModel): base(viewModel)
	{
		InitializeComponent();
	}
}
