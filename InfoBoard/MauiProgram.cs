﻿using Microsoft.Extensions.Logging;

namespace InfoBoard;

/*
 
 Create a .NET MAUI app
 https://learn.microsoft.com/en-us/dotnet/maui/tutorials/notes-app/?view=net-maui-7.0

 */

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}