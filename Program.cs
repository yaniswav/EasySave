using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;

namespace EasySave;

sealed class Program
{
    private static Server server;
    private static Task? serverTask;

    [STAThread]
    public static async Task Main(string[] args)
    {
        var configModel = ConfigModel.Instance;
        server = new Server(8080);
        serverTask = Task.Run(() => server.Start());

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static void ShutdownServer()
    {
        if (server != null)
        {
            server.Stop(); // Stop the server
            serverTask?.Wait(); // Ensure the task completes
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}