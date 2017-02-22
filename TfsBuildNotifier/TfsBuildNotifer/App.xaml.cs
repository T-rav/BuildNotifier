using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using log4net;
using TfsBuildNotifier.DataContext;
using TfsBuildNotifier.Services;

namespace TfsBuildNotifier
{
    /// <summary>
    /// Simple application. Check the XAML for comments.
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(App));

        private DispatcherTimer _dispatcherTimer;
        private TaskbarIcon _notifyIcon;
        private DispatchedFetch _fetchProc;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon) FindResource("NotifyIcon");

            StaticMessageHolder.Message = new BuildMessage { BuildServerMessage = "Looking into build status, hang tight guy !" };

            _fetchProc = new DispatchedFetch(new ConfigService("App.json"), new HttpService(), new WpfService(_notifyIcon));

            InitDispatchTimer(_fetchProc);
            FetchInitalBuildStatus(_fetchProc);
        }

        private void FetchInitalBuildStatus(DispatchedFetch dispatchedFetch)
        {
            Task.Factory.StartNew(dispatchedFetch.SetBuildStatus);
        }

        private void InitDispatchTimer(DispatchedFetch dispatchedFetch)
        {
            Log.Info("Starting Application Dispatch Timer");

            StaticMessageHolder.IsFirstBuild = true;

            var intMinutes = ReadPollIntervalMinutes();
            _dispatcherTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, intMinutes, 0)};
            
            _dispatcherTimer.Tick += (sender, args) =>
            {
                dispatchedFetch.SetBuildStatus();
            };

            _dispatcherTimer.Start();
        }

        private static int ReadPollIntervalMinutes()
        {
            try
            {
                var config = new ConfigService("App.json");
                var minutes = config.ReadValue("pollIntervalInMinutes");
                var intMinutes = int.Parse(minutes);
                return intMinutes;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return 15;
            }
        }
        
        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            _dispatcherTimer.Stop();
            base.OnExit(e);
        }

        public void SetBuildStatus()
        {
            _fetchProc.SetBuildStatus();
        }
    }
}
