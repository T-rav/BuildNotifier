using System;
using NUnit.Framework;
using TfsBuildNotifier.ExtensionMethods;

namespace TfsTrayNotification.Tests
{
    public class StringUtilTests
    {
        [Test]
        public void ValidateHttpEndpoint_WhenValid_ExpectNoException()
        {
            //---------------Set up test pack-------------------
            var uri = "http://tfs";
            //---------------Test Result -----------------------
            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(()=>uri.ValidateHttpEndPoint());
            
        }

        [Test]
        public void ValidateHttpEndpoint_WhenEmptyString_ExpectException()
        {
            //---------------Set up test pack-------------------
            var uri = "";
            //---------------Test Result -----------------------
            //---------------Execute Test ----------------------
            Assert.Throws<Exception>(() => uri.ValidateHttpEndPoint());

        }

        [Test]
        public void ValidateHttpEndpoint_WhenMalformedHttpUri_ExpectException()
        {
            //---------------Set up test pack-------------------
            var uri = "http//tfs";
            //---------------Test Result -----------------------
            //---------------Execute Test ----------------------
            Assert.Throws<Exception>(() => uri.ValidateHttpEndPoint());

        }
    }
}