using System;
using System.Collections;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace VcpkgBuildTask.Tests
{
    [TestClass]
    public class InstallTaskTests
    {
        [TestMethod]
        public void Defaults()
        {
            var instance = new InstallTask();

            Assert.AreEqual(10 * 60 * 1000, instance.Timeout);
            Assert.IsNotNull(instance.VcpkgExe);
        }

        [TestMethod]
        public void GenerateFullPathToTool()
        {
            var instance = new InstallTask();

            var method = typeof(InstallTask).GetMethod("GenerateFullPathToTool", BindingFlags.Instance | BindingFlags.NonPublic);

            var defaultPath = instance.VcpkgExe;
            Assert.AreEqual(defaultPath, method.Invoke(instance, null));

            var modifiedPath = "test123";
            instance.VcpkgExe = modifiedPath;
            Assert.AreEqual(modifiedPath, method.Invoke(instance, null));
        }

        [TestMethod]
        public void GenerateCommandLineCommands_Basic()
        {
            var instance = new InstallTask();

            var method = typeof(InstallTask).GetMethod("GenerateCommandLineCommands", BindingFlags.Instance | BindingFlags.NonPublic);
            
            // with no Packages set, we throw.
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                try
                {
                    method.Invoke(instance, null);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            });

            instance.VcpkgRoot = "test";
            instance.Packages = new string[] { "gtest" };

            Assert.AreEqual("install gtest --vcpkg-root test", method.Invoke(instance, null));

            instance.Packages = new string[] { "gtest", "angle:x86-pie" };

            Assert.AreEqual("install gtest angle:x86-pie --vcpkg-root test", method.Invoke(instance, null));
        }

        [TestMethod]
        public void GenerateCommandLineCommands_DefaultTriplet()
        {
            var instance = new InstallTask();
            instance.VcpkgRoot = "test";
            instance.VcpkgTriplet = "x86-pie";
            instance.Packages = new string[] { "gtest" };

            var method = typeof(InstallTask).GetMethod("GenerateCommandLineCommands", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.AreEqual("install gtest:x86-pie --vcpkg-root test", method.Invoke(instance, null));
        }

        [TestMethod]
        public void GenerateCommandLineCommands_InvalidTriplet()
        {
            var instance = new InstallTask();

            var method = typeof(InstallTask).GetMethod("GenerateCommandLineCommands", BindingFlags.Instance | BindingFlags.NonPublic);

            var moqEngine = new Mock<IBuildEngine>();
            moqEngine.Setup(m => m.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()));

            instance.BuildEngine = moqEngine.Object;
            instance.Packages = new string[] { "gtest:invalid" };

            method.Invoke(instance, null);

            moqEngine.Verify(m => m.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()), Times.Once());
        }
    }
}
