using System;
using System.Threading.Tasks;
using AvaloniaApplication1.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    
    private readonly CosmosDbClient? _cosmos;

    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome to Avalonia!";
    
    [ObservableProperty]
    public partial string DatabaseName { get; set; }
    [ObservableProperty]
    public partial string ContainerName { get; set; }
    
    
    [RelayCommand]
    private async Task ShowValues()
    {

        if (_cosmos != null)
        {
            await _cosmos.CreateDb(DatabaseName!, ContainerName!);
        }
        
        // Console.WriteLine($"Database: {DatabaseName}");
        // Console.WriteLine($"Container: {ContainerName}");
    }
    
}
