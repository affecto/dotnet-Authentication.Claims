using System;

namespace Affecto.Authentication.Claims
{
    public class ClaimNotFoundException : Exception
    {
        public string ClaimName { get; private set; }

        public ClaimNotFoundException(string claimName)
        {
            ClaimName = claimName;
        }
    }
}