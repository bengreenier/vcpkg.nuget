using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VcpkgBuildTask.Tests
{
    [TestClass]
    public class PackageInfoTests
    {
        [TestMethod]
        public void ParseShort()
        {
            var instance = new PackageInfo("gtest");
            Assert.AreEqual("gtest", instance.Name);
            Assert.IsNull(instance.Platform);
            Assert.IsNull(instance.Architecture);
            Assert.IsNull(instance.Linkage);
            Assert.IsNull(instance.Triplet);
        }

        [TestMethod]
        public void ParseTriplet_Failure()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var instance = new PackageInfo("gtest:nothing");
            });
        }

        [TestMethod]
        public void ParseFull()
        {
            var instance = new PackageInfo("gtest:x86-pie");
            Assert.AreEqual("gtest", instance.Name);
            Assert.AreEqual("x86", instance.Platform);
            Assert.AreEqual("pie", instance.Architecture);
            Assert.AreEqual("x86-pie", instance.Triplet);
        }

        [TestMethod]
        public void ParseFullLinkage()
        {
            var instance = new PackageInfo("gtest:x86-pie-static");
            Assert.AreEqual("gtest", instance.Name);
            Assert.AreEqual("x86", instance.Platform);
            Assert.AreEqual("pie", instance.Architecture);
            Assert.AreEqual("static", instance.Linkage);
            Assert.AreEqual("x86-pie-static", instance.Triplet);
        }
    }
}
