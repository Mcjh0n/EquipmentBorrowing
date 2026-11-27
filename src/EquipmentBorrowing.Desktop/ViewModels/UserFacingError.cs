using System;
using System.Collections.Generic;

namespace EquipmentBorrowing.Desktop.ViewModels;

internal static class UserFacingError
{
    public static string From(Exception exception) => exception switch
    {
        EquipmentBorrowing.Desktop.Services.ApiClientException or ArgumentException or InvalidOperationException or KeyNotFoundException => exception.Message,
        EquipmentBorrowing.Application.CommitOutcomeUnknownException => exception.Message,
        _ => "The API operation failed. Refresh data and check the API server."
    };
}


