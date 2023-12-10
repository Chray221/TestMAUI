using System;
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
            services.AddTransientWithShellRoute<MainPage, MainViewModel>(nameof(MainPage));
            services.AddTransientWithShellRoute<MineSwepperPage, MineSweeperViewModel>(nameof(MineSwepperPage));

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

    }
}

