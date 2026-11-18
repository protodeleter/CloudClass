using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using Microsoft.Azure.Cosmos;

namespace AvaloniaApplication1;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is
            IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Create one Cosmos DB client
            var cosmos = new CosmosDbClient();

            // Inject it into the ViewModel
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(cosmos)
            };

            // Dispose when the application exits
            desktop.Exit += (_, _) => cosmos.Dispose();
        }

        base.OnFrameworkInitializationCompleted();
    }
}