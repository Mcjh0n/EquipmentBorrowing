using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class BorrowingsViewModel : ViewModelBase
{
    private readonly BorrowingLookupService _borrowingLookupService;
    private readonly ReturnEquipmentService _returnEquipmentService;

    public ObservableCollection<Borrowing> ActiveBorrowings { get; } = new();

    [ObservableProperty]
    private Borrowing? selectedBorrowing;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public BorrowingsViewModel(
        BorrowingLookupService borrowingLookupService,
        ReturnEquipmentService returnEquipmentService)
    {
        _borrowingLookupService = borrowingLookupService;
        _returnEquipmentService = returnEquipmentService;
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

    [RelayCommand]
    private async Task ReturnEquipmentAsync()
    {
        if (SelectedBorrowing is null)
        {
            StatusMessage = "Select an active borrowing before returning equipment.";
            return;
        }

        var result = await _returnEquipmentService.ExecuteAsync(
            SelectedBorrowing.Id);

        StatusMessage = result.Message;

        if (result.IsSuccess)
        {
            SelectedBorrowing = null;

            await LoadAsync();

            StatusMessage = result.Message;
        }
    }
}