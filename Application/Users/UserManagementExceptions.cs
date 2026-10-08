namespace HMRC_TAX_FLOW.Application.Users;

public sealed class ManagedUserNotFoundException() : Exception("The user was not found.");

public sealed class InvalidManagedUserRoleException()
    : Exception("Only User, Practice, or Debitam can be assigned through this endpoint.");
