// ReSharper disable UnusedVariable

using System;
using System.Security.Claims;
using Affecto.Authentication.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Authentication.Claims.Tests
{
    [TestClass]
    public class AuthenticatedUserContextTests
    {
        private const string Permission = "permission";
        private const string NotExistingPermission = "Not existing permission";
        private const string Group = "group";
        private const string NotExistingGroup = "Not existing group";

        private AuthenticatedUserContext sut;
        private ClaimsIdentity claimsIdentity;

        [TestInitialize]
        public void Setup()
        {
            claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimType.Permission, Permission));
            claimsIdentity.AddClaim(new Claim(ClaimType.Group, Group));
            sut = new AuthenticatedUserContext(claimsIdentity);
        }

        [TestMethod]
        public void AuthenticatedUserContextHasSpecificPermission()
        {
            Assert.IsTrue(sut.HasPermission(Permission));
        }

        [TestMethod]
        public void AuthenticatedUserContextDoesntContainSpecificPermission()
        {
            Assert.IsFalse(sut.HasPermission(NotExistingPermission));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PermissionNameIsNotDefined()
        {
            sut.HasPermission(null);
        }

        [TestMethod]
        public void PermissionNameIsEmpty()
        {
            Assert.IsFalse(sut.HasPermission(string.Empty));
        }

        [TestMethod]
        public void AuthenticatedUserContextHasSpecificGroup()
        {
            Assert.IsTrue(sut.IsInGroup(Group));
        }

        [TestMethod]
        public void AuthenticatedUserContextDoesntContainSpecificGroup()
        {
            Assert.IsFalse(sut.IsInGroup(NotExistingGroup));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GroupNameIsNotDefined()
        {
            sut.IsInGroup(null);
        }

        [TestMethod]
        public void GroupNameIsEmpty()
        {
            Assert.IsFalse(sut.IsInGroup(string.Empty));
        }

        [TestMethod]
        public void UserBelongsToSpecifiedGroup()
        {
            sut.CheckPermission(Permission);
        }

        [TestMethod]
        [ExpectedException(typeof(InsufficientPermissionsException))]
        public void UserDoesNotBelongToSpecifiedGroup()
        {
            sut.CheckPermission(NotExistingPermission);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PermissionNameIsNotDefinedWhenCheckingPermissions()
        {
            sut.CheckPermission(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InsufficientPermissionsException))]
        public void PermissionNameIsEmptyWhenCheckingPermissions()
        {
            sut.CheckPermission(string.Empty);
        }

        [TestMethod]
        public void UserIdIsRetrieved()
        {
            var id = Guid.NewGuid();
            claimsIdentity.AddClaim(new Claim(ClaimType.Id, id.ToString()));

            Assert.AreEqual(id, sut.UserId);
        }

        [TestMethod]
        [ExpectedException(typeof(ClaimNotFoundException))]
        public void UserIdIsNotDefined()
        {
            Guid userId = sut.UserId;
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleClaimsFoundException))]
        public void MultipleUserIds()
        {
            claimsIdentity.AddClaim(new Claim(ClaimType.Id, Guid.NewGuid().ToString()));
            claimsIdentity.AddClaim(new Claim(ClaimType.Id, Guid.NewGuid().ToString()));

            Guid userId = sut.UserId;
        }

        [TestMethod]
        public void UserNameIsRetrieved()
        {
            const string name = "user name";
            claimsIdentity.AddClaim(new Claim(ClaimType.Name, name));

            Assert.AreEqual(name, sut.UserName);
        }

        [TestMethod]
        [ExpectedException(typeof(ClaimNotFoundException))]
        public void UserNameIsNotDefined()
        {
            string userName = sut.UserName;
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleClaimsFoundException))]
        public void MultipleUserNames()
        {
            claimsIdentity.AddClaim(new Claim(ClaimType.Name, "name1"));
            claimsIdentity.AddClaim(new Claim(ClaimType.Name, "name2"));

            string userName = sut.UserName;
        }

        [TestMethod]
        public void IsSystemUserIsSetToTrue()
        {
            claimsIdentity.AddClaim(new Claim(ClaimType.IsSystemUser, Boolean.TrueString));

            Assert.IsTrue(sut.IsSystemUser);
        }

        [TestMethod]
        public void IsSystemUserIsSetToFalse()
        {
            claimsIdentity.AddClaim(new Claim(ClaimType.IsSystemUser, Boolean.FalseString));

            Assert.IsFalse(sut.IsSystemUser);
        }

        [TestMethod]
        public void IsSystemUserReturnsFalseWhenItIsNotDefined()
        {
            Assert.IsFalse(sut.IsSystemUser);
        }

        [TestMethod]
        public void IsSystemUserReturnsFalseWhenClaimsValueCannotBeParsed()
        {
            claimsIdentity.AddClaim(new Claim(ClaimType.IsSystemUser, "Definitely not a boolean value"));

            Assert.IsFalse(sut.IsSystemUser);
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleClaimsFoundException))]
        public void IsSystemUserIsDefinedMultipleTimes()
        {
            claimsIdentity.AddClaim(new Claim(ClaimType.IsSystemUser, Boolean.TrueString));
            claimsIdentity.AddClaim(new Claim(ClaimType.IsSystemUser, Boolean.FalseString));

            bool isSystemUser = sut.IsSystemUser;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasCustomPropertyCannotBeUsedWithNullPropertyName()
        {
            sut.HasCustomProperty(null);
        }

        [TestMethod]
        public void HasCustomPropertyReturnsFalseWhenNoClaimsSet()
        {
            Assert.IsFalse(sut.HasCustomProperty("property"));
        }

        [TestMethod]
        public void HasCustomPropertyReturnsFalseWhenNoPropertyFound()
        {
            const string propertyName = "SomeProp";
            claimsIdentity.AddClaim(new Claim(ClaimTypePrefix.CustomProperty + propertyName, "Foo"));

            Assert.IsFalse(sut.HasCustomProperty("OtherProp"));
        }

        [TestMethod]
        public void HasCustomPropertyReturnsTrueWhenPropertyFound()
        {
            const string propertyName = "SomeProp";
            claimsIdentity.AddClaim(new Claim(ClaimTypePrefix.CustomProperty + propertyName, "Foo"));
            claimsIdentity.AddClaim(new Claim(ClaimTypePrefix.CustomProperty + "OtherProperty", "Bar"));

            Assert.IsTrue(sut.HasCustomProperty(propertyName));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetCustomPropertyValueCannotBeUsedWithNullPropertyName()
        {
            sut.GetCustomPropertyValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ClaimNotFoundException))]
        public void GetCustomPropertyValueThrowsExceptionIfPropertyNotFound()
        {
            sut.GetCustomPropertyValue("SomeProp");
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleClaimsFoundException))]
        public void GetCustomPropertyValueThrowsExceptionIfMultiplePropertiesFound()
        {
            const string propertyName = "SomeProp";
            claimsIdentity.AddClaim(new Claim(ClaimTypePrefix.CustomProperty + propertyName, "Foo"));
            claimsIdentity.AddClaim(new Claim(ClaimTypePrefix.CustomProperty + propertyName, "Bar"));

            sut.GetCustomPropertyValue(propertyName);
        }

        [TestMethod]
        public void GetCustomPropertyValue()
        {
            const string propertyName = "SomeProp";
            const string expectedValue = "Foo";
            claimsIdentity.AddClaim(new Claim(ClaimTypePrefix.CustomProperty + propertyName, expectedValue));
            claimsIdentity.AddClaim(new Claim(ClaimTypePrefix.CustomProperty + "OtherProperty", "Bar"));

            string value = sut.GetCustomPropertyValue(propertyName);

            Assert.AreEqual(expectedValue, value);
        }
    }
}