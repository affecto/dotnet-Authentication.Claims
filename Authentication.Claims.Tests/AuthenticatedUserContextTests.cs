﻿// ReSharper disable UnusedVariable

using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string Role = "role";
        private const string NotExistingRole = "Not existing role";
        private const string Group = "group";
        private const string NotExistingGroup = "Not existing group";

        private AuthenticatedUserContext sut;
        private ClaimsIdentity claimsIdentity;

        [TestInitialize]
        public void Setup()
        {
            claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimType.Permission, Permission));
            claimsIdentity.AddClaim(new Claim(ClaimType.Permission, "OtherPermission"));
            claimsIdentity.AddClaim(new Claim(ClaimType.Role, Role));
            claimsIdentity.AddClaim(new Claim(ClaimType.Role, "OtherRole"));
            claimsIdentity.AddClaim(new Claim(ClaimType.Group, Group));
            claimsIdentity.AddClaim(new Claim(ClaimType.Group, "OtherGroup"));
            sut = new AuthenticatedUserContext(claimsIdentity);
        }

        [TestMethod]
        public void HasSpecificPermission()
        {
            Assert.IsTrue(sut.HasPermission(Permission));
        }

        [TestMethod]
        public void DoesNotHaveSpecificPermission()
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
        public void HasSpecificRole()
        {
            Assert.IsTrue(sut.HasRole(Role));
        }

        [TestMethod]
        public void DoesNotHaveSpecificRole()
        {
            Assert.IsFalse(sut.HasRole(NotExistingRole));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RoleNameIsNotDefined()
        {
            sut.HasRole(null);
        }

        [TestMethod]
        public void RoleNameIsEmpty()
        {
            Assert.IsFalse(sut.HasRole(string.Empty));
        }

        [TestMethod]
        public void HasSpecificGroup()
        {
            Assert.IsTrue(sut.IsInGroup(Group));
        }

        [TestMethod]
        public void DoesNotHaveSpecificGroup()
        {
            Assert.IsFalse(sut.IsInGroup(NotExistingGroup));
        }

        [TestMethod]
        public void HasSpecificGroups()
        {
            IReadOnlyCollection<string> groups = sut.GetGroups();

            Assert.AreEqual(2, groups.Count);
            Assert.AreEqual(Group, groups.ElementAt(0));
            Assert.AreEqual("OtherGroup", groups.ElementAt(1));
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
        public void UserHasPermission()
        {
            sut.CheckPermission(Permission);
        }

        [TestMethod]
        [ExpectedException(typeof(InsufficientPermissionsException))]
        public void UserDoesNotHavePermission()
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
        public void AccountNameIsRetrieved()
        {
            const string name = "account name";
            claimsIdentity.AddClaim(new Claim(ClaimType.AccountName, name));

            Assert.AreEqual(name, sut.AccountName);
        }

        [TestMethod]
        [ExpectedException(typeof(ClaimNotFoundException))]
        public void AccountNameIsNotDefined()
        {
            string userName = sut.AccountName;
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleClaimsFoundException))]
        public void MultipleAccountNames()
        {
            claimsIdentity.AddClaim(new Claim(ClaimType.AccountName, "name1"));
            claimsIdentity.AddClaim(new Claim(ClaimType.AccountName, "name2"));

            string userName = sut.AccountName;
        }

        [TestMethod]
        public void IsSystemUserIsSetToTrue()
        {
            claimsIdentity.AddClaim(new Claim(ClaimType.IsSystemUser, bool.TrueString));

            Assert.IsTrue(sut.IsSystemUser);
        }

        [TestMethod]
        public void IsSystemUserIsSetToFalse()
        {
            claimsIdentity.AddClaim(new Claim(ClaimType.IsSystemUser, bool.FalseString));

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
            claimsIdentity.AddClaim(new Claim(ClaimType.IsSystemUser, bool.TrueString));
            claimsIdentity.AddClaim(new Claim(ClaimType.IsSystemUser, bool.FalseString));

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

        [TestMethod]
        public void DoesNotContainClaim()
        {
            Assert.IsFalse(sut.HasClaim(ClaimType.Name));
        }

        [TestMethod]
        public void ContainsSingleClaim()
        {
            const string name = "user name";
            claimsIdentity.AddClaim(new Claim(ClaimType.Name, name));

            Assert.IsTrue(sut.HasClaim(ClaimType.Name));
        }

        [TestMethod]
        public void ContainsMultipleClaims()
        {
            const string name = "user name";
            claimsIdentity.AddClaim(new Claim(ClaimType.Name, name));
            claimsIdentity.AddClaim(new Claim(ClaimType.Name, name));

            Assert.IsTrue(sut.HasClaim(ClaimType.Name));
        }

        [TestMethod]
        public void SingleClaimIsRetrieved()
        {
            const string name = "user name";
            claimsIdentity.AddClaim(new Claim(ClaimType.Name, name));

            Assert.AreEqual(name, sut.GetClaim(ClaimType.Name));
        }

        [TestMethod]
        [ExpectedException(typeof(ClaimNotFoundException))]
        public void ClaimIsNotDefined()
        {
            sut.GetClaim(ClaimType.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleClaimsFoundException))]
        public void MultipleClaimsDefined()
        {
            claimsIdentity.AddClaim(new Claim(ClaimType.Name, "name1"));
            claimsIdentity.AddClaim(new Claim(ClaimType.Name, "name2"));

            sut.GetClaim(ClaimType.Name);
        }

        [TestMethod]
        public void MultipleClaimsRetrieved()
        {
            const string name1 = "user name 1";
            const string name2 = "user name 2";
            claimsIdentity.AddClaim(new Claim(ClaimType.Name, name1));
            claimsIdentity.AddClaim(new Claim(ClaimType.Name, name2));

            IReadOnlyCollection<string> claims = sut.GetClaims(ClaimType.Name);

            Assert.AreEqual(2, claims.Count);
            Assert.AreEqual(name1, claims.First());
            Assert.AreEqual(name2, claims.Last());
        }

        [TestMethod]
        [ExpectedException(typeof(ClaimNotFoundException))]
        public void MultipleClaimsNotDefined()
        {
            sut.GetClaims(ClaimType.Name);
        }
    }
}