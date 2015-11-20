using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Affecto.Authentication.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Authentication.Claims.Tests
{
    [TestClass]
    public class ClaimExtensionsTests
    {
        private List<Claim> sut;

        [TestInitialize]
        public void Setup()
        {
            sut = new List<Claim>();
        }

        [TestMethod]
        public void GetSingleClaimValueWhenThereAreNoClaimValues()
        {
            Assert.IsNull(sut.GetSingleClaimValue(ClaimType.AccountName));
        }

        [TestMethod]
        public void GetSingleClaimValueWhenThereIsNoClaimValueWithCorrectType()
        {
            sut.Add(new Claim(ClaimType.Group, "group A"));

            Assert.IsNull(sut.GetSingleClaimValue(ClaimType.AccountName));
        }

        [TestMethod]
        public void GetSingleClaimValueWhenThereIsOneClaimValueWithCorrectType()
        {
            const string groupName = "group A";
            sut.Add(new Claim(ClaimType.Group, groupName));
            sut.Add(new Claim(ClaimType.Name, "user"));
            
            Assert.AreEqual(groupName, sut.GetSingleClaimValue(ClaimType.Group));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetSingleClaimValueWhenThereAreMultipleClaimValuesWithCorrectType()
        {
            sut.Add(new Claim(ClaimType.Group, "group A"));
            sut.Add(new Claim(ClaimType.Group, "group B"));

            sut.GetSingleClaimValue(ClaimType.Group);
        }

        [TestMethod]
        public void GetClaimValuesWhenThereAreNoClaimValues()
        {
            Assert.IsFalse(sut.GetClaimValues(ClaimType.AccountName).Any());
        }

        [TestMethod]
        public void GetClaimValuesWhenThereIsNoClaimValueWithCorrectType()
        {
            sut.Add(new Claim(ClaimType.Group, "group A"));

            Assert.IsFalse(sut.GetClaimValues(ClaimType.AccountName).Any());
        }

        [TestMethod]
        public void GetClaimValueWhenThereAreMultipleClaimValuesWithCorrectType()
        {
            const string firstGroupName = "group A";
            const string secondGroupName = "group B";

            sut.Add(new Claim(ClaimType.Group, firstGroupName));
            sut.Add(new Claim(ClaimType.Group, secondGroupName));
            sut.Add(new Claim(ClaimType.Name, "user"));

            IReadOnlyCollection<string> claimValues = sut.GetClaimValues(ClaimType.Group);

            Assert.AreEqual(2, claimValues.Count);
            Assert.IsTrue(claimValues.Contains(firstGroupName));
            Assert.IsTrue(claimValues.Contains(secondGroupName));
        }
    }
}