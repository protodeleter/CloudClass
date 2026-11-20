using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Services;

namespace AvaloniaApplication1.Views;

public partial class MainWindow : Window
{
    private readonly CosmosDbClient _cosmos;

    public MainWindow()
    {
        InitializeComponent();

        // Initialize the database service once
        _cosmos = new CosmosDbClient();
        
        Opened += DatabasesList_OnInitialized;

    }
    
    

    private async void CreateDatabase_Click(
        object? sender,
        RoutedEventArgs e)
    {
        string databaseName =
            DatabaseNameTextBox.Text?.Trim() ?? "";

        string containerName =
            ContainerNameTextBox.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(databaseName) ||
            string.IsNullOrWhiteSpace(containerName))
        {
            StatusText.Text =
                "Please enter database and container names.";
            return;
        }

        try
        {
            CreateButton.IsEnabled = false;
            StatusText.Text = "Creating database...";

            await _cosmos.CreateDbAsync(
                databaseName,
                containerName,
                "/id");

            StatusText.Text =
                "Database and container created!";

            // callback 
            DatabasesList_OnInitialized( sender, e);
        }
        catch (Exception ex)
        {
            StatusText.Text =
                $"Error: {ex.GetBaseException().Message}";
        }
        finally
        {
            CreateButton.IsEnabled = true;
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _cosmos.Dispose();
        base.OnClosed(e);
    }

    private async void DatabasesList_OnInitialized(object? sender, EventArgs e)
    {
        Opened -= DatabasesList_OnInitialized;

        try
        {
            var databases = await _cosmos.GetDataBasesAsync();
             DatabasesList.ItemsSource = databases;

        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }

        
    }
}