namespace HMRC_TAX_FLOW.Domain.Users
{
    public static class UserRole
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Practice = "Practice";
        public const string Debitam = "Debitam";

        public static bool TryNormalizeAssignable(string? role, out string normalizedRole)
        {
            normalizedRole = role?.Trim().ToUpperInvariant() switch
            {
                "USER" => User,
                "PRACTICE" => Practice,
                "DEBITAM" => Debitam,
                _ => string.Empty
            };

            return normalizedRole.Length > 0;
        }
    }
}
