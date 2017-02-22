using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Hardcodet.Wpf.TaskbarNotification;
using TfsBuildNotifier.DataContext;
using TfsBuildNotifier.ValueObjects;

namespace TfsBuildNotifier.Services
{
    public class WpfService : IWpfService
    {
        private readonly TaskbarIcon _notifyIcon;

        public WpfService(TaskbarIcon notifyIcon)
        {
            _notifyIcon = notifyIcon;
        }

        public void SetRedState(IEnumerable<BuildStatusDef> badBuilds)
        {
            StaticMessageHolder.CurrentState = BuildState.Red;
            SetTrayIcon("Red.ico");
            _notifyIcon.ShowBalloonTip("Broken Builds", "One or more builds have gone red! Please fix.", BalloonIcon.Error);
            var buildString = MakeBadBuildString(badBuilds);
            SetWindowMessage("Please fix the following builds : " + Environment.NewLine + buildString);
        }

        public StringBuilder MakeBadBuildString(IEnumerable<BuildStatusDef> badBuilds)
        {
            if (badBuilds == null)
            {
                throw new ArgumentNullException(nameof(badBuilds));
            }

            var buildString = new StringBuilder();
            var cnt = 1;
            foreach (var build in badBuilds)
            {
                buildString.Append(cnt + "] ");
                buildString.Append(build.BuildName);
                buildString.Append(Environment.NewLine);
                cnt++;
            }
            return buildString;
        }

        public void SetGreenState()
        {
            StaticMessageHolder.CurrentState = BuildState.Green;
            SetWindowMessage("All good buddy. Keep on coding you code ninja!");
            SetTrayIcon("Green.ico");
        }

        public void SetRedToGreenState()
        {
            StaticMessageHolder.CurrentState = BuildState.Green;
            SetTrayIcon("Green.ico");
            ShowBalloonTip("Fixed Builds", "Builds back to green. Keep on coding!", BalloonIcon.Info);
            SetWindowMessage("All good. The builds are fixed. Keep on coding you code ninja!");
        }

        public BuildState FetchCurrentBuildState()
        {
            return StaticMessageHolder.CurrentState;
        }

        public bool IsFirstBuildState()
        {
            return StaticMessageHolder.IsFirstBuild;
        }

        public void SetWindowMessage(string message)
        {
            StaticMessageHolder.Message.BuildServerMessage = message;
        }

        public void ShowBalloonTip(string title, string message, BalloonIcon icon)
        {
            _notifyIcon.ShowBalloonTip(title, message, icon);
        }
        
        public void SetTrayIcon(string icon)
        {
            _notifyIcon.Icon = new Icon("RuntimeIcons/" + icon);
        }

        public void AdjustFirstBuildFlag()
        {
            if (StaticMessageHolder.IsFirstBuild)
            {
                StaticMessageHolder.IsFirstBuild = false;
            }
        }
    }
}