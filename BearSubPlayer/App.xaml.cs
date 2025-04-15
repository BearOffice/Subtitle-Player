using BearSubPlayer.Services;
using BearSubPlayer.Stores;
using BearSubPlayer.SubReaders;
using BearSubPlayer.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using WindowsInput;

namespace BearSubPlayer;

public partial class App : Application
{
    private IHost? _host;

    public App() { }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;

        _host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureAppConfiguration(c =>
                {
                    c.SetBasePath(appLocation);
                })
                .ConfigureServices(ConfigureServices)
                .Build();

        await _host.StartAsync();
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddHostedService<ApplicationHostService>();

        services.AddSingleton(_ => new ConfigService(context.HostingEnvironment.ContentRootPath));
        services.AddSingleton(_ => new InputSimulationService(new InputSimulator()));
        services.AddSingleton(_ => new SubReadService(new Dictionary<string, ISubReader>
        {
            ["srt"] = new SrtReader(),
            ["ass"] = new AssReader()
        }));
        services.AddSingleton<SubPlayerService>();

        services.AddSingleton<DispatchCenter>();

        services.AddTransient<MainStore>();
        services.AddTransient<MainWindow>();
        services.AddTransient<TimeJumpWindow>();
        services.AddTransient<SettingStore>();
        services.AddTransient<SettingWindow>();
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        if (_host is null) return;

        await _host.StopAsync();
        _host.Dispose();
        _host = null;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(e.ToString(), "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        Environment.Exit(-1);
    }
}
