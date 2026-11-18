using System;
using System.Collections.Generic;

namespace EquipmentBorrowing.Desktop.ViewModels;

internal static class UserFacingError
{
    public static string From(Exception exception) => exception switch
    {
        ArgumentException or InvalidOperationException or KeyNotFoundException => exception.Message,
        EquipmentBorrowing.Application.CommitOutcomeUnknownException => exception.Message,
        _ => "The database operation failed. Check the PostgreSQL connection and try again."
    };
}
