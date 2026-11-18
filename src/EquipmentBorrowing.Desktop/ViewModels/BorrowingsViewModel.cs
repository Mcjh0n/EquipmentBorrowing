using System;
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
    public ObservableCollection<Student> Students { get; } = new();
    public ObservableCollection<Equipment> Equipment { get; } = new();
    public ObservableCollection<string> StatusOptions { get; } = ["Active", "Returned", "All"];
    public ObservableCollection<string> SortOptions { get; } = ["DateBorrowed", "ExpectedReturnDate"];

    [ObservableProperty]
    private Borrowing? selectedBorrowing;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string statusFilter = "Active";
    [ObservableProperty] private string sortField = "DateBorrowed";
    [ObservableProperty] private bool sortDescending;
    [ObservableProperty] private Student? selectedStudentForHistory;
    [ObservableProperty] private Equipment? selectedEquipmentForHistory;
    [ObservableProperty] private bool isLoading;

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
        IsLoading = true;
        StatusMessage = "Loading borrowings...";
        try
        {
            ActiveBorrowings.Clear();
            var status = StatusFilter switch { "Returned" => BorrowingStatus.Returned, "All" => (BorrowingStatus?)null, _ => BorrowingStatus.Active };
            var borrowings = await _borrowingLookupService.SearchBorrowingsAsync(SearchText, status, SortField, SortDescending);
            foreach (Borrowing borrowing in borrowings) ActiveBorrowings.Add(borrowing);
            Students.Clear(); Equipment.Clear();
            foreach (var student in await _borrowingLookupService.GetStudentsAsync()) Students.Add(student);
            foreach (var item in await _borrowingLookupService.GetEquipmentAsync()) Equipment.Add(item);
            StatusMessage = ActiveBorrowings.Count == 0 ? "No borrowings match these criteria." : $"{ActiveBorrowings.Count} borrowing(s).";
        }
        catch (Exception ex) { StatusMessage = $"Could not load borrowings: {UserFacingError.From(ex)}"; }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task SearchAsync() => await LoadAsync();

    [RelayCommand]
    private async Task ClearSearchAsync()
    {
        SearchText = string.Empty; StatusFilter = "Active"; SortField = "DateBorrowed"; SortDescending = false;
        SelectedStudentForHistory = null; SelectedEquipmentForHistory = null;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task ShowStudentHistoryAsync()
    {
        if (SelectedStudentForHistory is null) { StatusMessage = "Select a student to view history."; return; }
        try { IsLoading = true; StatusMessage = "Loading student history..."; await LoadHistoryAsync(await _borrowingLookupService.GetStudentHistoryAsync(SelectedStudentForHistory.Id)); }
        catch (Exception ex) { StatusMessage = UserFacingError.From(ex); }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task ShowEquipmentHistoryAsync()
    {
        if (SelectedEquipmentForHistory is null) { StatusMessage = "Select equipment to view history."; return; }
        try { IsLoading = true; StatusMessage = "Loading equipment history..."; await LoadHistoryAsync(await _borrowingLookupService.GetEquipmentHistoryAsync(SelectedEquipmentForHistory.Id)); }
        catch (Exception ex) { StatusMessage = UserFacingError.From(ex); }
        finally { IsLoading = false; }
    }

    private async Task LoadHistoryAsync(IEnumerable<Borrowing> history)
    {
        ActiveBorrowings.Clear(); foreach (var borrowing in history) ActiveBorrowings.Add(borrowing);
        StatusMessage = ActiveBorrowings.Count == 0 ? "No borrowing history for this selection." : $"{ActiveBorrowings.Count} history record(s).";
    }

    [RelayCommand]
    private async Task ReturnEquipmentAsync()
    {
        if (SelectedBorrowing is null)
        {
            StatusMessage = "Select an active borrowing before returning equipment.";
            return;
        }

        EquipmentBorrowing.Application.BorrowingResult result;
        try { result = await _returnEquipmentService.ExecuteAsync(SelectedBorrowing.Id); }
        catch (Exception ex) { StatusMessage = UserFacingError.From(ex); return; }

        StatusMessage = result.Message;

        if (result.IsSuccess)
        {
            SelectedBorrowing = null;

            await LoadAsync();

            StatusMessage = result.Message;
        }
    }
}
