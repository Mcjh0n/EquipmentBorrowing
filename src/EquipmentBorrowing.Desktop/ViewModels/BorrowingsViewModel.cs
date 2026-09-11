using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class BorrowingsViewModel : ViewModelBase
{
    private readonly BorrowingLookupService _borrowingLookupService;

    public ObservableCollection<Borrowing> ActiveBorrowings { get; } = new();

    [ObservableProperty]
    private Borrowing? selectedBorrowing;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public BorrowingsViewModel(
        BorrowingLookupService borrowingLookupService)
    {
        _borrowingLookupService = borrowingLookupService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        ActiveBorrowings.Clear();

        IEnumerable<Borrowing> borrowings =
            await _borrowingLookupService.GetActiveBorrowingsAsync();

        foreach (Borrowing borrowing in borrowings)
        {
            ActiveBorrowings.Add(borrowing);
        }

        StatusMessage = ActiveBorrowings.Count == 0
            ? "There are no active borrowings."
            : $"{ActiveBorrowings.Count} active borrowing(s).";
    }
}
