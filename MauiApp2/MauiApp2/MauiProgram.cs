using Microsoft.AspNetCore.Components.WebView.Maui;
using MauiApp2.Data;
using MauiApp2.Game;
using Microsoft.Extensions.Configuration;
using BitterClient;

namespace MauiApp2;

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
            });

        builder.Services.AddMauiBlazorWebView();
        #if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif
        
        builder.Services.AddSingleton<WeatherForecastService>();
        builder.Services.AddSingleton<IGameService, GameService>();

        return builder.Build();
    }
}
