using System;

namespace Affecto.Authentication.Claims
{
    public interface IAuthenticatedUserContext
    {
        Guid UserId { get; }
        string UserName { get; }
        bool IsSystemUser { get; }
        bool HasPermission(string permission);
        bool IsInGroup(string groupName);
        void CheckPermission(string permission);
        bool HasCustomProperty(string customPropertyName);
        string GetCustomPropertyValue(string customPropertyName);
    }
}