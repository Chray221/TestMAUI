using TestMAUI.Services.Interfaces;
using TestMAUI.ViewModels;

namespace TestMAUI.Views;

public class BaseContentPage : ContentPage
{
	public BaseContentPage(BaseViewModel viewModel)
	{
        BindingContext = viewModel;
        if (viewModel is IPageLoadedLifeCycle)
        {
            Loaded += BaseContentPage_Loaded;
        }
    }

    private void BaseContentPage_Loaded(object sender, EventArgs e)
    {
        if (BindingContext is IPageLoadedLifeCycle pageLifeCycle)
        {
            pageLifeCycle.OnPageLoaded();
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is IPageLifeCycle pageLifeCycle)
        {
            pageLifeCycle.OnNavigatingTo();
        }
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (BindingContext is IPageLifeCycle pageLifeCycle)
        {
            pageLifeCycle.OnNavigatedTo();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is IPageLifeCycle pageLifeCycle)
        {
            pageLifeCycle.OnDisappearing();
        }
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        if (BindingContext is IPageLifeCycle pageLifeCycle)
        {
            pageLifeCycle.OnNavigatedFrom();
        }
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
        if (BindingContext is IPageLifeCycle pageLifeCycle)
        {
            pageLifeCycle.OnNavigatingFrom();
        }
    }
}
