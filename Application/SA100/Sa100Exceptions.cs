namespace HMRC_TAX_FLOW.Application.SA100;

public sealed class Sa100NotFoundException() : Exception("The SA100 return was not found.");

public sealed class ClientNotFoundException() : Exception("The client was not found.");

public sealed class ClientAccessDeniedException() : Exception("The user cannot access this client or return.");

public sealed class Sa100ConflictException() : Exception("The SA100 return is no longer an editable draft.");
