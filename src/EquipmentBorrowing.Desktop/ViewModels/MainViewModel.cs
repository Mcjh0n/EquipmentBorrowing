using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly EquipmentViewModel _equipmentViewModel;
    private readonly BorrowingsViewModel _borrowingsViewModel;
    private readonly ManagementViewModel _managementViewModel;

    [ObservableProperty]
    private string currentSection = "Equipment";

    [ObservableProperty]
    private string currentSectionDescription =
        "View available equipment and start a borrowing transaction.";

    [ObservableProperty]
    private ViewModelBase currentView = null!;

    public MainViewModel(
        EquipmentViewModel equipmentViewModel,
        BorrowingsViewModel borrowingsViewModel,
        ManagementViewModel managementViewModel
        )
    {
        _equipmentViewModel = equipmentViewModel;
        _borrowingsViewModel = borrowingsViewModel;
        _managementViewModel = managementViewModel;
        CurrentView = _equipmentViewModel;
    }

    [RelayCommand]
    private async Task ShowEquipmentAsync()
    {
        CurrentSection = "Equipment";
        CurrentSectionDescription =
            "View available equipment and start a borrowing transaction.";

        CurrentView = _equipmentViewModel;
        await _equipmentViewModel.LoadAsync();
    }

    [RelayCommand]
    private async Task ShowBorrowingsAsync()
    {
        CurrentSection = "Active Borrowings";
        CurrentSectionDescription =
            "View currently borrowed equipment and return a selected item.";

        CurrentView = _borrowingsViewModel;
        await _borrowingsViewModel.LoadAsync();
    }

    [RelayCommand]
    private async Task ShowManagementAsync()
    {
        CurrentSection = "Manage Records";
        CurrentSectionDescription = "Register and update students and equipment records.";
        CurrentView = _managementViewModel;
        await _managementViewModel.LoadAsync();
    }

    [ObservableProperty] private string startupMessage = string.Empty;
    public bool IsDatabaseUnavailable => !string.IsNullOrEmpty(StartupMessage);
    partial void OnStartupMessageChanged(string value) => OnPropertyChanged(nameof(IsDatabaseUnavailable));

    [RelayCommand]
    private async Task RetryDatabaseAsync()
    {
        try
        {
            StartupMessage = string.Empty;
            await _equipmentViewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            StartupMessage = UserFacingError.From(ex);
        }
    }
}



