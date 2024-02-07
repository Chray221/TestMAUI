using TestMAUI.ViewModels;

namespace TestMAUI.Views;

public partial class SwipeMergePage : BaseContentPage
{
	public SwipeMergePage(SwipeMergeViewModel viewModel): base(viewModel)
	{
		InitializeComponent();
	}
}
