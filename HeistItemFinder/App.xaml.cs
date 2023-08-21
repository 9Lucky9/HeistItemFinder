using HeistItemFinder.Interfaces;
using HeistItemFinder.MVVM.ViewModels;
using HeistItemFinder.MVVM.Views;
using HeistItemFinder.Realizations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace HeistItemFinder;

public partial class App : Application
{
    public static IHost AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                //Add services
                services.AddTransient<ILeaguesParser, LeaguesParser>();
                services.AddTransient<IPoeNinjaParser, PoeNinjaParser>();
                services.AddTransient<IPoeTradeParser, PoeTradeParser>();
                services.AddTransient<IOpenCvVision, OpenCvVision>();
                services.AddTransient<ITextFromImageReader, TextFromImageReader>();
                services.AddSingleton<IScreenShotWin32, ScreenShotWin32>();
                services.AddSingleton<IKeyboardHook, KeyboardHook>();
                services.AddTransient<IItemFinder, ItemFinder>();

                //Add ViewModels
                services.AddSingleton<SettingsViewModel>(provider =>
                new SettingsViewModel(provider.GetRequiredService<ILeaguesParser>()));
                services.AddSingleton<SearchViewModel>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton<MainWindow>(provider => new MainWindow()
                {
                    DataContext = provider.GetRequiredService<MainWindowViewModel>()
                });
                services.AddSingleton<SettingsView>(provider => new SettingsView()
                {
                    DataContext = provider.GetRequiredService<SettingsViewModel>()
                });
                services.AddSingleton<Popup>(provider => new Popup());
                services.AddSingleton<ErrorPopup>(provider => new ErrorPopup());
                services.AddSingleton(provider =>
                new SearchViewModel(
                    provider.GetRequiredService<IPoeTradeParser>(),
                    provider.GetRequiredService<IPoeNinjaParser>(),
                    provider.GetRequiredService<IOpenCvVision>(),
                    provider.GetRequiredService<ITextFromImageReader>(),
                    provider.GetRequiredService<IScreenShotWin32>(),
                    provider.GetRequiredService<IKeyboardHook>(),
                    provider.GetRequiredService<IItemFinder>(),
                    provider.GetRequiredService<Popup>(),
                    provider.GetRequiredService<ErrorPopup>()
                    ));
                services.AddSingleton<SearchView>(provider => new SearchView()
                {
                    DataContext = provider.GetRequiredService<SearchViewModel>()
                });
            }).Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();
        var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
        startupForm.Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        //Unhook keyboard
        var keyHook = AppHost.Services.GetRequiredService<IKeyboardHook>();
        keyHook.UnHookKeyboard();
        await AppHost.StopAsync();
        base.OnExit(e);
    }
}
