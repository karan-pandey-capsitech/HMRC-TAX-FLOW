namespace HMRC_TAX_FLOW.Application.Clients;

public sealed class InvalidClientAssignmentException(string message) : Exception(message);
