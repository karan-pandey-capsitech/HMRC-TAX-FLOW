namespace HMRC_TAX_FLOW.Application.Authentication;

public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("The username or password is incorrect.")
    {
    }
}

public sealed class UsernameAlreadyExistsException : Exception
{
    public UsernameAlreadyExistsException()
        : base("That username is already in use.")
    {
    }
}
