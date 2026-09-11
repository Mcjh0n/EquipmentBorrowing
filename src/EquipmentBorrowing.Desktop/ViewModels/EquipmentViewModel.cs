using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ViewModelBase
{
    private readonly BorrowingLookupService _borrowingLookupService;
    private readonly BorrowEquipmentService _borrowEquipmentService;

    public ObservableCollection<Student> Students { get; } = new();

    public ObservableCollection<Equipment> AvailableEquipment { get; } = new();

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
        BorrowEquipmentService borrowEquipmentService)
    {
        _borrowingLookupService = borrowingLookupService;
        _borrowEquipmentService = borrowEquipmentService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        Students.Clear();
        AvailableEquipment.Clear();

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
        }

        StatusMessage = AvailableEquipment.Count == 0
            ? "No equipment is currently available."
            : $"{AvailableEquipment.Count} item(s) available.";
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

        var result = await _borrowEquipmentService.ExecuteAsync(
            SelectedStudent.Id,
            SelectedEquipment.Id,
            returnDate);

        StatusMessage = result.Message;

        if (result.IsSuccess)
        {
            SelectedEquipment = null;

            await LoadAsync();

            StatusMessage = result.Message;
        }
    }
}