using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Supabase;
using SalonBookingApp.Data; 

namespace SalonBookingApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Lora.ttf", "Lora");
                    fonts.AddFont("ImperialScript-Regular.ttf", "Imperial");
                });

            var url = "https://iufctragcodrtzfdwkoq.supabase.co";
            var key = "sb_publishable_b7r2dhT1u58cvW8c8D1WFw_z56qqm02";

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };

            var supabaseClient = new Supabase.Client(url, key, options);
            builder.Services.AddSingleton(supabaseClient);

            builder.Services.AddSingleton<SalonDatabase>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}