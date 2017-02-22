using System;
using NUnit.Framework;
using TfsBuildNotifier.Services;

namespace TfsTrayNotification.Tests.Services
{
    [TestFixture]
    public class ConfigServiceTests
    {
        [Test]
        public void ReadValue_WhenValidConfigurationPath_ShouldReturnExpectedValue()
        {
            //---------------Set up test pack-------------------
            var expected = "http://tfs/buildNotifier/builds.json";
            var path = "BuildData/App.json";
            var key = "buildIdProviderUrl";
            var reader = new ConfigService(path);

            //---------------Execute Test ----------------------
            var result = reader.ReadValue(key);

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ReadValue_WhenInvalidConfigurationPath_ShouldThrowException()
        {
            //---------------Set up test pack-------------------
            var path = "BuildData/App2.json";
            var key = "buildIdProviderUrl";
            var reader = new ConfigService(path);

            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
            Assert.Throws<Exception>(()=>reader.ReadValue(key));
            
        }

        [Test]
        public void ReadValue_WhenNullConfigurationPath_ShouldThrowANE()
        {
            //---------------Set up test pack-------------------
            var key = "buildIdProviderUrl";
            var reader = new ConfigService(null);

            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
            Assert.Throws<ArgumentNullException>(()=>reader.ReadValue(key)); 
        }

        [TestCase(null)]
        [TestCase("")]
        public void ReadValue_WhenNullOrEmptyKey_ShouldThrowANE(string key)
        {
            //---------------Set up test pack-------------------
            var path = "BuildData/App.json";
            var reader = new ConfigService(path);

            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
            Assert.Throws<ArgumentNullException>(()=>reader.ReadValue(key));
        }
    }
}