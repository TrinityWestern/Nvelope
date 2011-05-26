using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.Configuration;
using System.Configuration;

namespace Nvelope.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationManagerTests
    {
        [SetUp]
        [TearDown]
        public void Cleanup()
        {
            Environment.SetEnvironmentVariable(Config.DEPLOYMENT_ENV_VAR, null);
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
            Environment.SetEnvironmentVariable(Config.DEPLOYMENT_ENV_VAR, 
                DeploymentLocation.Live.ToString(), 
                EnvironmentVariableTarget.Process);

            Assert.AreEqual(DeploymentLocation.Live, Config.Location);

            Environment.SetEnvironmentVariable(Config.DEPLOYMENT_ENV_VAR,
                DeploymentLocation.Dev.ToString(),
                EnvironmentVariableTarget.Process);

            Assert.AreEqual(DeploymentLocation.Dev, Config.Location);

            Environment.SetEnvironmentVariable(Config.DEPLOYMENT_ENV_VAR,
                DeploymentLocation.Local.ToString(),
                EnvironmentVariableTarget.Process);

            Assert.AreEqual(DeploymentLocation.Local, Config.Location);
        }

        [Test]
        public void LocationizedNames()
        {
            var live  = Config._getLocationizedNames(DeploymentLocation.Live, "name");
            var dev   = Config._getLocationizedNames(DeploymentLocation.Dev, "name");
            var local = Config._getLocationizedNames(DeploymentLocation.Local, "name");
            Assert.AreEqual("name-live", live.First());
            Assert.AreEqual("name", live.Second());
            Assert.AreEqual("name-dev", dev.First());
            Assert.AreEqual("name-dev", local.Second());
            Assert.AreEqual("name-local", local.First());
        }

        [Test]
        public void ConnectionStrings()
        {
            Assert.IsFalse(Config.HasConnectionString("this should really not be set"));
        }
    }
}
