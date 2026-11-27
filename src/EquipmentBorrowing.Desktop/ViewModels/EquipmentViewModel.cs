using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Desktop.Services;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ViewModelBase
{
    private readonly BorrowingLookupService _borrowingLookupService;
    private readonly BorrowEquipmentService _borrowEquipmentService;
    private readonly MultiItemCheckoutService _multiItemCheckoutService;

    public ObservableCollection<Student> Students { get; } = new();

    public ObservableCollection<Equipment> AvailableEquipment { get; } = new();
    public ObservableCollection<Equipment> EquipmentCatalog { get; } = new();
    public ObservableCollection<EquipmentChoice> EquipmentChoices { get; } = new();

    [ObservableProperty]
    private Student? selectedStudent;

    [ObservableProperty]
    private Equipment? selectedEquipment;

    [ObservableProperty]
    private DateTimeOffset? expectedReturnDate;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public EquipmentViewModel(
        BorrowingLookupService borrowingLookupService,
        BorrowEquipmentService borrowEquipmentService,
        MultiItemCheckoutService multiItemCheckoutService)
    {
        _borrowingLookupService = borrowingLookupService;
        _borrowEquipmentService = borrowEquipmentService;
        _multiItemCheckoutService = multiItemCheckoutService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        StatusMessage = "Loading equipment from API...";
        Students.Clear();
        AvailableEquipment.Clear();
        EquipmentCatalog.Clear();
        EquipmentChoices.Clear();

        IEnumerable<Student> students =
            await _borrowingLookupService.GetStudentsAsync();

        foreach (Student student in students)
        {
            Students.Add(student);
        }

        IEnumerable<Equipment> equipment =
            await _borrowingLookupService.GetAvailableEquipmentAsync();

        foreach (Equipment item in equipment)
        {
            AvailableEquipment.Add(item);
            EquipmentChoices.Add(new EquipmentChoice(item));
        }

        foreach (var item in await _borrowingLookupService.GetEquipmentAsync()) EquipmentCatalog.Add(item);

        StatusMessage = EquipmentCatalog.Count == 0
            ? "No equipment is currently available."
            : $"Catalog: {EquipmentCatalog.Count} item(s); {AvailableEquipment.Count} currently available.";
    }

    [RelayCommand]
    private async Task BorrowSelectedItemsAsync()
    {
        if (SelectedStudent is null) { StatusMessage = "Select a student before checkout."; return; }
        if (ExpectedReturnDate is null) { StatusMessage = "Select an expected return date."; return; }
        var ids = EquipmentChoices.Where(x => x.IsSelected).Select(x => x.Item.Id).ToArray();
        try
        {
            var result = await _multiItemCheckoutService.ExecuteAsync(
                SelectedStudent.Id, ids, DateOnly.FromDateTime(ExpectedReturnDate.Value.Date));
            var message = $"Checked out {result.Count} items atomically.";
            await LoadAsync();
            StatusMessage = message;
        }
        catch (Exception ex) { StatusMessage = UserFacingError.From(ex); }
    }

    [RelayCommand]
    private async Task BorrowAsync()
    {
        if (SelectedStudent is null)
        {
            StatusMessage = "Select a student before borrowing equipment.";
            return;
        }

        if (SelectedEquipment is null)
        {
            StatusMessage = "Select equipment before borrowing.";
            return;
        }

        if (ExpectedReturnDate is null)
        {
            StatusMessage = "Select an expected return date.";
            return;
        }

        DateTime returnDate = ExpectedReturnDate.Value.DateTime;

        if (returnDate.Date <= DateTime.Today)
        {
            StatusMessage = "The expected return date must be after today.";
            return;
        }

        EquipmentBorrowing.Application.BorrowingResult result;
        try
        {
            result = await _borrowEquipmentService.ExecuteAsync(
                SelectedStudent.Id,
                SelectedEquipment.Id,
                returnDate);
        }
        catch (Exception ex) { StatusMessage = UserFacingError.From(ex); return; }

        StatusMessage = result.Message;

        if (result.IsSuccess)
        {
            SelectedEquipment = null;

            await LoadAsync();

            StatusMessage = result.Message;
        }
    }
}

public sealed class EquipmentChoice(Equipment item) : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    public Equipment Item { get; } = item;
    private bool _isSelected;
    public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
}


