using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Threading.Tasks;

namespace EasySave;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static async Task Main(string[] args)
    {
        var configModel = ConfigModel.Instance;
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        Server server = new Server(8080);
        await server.Start();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}