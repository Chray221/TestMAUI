using System;
namespace TestMAUI.Services.Interfaces
{
	public interface IPageLifeCycle
    {
        void OnNavigatingTo();
        void OnNavigatedTo();
        void OnNavigatingFrom();
        void OnNavigatedFrom();
        void OnDisappearing();
    }

    public interface IPageLoadedLifeCycle
    {
        void OnPageLoaded();
    }
}

