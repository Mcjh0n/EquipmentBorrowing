using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class ManagementViewModel(
    BorrowingLookupService lookup,
    StudentManagementService students,
    EquipmentManagementService equipment) : ViewModelBase
{
    public ObservableCollection<Student> Students { get; } = [];
    public ObservableCollection<Equipment> Equipment { get; } = [];
    [ObservableProperty] private Student? selectedStudent;
    [ObservableProperty] private Equipment? selectedEquipment;
    [ObservableProperty] private string studentNumber = string.Empty;
    [ObservableProperty] private string studentName = string.Empty;
    [ObservableProperty] private string studentContact = string.Empty;
    [ObservableProperty] private string equipmentIdentifier = string.Empty;
    [ObservableProperty] private string equipmentName = string.Empty;
    [ObservableProperty] private string equipmentDescription = string.Empty;
    [ObservableProperty] private string statusMessage = string.Empty;

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            Students.Clear(); Equipment.Clear();
            foreach (var student in await lookup.GetStudentsAsync()) Students.Add(student);
            foreach (var item in await lookup.GetEquipmentAsync()) Equipment.Add(item);
            StatusMessage = "Student and equipment records loaded.";
        }
        catch (Exception ex) { StatusMessage = $"Could not load records: {UserFacingError.From(ex)}"; }
    }

    partial void OnSelectedStudentChanged(Student? value)
    {
        if (value is null) return;
        StudentNumber = value.StudentNumber; StudentName = value.Name; StudentContact = value.ContactInformation ?? string.Empty;
    }
    partial void OnSelectedEquipmentChanged(Equipment? value)
    {
        if (value is null) return;
        EquipmentIdentifier = value.EquipmentIdentifier; EquipmentName = value.Name; EquipmentDescription = value.Description ?? string.Empty;
    }

    [RelayCommand]
    private async Task SaveStudentAsync()
    {
        try
        {
            if (SelectedStudent is null) await students.RegisterAsync(StudentNumber, StudentName, StudentContact);
            else await students.UpdateAsync(SelectedStudent.Id, StudentNumber, StudentName, StudentContact);
            StatusMessage = "Student saved."; SelectedStudent = null; await LoadAsync();
        }
        catch (Exception ex) { StatusMessage = UserFacingError.From(ex); }
    }
    [RelayCommand]
    private async Task PermitStudentAsync()
    {
        try
        {
            if (SelectedStudent is null) { StatusMessage = "Select a student first."; return; }
            if (SelectedStudent.IsAllowedToBorrow) { StatusMessage = "Student is already eligible."; return; }
            await students.SetEligibilityAsync(SelectedStudent.Id, true);
            StatusMessage = "Student is now eligible.";
            await LoadAsync();
        }
        catch (Exception ex) { StatusMessage = UserFacingError.From(ex); }
    }
    [RelayCommand]
    private async Task RestrictStudentAsync()
    {
        try
        {
            if (SelectedStudent is null) { StatusMessage = "Select a student first."; return; }
            if (!SelectedStudent.IsAllowedToBorrow) { StatusMessage = "Student is already restricted."; return; }
            await students.SetEligibilityAsync(SelectedStudent.Id, false);
            StatusMessage = "Student is restricted from new borrowings; existing loans remain active and returnable.";
            await LoadAsync();
        }
        catch (Exception ex) { StatusMessage = UserFacingError.From(ex); }
    }
    [RelayCommand]
    private async Task SaveEquipmentAsync()
    {
        try
        {
            if (SelectedEquipment is null) await equipment.RegisterAsync(EquipmentIdentifier, EquipmentName, EquipmentDescription);
            else await equipment.UpdateAsync(SelectedEquipment.Id, EquipmentIdentifier, EquipmentName, EquipmentDescription);
            StatusMessage = "Equipment saved."; SelectedEquipment = null; await LoadAsync();
        }
        catch (Exception ex) { StatusMessage = UserFacingError.From(ex); }
    }
    [RelayCommand]
    private void ClearStudent()
    {
        SelectedStudent = null; StudentNumber = string.Empty; StudentName = string.Empty; StudentContact = string.Empty;
    }
    [RelayCommand]
    private void ClearEquipment()
    {
        SelectedEquipment = null; EquipmentIdentifier = string.Empty; EquipmentName = string.Empty; EquipmentDescription = string.Empty;
    }
}
