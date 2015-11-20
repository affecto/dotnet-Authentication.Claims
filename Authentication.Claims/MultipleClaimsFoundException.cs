using System;

namespace Affecto.Authentication.Claims
{
    public class MultipleClaimsFoundException : Exception
    {
        public string ClaimName { get; private set; }

        public MultipleClaimsFoundException(string claimName)
        {
            ClaimName = claimName;
        }
    }
}