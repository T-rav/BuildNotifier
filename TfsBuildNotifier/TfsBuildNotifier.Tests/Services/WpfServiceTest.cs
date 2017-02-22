using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using TfsBuildNotifier.DataContext;
using TfsBuildNotifier.Services;
using Hardcodet.Wpf.TaskbarNotification;
using TfsBuildNotifier.ValueObjects;

namespace TfsTrayNotification.Tests.Services
{
    [TestFixture]
    public class WpfServiceTest
    {
        private static WpfService CreateWpfService()
        {
            StaticMessageHolder.Message = new BuildMessage();
            var notifyMock = Substitute.For<TaskbarIcon>();
            var sut = new WpfService(notifyMock);
            return sut;
        }

        [Test]
        [STAThread]
        public void AdjustFirstBuildFlag_WhenTrue_ExpectFalse()
        {
            //---------------Set up test pack-------------------
            var notifyMock = Substitute.For<TaskbarIcon>();
            var sut = new WpfService(notifyMock);
            StaticMessageHolder.IsFirstBuild = true;
            //---------------Assert Precondition----------------
            Assert.IsTrue(StaticMessageHolder.IsFirstBuild);
            //---------------Execute Test ----------------------
            sut.AdjustFirstBuildFlag();
            //---------------Test Result -----------------------
            Assert.IsFalse(StaticMessageHolder.IsFirstBuild);
        }

        [Test]
        [STAThread]
        public void AdjustFirstBuildFlag_WhenFalse_ExpectFalse()
        {
            //---------------Set up test pack-------------------
            var notifyMock = Substitute.For<TaskbarIcon>();
            var sut = new WpfService(notifyMock);
            StaticMessageHolder.IsFirstBuild = false;
            //---------------Assert Precondition----------------
            Assert.IsFalse(StaticMessageHolder.IsFirstBuild);
            //---------------Execute Test ----------------------
            sut.AdjustFirstBuildFlag();
            //---------------Test Result -----------------------
            Assert.IsFalse(StaticMessageHolder.IsFirstBuild);

        }

        [Test]
        [STAThread]
        public void ShowBalloonTip_NotifyIconShowBalloonTipCalled()
        {
            //---------------Set up test pack-------------------
            var notifyMock = Substitute.For<TaskbarIcon>();
            var sut = new WpfService(notifyMock);
            //---------------Execute Test ----------------------
            sut.ShowBalloonTip("foo", "bar", BalloonIcon.Info);
            //---------------Test Result -----------------------
            notifyMock.Received(1).ShowBalloonTip(Arg.Any<string>(), Arg.Any<string>(), BalloonIcon.Info);
        }

        [Test]
        [STAThread]
        public void SetWindowMessage_ExpectBuildMessageUpdated()
        {
            //---------------Set up test pack-------------------
            var sut = CreateWpfService();
            StaticMessageHolder.Message = new BuildMessage {BuildServerMessage = "bar"};
            //---------------Execute Test ----------------------
            sut.SetWindowMessage("foo");
            //---------------Test Result -----------------------
            Assert.AreEqual("foo",StaticMessageHolder.Message.BuildServerMessage);
        }

        [Test]
        [STAThread]
        public void IsFirstBuild_WhenFirstBuildTrue_ExpectTrue()
        {
            //---------------Set up test pack-------------------
            var sut = CreateWpfService();
            StaticMessageHolder.IsFirstBuild = true;
            //---------------Execute Test ----------------------
            var result = sut.IsFirstBuildState();
            //---------------Test Result -----------------------
            Assert.IsTrue(result);
        }

        [Test]
        [STAThread]
        public void IsFirstBuild_WhenFirstBuildFalse_ExpectFalse()
        {
            //---------------Set up test pack-------------------
            var sut = CreateWpfService();
            StaticMessageHolder.IsFirstBuild = false;
            //---------------Execute Test ----------------------
            var result = sut.IsFirstBuildState();
            //---------------Test Result -----------------------
            Assert.IsFalse(result);
        }

        [Test]
        [STAThread]
        public void MakeBadBuildString_WhenPopulatedList_ShouldReturnStringBuilderWithData()
        {
            //---------------Set up test pack-------------------
            const string expected = "1] Test1\r\n2] Test2\r\n";
            var badBuilds = new List<BuildStatusDef>
            {
                new BuildStatusDef {BuildId = 1, BuildName = "Test1", BuildStatus = BuildState.Red},
                new BuildStatusDef {BuildId = 2, BuildName = "Test2", BuildStatus = BuildState.Red}
            };
            var sut = CreateWpfService();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = sut.MakeBadBuildString(badBuilds);
            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [STAThread]
        public void MakeBadBuildString_WhenNullList_ShouldThrowANE()
        {
            //---------------Set up test pack-------------------
            var sut = CreateWpfService();
            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
            Assert.Throws<ArgumentNullException>(()=>sut.MakeBadBuildString(null));
            
        }

        [TestCase(BuildState.Red)]
        [TestCase(BuildState.Green)]
        [STAThread]
        public void FetchCurrentBuildState_ShouldReturnBuildState(BuildState state)
        {
            //---------------Set up test pack-------------------
            var sut = CreateWpfService();
            StaticMessageHolder.CurrentState = state;
            //---------------Execute Test ----------------------
            var result = sut.FetchCurrentBuildState();
            //---------------Test Result -----------------------
            Assert.AreEqual(state, result);
        }

        [Test]
        [STAThread]
        public void SetGreenState_ExpectGreenStateSet()
        {
            //---------------Set up test pack-------------------
            var sut = CreateWpfService();
            StaticMessageHolder.CurrentState = BuildState.Red;
            //---------------Execute Test ----------------------
            sut.SetGreenState();
            //---------------Test Result -----------------------
            Assert.AreEqual(BuildState.Green, StaticMessageHolder.CurrentState);
        }

        [Test]
        [STAThread]
        public void SetRedState_ExpectRedStateSet()
        {
            //---------------Set up test pack-------------------
            var sut = CreateWpfService();
            var badBuilds = new List<BuildStatusDef>
            {
                new BuildStatusDef {BuildId = 1, BuildName = "Test1", BuildStatus = BuildState.Red},
                new BuildStatusDef {BuildId = 2, BuildName = "Test2", BuildStatus = BuildState.Red}
            };
            StaticMessageHolder.CurrentState = BuildState.Green;
            //---------------Execute Test ----------------------
            sut.SetRedState(badBuilds);
            //---------------Test Result -----------------------
            Assert.AreEqual(BuildState.Red, StaticMessageHolder.CurrentState);
        }

        [Test]
        [STAThread]
        public void SetRedToGreenState_ExpectGreenStateSet()
        {
            //---------------Set up test pack-------------------
            var sut = CreateWpfService();
            StaticMessageHolder.CurrentState = BuildState.Red;
            //---------------Execute Test ----------------------
            sut.SetRedToGreenState();
            //---------------Test Result -----------------------
            Assert.AreEqual(BuildState.Green, StaticMessageHolder.CurrentState);
        }
    }
}