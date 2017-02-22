using System;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using TfsBuildNotifier.BuildStatusProviders;
using TfsBuildNotifier.Services;
using TfsBuildNotifier.ValueObjects;

namespace TfsTrayNotification.Tests.BuildStatusProviders
{
    [TestFixture]
    public class TfsBuildStatusProviderTests
    {
        private static IHttpService CreateMockHttpService(string callData)
        {
            var httpService = Substitute.For<IHttpService>();
            httpService.FetchDataFrom(Arg.Any<string>()).Returns(callData);
            return httpService;
        }

        [Test]
        public void Ctor_ShouldNotThrowANE()
        {
            //---------------Set up test pack-------------------
            var httpService = Substitute.For<IHttpService>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.DoesNotThrow(()=>new TfsBuildStatusProvider(httpService));
        }

        [Test]
        public void Fetch_WhenNullHttpServiceInjected_ShouldThrowANE()
        {
            //---------------Set up test pack-------------------
            var tfsBuildStatusProvider = new TfsBuildStatusProvider(null);
            var uri = "http://tfs";
            
            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
            Assert.Throws<ArgumentNullException>(()=>tfsBuildStatusProvider.FetchBuildStatusFromHttp(uri));
        }

        [Test]
        public void Fetch_WhenPassingBuildData_ShouldGreenBuildState()
        {
            //---------------Set up test pack-------------------

            var expected = BuildState.Green;
            var callData = JsonDataTestUtil.FetchBuildData("GoodData.json", 19);
            var httpService = CreateMockHttpService(callData);
            var tfsBuildStatusProvider = new TfsBuildStatusProvider(httpService);
            var uri = "http://tfs";
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            var result = tfsBuildStatusProvider.FetchBuildStatusFromHttp(uri);
            //---------------Test Result -----------------------

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Fetch_WhenFailingBuildData_ShouldRedBuildState()
        {
            //---------------Set up test pack-------------------

            var expected = BuildState.Red;
            var callData = JsonDataTestUtil.FetchBuildData("BadData.json",2);
            var httpService = CreateMockHttpService(callData);
            var tfsBuildStatusProvider = new TfsBuildStatusProvider(httpService);
            var uri = "http://tfs";
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            var result = tfsBuildStatusProvider.FetchBuildStatusFromHttp(uri);
            //---------------Test Result -----------------------

            Assert.AreEqual(expected, result);
        }
    }
}