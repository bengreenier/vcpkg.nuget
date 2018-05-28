using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VcpkgBuildTask.Tests
{
    [TestClass]
    public class VcpkgPackagesTests
    {
        [TestMethod]
        public void DefaultValues()
        {
            var instance = new VcpkgPackages();

            Assert.AreEqual(instance.TimeoutMs, 10 * 60 * 1000);
            Assert.IsTrue(!string.IsNullOrEmpty(instance.VcpkgExe));
            Assert.IsTrue(string.IsNullOrEmpty(instance.VcpkgRoot));
            Assert.IsNull(instance.Packages);
        }
    }
}
