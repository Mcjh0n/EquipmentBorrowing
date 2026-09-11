using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ViewModelBase
{
    private readonly BorrowingLookupService _borrowingLookupService;

    public ObservableCollection<Equipment> AvailableEquipment { get; } = new();

    [ObservableProperty]
    private Equipment? selectedEquipment;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public EquipmentViewModel(BorrowingLookupService borrowingLookupService)
    {
        _borrowingLookupService = borrowingLookupService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        AvailableEquipment.Clear();

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
}
