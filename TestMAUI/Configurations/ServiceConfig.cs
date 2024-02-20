using System;
using System.ComponentModel;
using CommunityToolkit.Maui;
using TestMAUI.Services;
using TestMAUI.Services.Interfaces;
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
            services.AddSingleton<ISwipeMergeService>(SwipeMergeService.Instance);
            //services.AddSingleton<INavigationService, MauiNavigationService>();

            //pages
            //services.AddTransient<MainPage>();
            services.AddTransientShell<MainPage, MainViewModel>();
            services.AddTransientShell<MineSwepperPage, MineSweeperViewModel>();
            services.AddTransientShell<TicTacToePage, TicTacToeViewModel>();
            services.AddTransientShell<SwipeMergePage, SwipeMergeViewModel>();
            services.AddTransientShell<TextTwistPage, TextTwistViewModel>();

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

