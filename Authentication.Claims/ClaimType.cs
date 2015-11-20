namespace Affecto.Authentication.Claims
{
    public static class ClaimType
    {
        public const string Id = System.Security.Claims.ClaimTypes.NameIdentifier;
        public const string Name = System.Security.Claims.ClaimTypes.Name;
        public const string Role = System.Security.Claims.ClaimTypes.Role;
        public const string AccountName = "http://affecto.com/claims/accountname";
        public const string Permission = "http://affecto.com/claims/permission";
        public const string Group = "http://affecto.com/claims/group";
        public const string Organization = "http://affecto.com/claims/organization";
        public const string IsSystemUser = "http://affecto.com/claims/issystemuser";
    }
}