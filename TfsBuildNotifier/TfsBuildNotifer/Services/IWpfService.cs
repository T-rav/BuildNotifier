using System.Collections.Generic;
using Hardcodet.Wpf.TaskbarNotification;
using TfsBuildNotifier.ValueObjects;

namespace TfsBuildNotifier.Services
{
    public interface IWpfService
    {
        void SetRedState(IEnumerable<BuildStatusDef> badBuilds);
        void SetGreenState();
        void SetRedToGreenState();
        BuildState FetchCurrentBuildState();
        bool IsFirstBuildState();
        void SetWindowMessage(string message);
        void ShowBalloonTip(string title, string message, BalloonIcon icon);
        void SetTrayIcon(string icon);
        void AdjustFirstBuildFlag();
    }
}