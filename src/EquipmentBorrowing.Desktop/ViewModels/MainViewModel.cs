using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string currentSection = "Equipment";

    [ObservableProperty]
    private string currentSectionDescription =
        "View available equipment and start a borrowing transaction.";

    [RelayCommand]
    private void ShowEquipment()
    {
        CurrentSection = "Equipment";
        CurrentSectionDescription =
            "View available equipment and start a borrowing transaction.";
    }

    [RelayCommand]
    private void ShowBorrowings()
    {
        CurrentSection = "Active Borrowings";
        CurrentSectionDescription =
            "View currently borrowed equipment and return a selected item.";
    }
}