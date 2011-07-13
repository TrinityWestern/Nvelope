using System;
using System.Configuration;
using NUnit.Framework;
using Nvelope.Configuration;

namespace Nvelope.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationManagerTests
    {
        [SetUp]
        [TearDown]
        public void Cleanup()
        {
            Environment.SetEnvironmentVariable(Config.DeploymentEnvironmentVariable, null);
        }

        [Test]
        public void IsLive()
        {
            // TODO: this test works in our environment, maybe, but it doesn't
            // seem like a ver generic test
            Assert.IsFalse(Config.Location == DeploymentLocation.Live);
        }
        [Test]
        public void Settings()
        {
            var village = "Wurbleydorf";
            ConfigurationManager.AppSettings["TestVillage"] = village;
            Assert.AreEqual(village, Config.Setting("TestVillage"));
            Assert.IsTrue(Config.HasSetting("TestVillage"));
            ConfigurationManager.AppSettings["unset"] = null;
            Assert.IsFalse(Config.HasSetting("unset"));
            Assert.AreEqual(village, Config.Setting("unset", village));
        }

        [Test]
        public void ReadsEnvironmentVariable()
        {
            // check basic settings
            ConfigurationManager.AppSettings["TestSetting"] = "Base";
            Assert.AreEqual("Base", Config.Setting("TestSetting"));

            // preconfigure settings for different environments
            ConfigurationManager.AppSettings["TestSetting-live"] = "Live";
            ConfigurationManager.AppSettings["TestSetting-dev"] = "Dev";
            ConfigurationManager.AppSettings["TestSetting-local"] = "Local";

            // check live settings
            Environment.SetEnvironmentVariable(
                Config.DeploymentEnvironmentVariable, 
                DeploymentLocation.Live.ToString(),
                EnvironmentVariableTarget.Process);

            Assert.AreEqual("Live", Config.Setting("TestSetting"));
            Assert.AreEqual(DeploymentLocation.Live, Config.Location);

            // check dev settings
            Environment.SetEnvironmentVariable(
                Config.DeploymentEnvironmentVariable,
                DeploymentLocation.Dev.ToString(),
                EnvironmentVariableTarget.Process);

            Assert.AreEqual("Dev", Config.Setting("TestSetting"));
            Assert.AreEqual(DeploymentLocation.Dev, Config.Location);

            Environment.SetEnvironmentVariable(
                Config.DeploymentEnvironmentVariable,
                DeploymentLocation.Local.ToString(),
                EnvironmentVariableTarget.Process);

            Assert.AreEqual("Local", Config.Setting("TestSetting"));
            Assert.AreEqual(DeploymentLocation.Local, Config.Location);
        }

        [Test]
        public void ConnectionStrings()
        {
            Assert.IsFalse(Config.HasConnectionString("this should really not be set"));
        }
    }
}
