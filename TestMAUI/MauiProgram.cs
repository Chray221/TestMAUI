using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TestMAUI.Configurations;

namespace TestMAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("FontAwesome6Free-Solid-900.otf", "FontAwesomeSolid");
                fonts.AddFont("FontAwesome6Brands-Regular-400.otf", "FontAwesomeBrands");
                fonts.AddFont("FontAwesome6Free-Regular-400.otf", "FontAwesomeRegular");

            });

		builder.Services.RegisterServices()
						.RegisterRepositories()
						.RegisterRoutes();

#if DEBUG
        builder.Logging.AddDebug();
  //      builder.Services.AddLogging(
		//	configure =>
		//	{
		//		configure.AddDebug();
		//		//configure.AddConsole();
		//	}
		//);
#endif

        return builder.Build();
	}
}

