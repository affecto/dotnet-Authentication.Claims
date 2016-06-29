using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Affecto.Authentication.Claims
{
    public class AuthenticatedUserContext : IAuthenticatedUserContext
    {
        private readonly ClaimsIdentity identity;

        public AuthenticatedUserContext(ClaimsIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            this.identity = identity;
        }

        public Guid UserId
        {
            get { return Guid.Parse(GetClaim(ClaimType.Id)); }
        }

        public string UserName
        {
            get { return GetClaim(ClaimType.Name); }
        }

        public string AccountName
        {
            get { return GetClaim(ClaimType.AccountName); }
        }

        public bool IsSystemUser
        {
            get
            {
                bool result;
                if (HasClaim(ClaimType.IsSystemUser) && bool.TryParse(GetClaim(ClaimType.IsSystemUser), out result))
                {
                    return result;
                }
                return false;
            }
        }

        public bool HasPermission(string permission)
        {
            if (permission == null)
            {
                throw new ArgumentNullException("permission");
            }

            return identity.HasClaim(ClaimType.Permission, permission);
        }

        public bool IsInGroup(string groupName)
        {
            if (groupName == null)
            {
                throw new ArgumentNullException("groupName");
            }

            return identity.HasClaim(ClaimType.Group, groupName);
        }

        public void CheckPermission(string permission)
        {
            if (!HasPermission(permission))
            {
                throw new InsufficientPermissionsException(permission);
            }
        }

        public bool HasCustomProperty(string customPropertyName)
        {
            if (customPropertyName == null)
            {
                throw new ArgumentNullException("customPropertyName");
            }
            return HasClaim(ClaimTypePrefix.CustomProperty + customPropertyName);
        }

        public string GetCustomPropertyValue(string customPropertyName)
        {
            if (customPropertyName == null)
            {
                throw new ArgumentNullException("customPropertyName");
            }
            return GetClaim(ClaimTypePrefix.CustomProperty + customPropertyName);
        }

        public bool HasClaim(string name)
        {
            return identity.FindFirst(name) != null;
        }

        public string GetClaim(string name)
        {
            List<Claim> claims = identity.FindAll(name).ToList();

            if (claims.Count == 0)
            {
                throw new ClaimNotFoundException(name);
            }
            if (claims.Count > 1)
            {
                throw new MultipleClaimsFoundException(name);
            }

            return claims[0].Value;
        }
    }
}