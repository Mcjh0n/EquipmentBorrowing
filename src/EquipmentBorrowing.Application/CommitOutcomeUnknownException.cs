namespace EquipmentBorrowing.Application;

public sealed class CommitOutcomeUnknownException(string message, Exception innerException)
    : Exception(message, innerException);
