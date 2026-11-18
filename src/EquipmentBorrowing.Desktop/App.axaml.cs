using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Desktop.ViewModels;
using EquipmentBorrowing.Desktop.Views;
using EquipmentBorrowing.Infrastructure.Persistence;
using EquipmentBorrowing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EquipmentBorrowing.Desktop;

public partial class App : Avalonia.Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        services.AddDbContextFactory<EquipmentBorrowingDbContext>(options =>
            options
                .UseNpgsql(EquipmentBorrowingDatabase.GetConnectionString())
                .LogTo(message => System.Diagnostics.Trace.WriteLine(message)));

        services.AddTransient<IStudentRepository, EfStudentRepository>();
        services.AddTransient<IEquipmentRepository, EfEquipmentRepository>();
        services.AddTransient<IBorrowingRepository, EfBorrowingRepository>();

        services.AddTransient<BorrowEquipmentService>();
        services.AddTransient<ReturnEquipmentService>();
        services.AddTransient<BorrowingLookupService>();
        services.AddTransient<StudentManagementService>();
        services.AddTransient<EquipmentManagementService>();
        services.AddTransient<MultiItemCheckoutService>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<EquipmentViewModel>();
        services.AddTransient<BorrowingsViewModel>();
        services.AddTransient<ManagementViewModel>();

        _serviceProvider = services.BuildServiceProvider();

        string? startupMessage = null;
        try
        {
            using var dbContext = _serviceProvider
                .GetRequiredService<IDbContextFactory<EquipmentBorrowingDbContext>>()
                .CreateDbContext();
            DatabaseInitializer.InitializeAsync(dbContext).GetAwaiter().GetResult();
        }
        catch (Exception)
        {
            startupMessage = "PostgreSQL is unavailable or not configured. Set EquipmentBorrowing__ConnectionString, then use Retry Database after the server is ready.";
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            mainViewModel.StartupMessage = startupMessage ?? string.Empty;
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            desktop.Exit += (_, _) => _serviceProvider.Dispose();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
