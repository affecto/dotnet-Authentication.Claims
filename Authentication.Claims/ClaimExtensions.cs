using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Affecto.Authentication.Claims
{
    public static class ClaimExtensions
    {
        public static string GetSingleClaimValue(this IEnumerable<Claim> claims, string claimType)
        {
            return GetClaimValues(claims, claimType).SingleOrDefault();
        }

        public static IReadOnlyCollection<string> GetClaimValues(this IEnumerable<Claim> claims, string claimType)
        {
            return claims
                .Where(c => c.Type == claimType)
                .Select(c => c.Value)
                .ToList();
        }
    }
}