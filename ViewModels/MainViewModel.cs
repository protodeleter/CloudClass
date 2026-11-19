
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AvaloniaApplication1.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly CosmosDbClient _cosmos;

    [ObservableProperty]
    public partial string Greeting { get; set; } =
        "Welcome to Avalonia!";

    [ObservableProperty]
    public partial string DatabaseName { get; set; } = "";

    [ObservableProperty]
    public partial string ContainerName { get; set; } = "";

    [ObservableProperty]
    public partial string Status { get; set; } = "Ready";

    public MainViewModel(CosmosDbClient cosmos)
    {
        _cosmos = cosmos;
    }

    [RelayCommand]
    private async Task ShowValues()
    {
        if (string.IsNullOrWhiteSpace(DatabaseName) ||
            string.IsNullOrWhiteSpace(ContainerName))
        {
            Status = "Please enter database and container names.";
            return;
        }

        try
        {
            Status = "Creating database...";

            await _cosmos.CreateDbAsync(
                DatabaseName,
                ContainerName);

            Status = "Database and container created!";
        }
        catch (Exception ex)
        {
            Status = $"Error: {ex.GetBaseException().Message}";
            Debug.WriteLine(ex.ToString());
        }
    }
}
