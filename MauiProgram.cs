using Microsoft.Extensions.Logging;
using ODSQuizApp.Services;
using ODSQuizApp.Views;

namespace ODSQuizApp
{
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

            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<QuizService>();
            builder.Services.AddSingleton<QuestionService>();
            builder.Services.AddSingleton<ResultService>();
            builder.Services.AddSingleton<SettingsService>();

            // Páginas
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
