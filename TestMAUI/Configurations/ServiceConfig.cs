using System;
using System.ComponentModel;
using CommunityToolkit.Maui;
using TestMAUI.ViewModels;
using TestMAUI.Views;

namespace TestMAUI.Configurations
{
    public static class ServiceConfig
	{
		public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // singleton
            services.AddSingleton<AppShell>();
            //services.AddSingleton<INavigationService, MauiNavigationService>();

            //pages
            //services.AddTransient<MainPage>();
            services.AddTransientShell<MainPage, MainViewModel>();
            services.AddTransientShell<MineSwepperPage, MineSweeperViewModel>();
            services.AddTransientShell<TicTacToePage, TicTacToeViewModel>();

            return services;
        }


        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            //repos

            // singleton

            //transient


            return services;
        }

        public static IServiceCollection RegisterRoutes(this IServiceCollection services)
		{
            return services;
        }

        public static IServiceCollection AddTransientShell<TPage,TViewModel>(this IServiceCollection services)
            where TPage : NavigableElement
            where TViewModel : class, INotifyPropertyChanged
        {
            services.AddTransientWithShellRoute<TPage, TViewModel>(typeof(TPage).Name);
            return services;
        }

    }
}

