using System;
using NSubstitute;
using NUnit.Framework;
using TfsBuildNotifier.BuildStatusProviders;
using TfsBuildNotifier.Services;
using TfsBuildNotifier.ValueObjects;

namespace TfsTrayNotification.Tests.Services
{
    [TestFixture]
    public class BuildStatusServiceTests
    {
        private static IBuildStatusProvider CreateTfsBuildStatusProvider()
        {
            var tfsProvider = Substitute.For<IBuildStatusProvider>();
            return tfsProvider;
        }

        private static BuildStatusService CreateBuildStatus(BuildState expectedState)
        {
            var tfsProvider = CreateTfsBuildStatusProvider();
            tfsProvider.FetchBuildStatusFromHttp(Arg.Any<string>()).Returns(expectedState);

            var sut = new BuildStatusService(tfsProvider);
            return sut;
        }

        [Test]
        public void Ctor_ShouldNotThrowANE()
        {
            //---------------Set up test pack-------------------
            var tfsProvider = CreateTfsBuildStatusProvider();
            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
            Assert.DoesNotThrow(()=>new BuildStatusService(tfsProvider));
        }

        [Test]
        public void FetchBuildStatusFromHttp_WhenKnownGoodBuilds_ShouldReturnGreenStatus()
        {
            //---------------Set up test pack-------------------
            var expected = BuildState.Green;
            var sut = CreateBuildStatus(BuildState.Green);
            var tfsUrl = "http://tfs:8080/tfs";
            

            //---------------Execute Test ----------------------
            var result = sut.FetchBuildStatusFromHttp(tfsUrl);

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void FetchBuildStatusFromHttp_WhenKnownBadBuilds_ShouldReturnRedStatus()
        {
            //---------------Set up test pack-------------------
            var expected = BuildState.Red;
            var tfsUrl = "http://tfs:8080/tfs";
            var sut = CreateBuildStatus(BuildState.Red);

            //---------------Execute Test ----------------------
            var result = sut.FetchBuildStatusFromHttp(tfsUrl);

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void FetchBuildStatusFromHttpWhenNonWellFormedTfsUrl_ShouldThrowException()
        {
            //---------------Set up test pack-------------------
            var tfsUrl = "tfs:8080/tfs";
            var tfsProvider = CreateTfsBuildStatusProvider();
            var sut = new BuildStatusService(tfsProvider);

            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
            Assert.Throws<Exception>(()=>sut.FetchBuildStatusFromHttp(tfsUrl));
            
        }
    }
}
