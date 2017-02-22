using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using TfsBuildNotifier.DataContext;
using TfsBuildNotifier.Services;
using TfsBuildNotifier.ValueObjects;

namespace TfsTrayNotification.Tests.Services
{
    [TestFixture]
    public class DispatchedFetchTest
    {

        private static object _serialLock = new object();

        [Test]
        public void FetchBuildList_WhenValidPayload_ShouldDeserialize()
        {
            //---------------Set up test pack-------------------
            var httpService = ConfigureHttpServiceWithBuildIds("");
            var configService = Substitute.For<IConfigService>();
            var wpfService = Substitute.For<IWpfService>();

            var sut = new DispatchedFetch(configService, httpService, wpfService);
            var expected = new BuildDefList {BuildDefs = new List<BuildDef>()};
            expected.BuildDefs.Add(new BuildDef { Id = 2, Name = "All Solutions"});
            expected.BuildDefs.Add(new BuildDef { Id = 5, Name = "Report Engine" });

            //---------------Execute Test ----------------------
            var result = sut.FetchBuildIdList();

            //---------------Test Result -----------------------
            Assert.AreEqual(2, result.BuildDefs.Count);
            Assert.AreEqual(expected.BuildDefs[0].Id, result.BuildDefs[0].Id);
            Assert.AreEqual(expected.BuildDefs[0].Name, result.BuildDefs[0].Name);
            Assert.AreEqual(expected.BuildDefs[1].Id, result.BuildDefs[1].Id);
            Assert.AreEqual(expected.BuildDefs[1].Name, result.BuildDefs[1].Name);
        }

        [Test]
        public void FetchData_WhenGreenStatuses_ShouldReturnGreenStatus()
        {
            //---------------Set up test pack-------------------
            var buildDefUrl = "http://foo.com/builds.json";
            var httpService = ConfigureHttpServiceWithBuildIds(buildDefUrl);

            // setup build data for each id
            ConfigureBuildServiceWithBuildGreenPayloads(httpService);
            var configService = Substitute.For<IConfigService>();
            configService.ReadValue("buildIdProviderUrl").Returns(buildDefUrl);
            configService.ReadValue("buildStatusProviderUrlTemplate").Returns("http://foo.com/tfs/{0}");
            var wpfService = Substitute.For<IWpfService>();

            var sut = new DispatchedFetch(configService, httpService, wpfService);
            //---------------Execute Test ----------------------
            var result = sut.FetchData();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[0].BuildId);
            Assert.AreEqual(BuildState.Green, result[0].BuildStatus);
            Assert.AreEqual(5, result[1].BuildId);
            Assert.AreEqual(BuildState.Green, result[1].BuildStatus);

        }

        [Test]
        public void FetchData_WhenRedStatuses_ShouldReturnRedStatus()
        {
            //---------------Set up test pack-------------------
            var buildDefUrl = "http://foo.com/builds.json";
            var httpService = ConfigureHttpServiceWithBuildIds(buildDefUrl);

            ConfigureBuildServiceWithBuildRedPayloads(httpService);
            var configService = Substitute.For<IConfigService>();
            configService.ReadValue("buildIdProviderUrl").Returns(buildDefUrl);
            configService.ReadValue("buildStatusProviderUrlTemplate").Returns("http://foo.com/tfs/{0}");
            var wpfService = Substitute.For<IWpfService>();
            
            var sut = new DispatchedFetch(configService, httpService, wpfService);
            //---------------Execute Test ----------------------
            var result = sut.FetchData();
            //---------------Test Result -----------------------
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[0].BuildId);
            Assert.AreEqual(BuildState.Red, result[0].BuildStatus);
            Assert.AreEqual(5, result[1].BuildId);
            Assert.AreEqual(BuildState.Red, result[1].BuildStatus);

        }

        [Test]
        public void SetBuildStatus_WhenRedBuildStatues_ShouldIndidateRedState()
        {
            //---------------Set up test pack-------------------
            var buildDefUrl = "http://foo.com/builds.json";
            var httpService = ConfigureHttpServiceWithBuildIds(buildDefUrl);

            ConfigureBuildServiceWithBuildRedPayloads(httpService);
            var configService = Substitute.For<IConfigService>();
            configService.ReadValue("buildIdProviderUrl").Returns(buildDefUrl);
            configService.ReadValue("buildStatusProviderUrlTemplate").Returns("http://foo.com/tfs/{0}");
            var wpfService = Substitute.For<IWpfService>();
                
            var sut = new DispatchedFetch(configService, httpService, wpfService);
            //---------------Execute Test ----------------------
            sut.SetBuildStatus();
            //---------------Test Result -----------------------
            wpfService.Received(1).SetRedState(Arg.Any<IEnumerable<BuildStatusDef>>());
        }

        [Test]
        public void SetBuildStatus_WhenRedToGreenBuildStatues_ShouldIndidateGreenState()
        {

            //---------------Set up test pack-------------------
            var buildDefUrl = "http://foo.com/builds.json";
            var httpService = ConfigureHttpServiceWithBuildIds(buildDefUrl);

            ConfigureBuildServiceWithBuildGreenPayloads(httpService);
            var configService = Substitute.For<IConfigService>();
            configService.ReadValue("buildIdProviderUrl").Returns(buildDefUrl);
            configService.ReadValue("buildStatusProviderUrlTemplate").Returns("http://foo.com/tfs/{0}");
            var wpfService = Substitute.For<IWpfService>();
            wpfService.FetchCurrentBuildState().Returns(BuildState.Red);
            wpfService.IsFirstBuildState().Returns(false);

            var sut = new DispatchedFetch(configService, httpService, wpfService);
            //---------------Execute Test ----------------------
            sut.SetBuildStatus();
            //---------------Test Result -----------------------
            wpfService.Received(1).SetRedToGreenState();
        }

        [Test]
        public void SetBuildStatus_WhenGreenBuildStatues_ShouldIndidateGreenState()
        {
            //---------------Set up test pack-------------------
            var buildDefUrl = "http://foo.com/builds.json";
            var httpService = ConfigureHttpServiceWithBuildIds(buildDefUrl);

            ConfigureBuildServiceWithBuildGreenPayloads(httpService);
            var configService = Substitute.For<IConfigService>();
            configService.ReadValue("buildIdProviderUrl").Returns(buildDefUrl);
            configService.ReadValue("buildStatusProviderUrlTemplate").Returns("http://foo.com/tfs/{0}");
            var wpfService = Substitute.For<IWpfService>();
            wpfService.FetchCurrentBuildState().Returns(BuildState.Green);
            wpfService.IsFirstBuildState().Returns(false);

            var sut = new DispatchedFetch(configService, httpService, wpfService);
            //---------------Execute Test ----------------------
            sut.SetBuildStatus();
            //---------------Test Result -----------------------
            wpfService.DidNotReceive().SetRedToGreenState();
            wpfService.Received(1).SetGreenState();
        }

        private static IHttpService ConfigureHttpServiceWithBuildIds(string buildDefUrl)
        {
            var httpService = Substitute.For<IHttpService>();
            httpService.FetchDataFrom(buildDefUrl)
                .Returns(
                    "{ BuildDefs: [ { \"name\": \"All Solutions\", \"id\": 2}, { \"name\": \"Report Engine\", \"id\": 5} ]}");
            return httpService;
        }

        private static void ConfigureBuildServiceWithBuildGreenPayloads(IHttpService httpService)
        {
            var id2Payload = JsonDataTestUtil.FetchBuildData("GoodData.json", 2);
            var id5Payload = JsonDataTestUtil.FetchBuildData("GoodData.json", 5);
            httpService.FetchDataFrom("http://foo.com/tfs/2").Returns(id2Payload);
            httpService.FetchDataFrom("http://foo.com/tfs/5").Returns(id5Payload);
        }

        private static void ConfigureBuildServiceWithBuildRedPayloads(IHttpService httpService)
        {
            var id2Payload = JsonDataTestUtil.FetchBuildData("BadData.json", 2);
            var id5Payload = JsonDataTestUtil.FetchBuildData("BadData.json", 5);
            httpService.FetchDataFrom("http://foo.com/tfs/2").Returns(id2Payload);
            httpService.FetchDataFrom("http://foo.com/tfs/5").Returns(id5Payload);
        }
    }
}