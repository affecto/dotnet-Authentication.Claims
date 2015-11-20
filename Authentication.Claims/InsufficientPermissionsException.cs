using System;

namespace Affecto.Authentication.Claims
{
    public class InsufficientPermissionsException : Exception
    {
        public string Permission { get; private set; }

        public InsufficientPermissionsException(string permission)
        {
            Permission = permission;
        }
    }
}