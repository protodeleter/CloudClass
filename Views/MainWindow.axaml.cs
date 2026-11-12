using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Services;
using System;
using System.Diagnostics;

namespace AvaloniaApplication1.Views;

public partial class MainWindow : Window
{
    private readonly CosmosDbClient? _cosmos;

    public MainWindow()
    {
        InitializeComponent();
        _cosmos = new CosmosDbClient(
            "MyDatabase",
            "Customer");

    }


    private async void clickFunc(
        object? sender,
        RoutedEventArgs e)
    {
        try
        {
            var customer = new Customer
            {
                Name = "John",
                Email = "john@example.com",
                Age = 30,
                Category = "general"
            };
            await _cosmos.InsertAsync(
                customer,
                customer.Id);

            Debug.WriteLine("Inserted successfully!");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }

}