using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly EquipmentViewModel _equipmentViewModel;
    private readonly BorrowingsViewModel _borrowingsViewModel;

    [ObservableProperty]
    private string currentSection = "Equipment";

    [ObservableProperty]
    private string currentSectionDescription =
        "View available equipment and start a borrowing transaction.";

    [ObservableProperty]
    private ViewModelBase currentView = null!;

    public MainViewModel(
        EquipmentViewModel equipmentViewModel,
        BorrowingsViewModel borrowingsViewModel)
    {
        _equipmentViewModel = equipmentViewModel;
        _borrowingsViewModel = borrowingsViewModel;
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
}