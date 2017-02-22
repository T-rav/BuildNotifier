using System.Collections.Generic;
using System.Linq;
using Hardcodet.Wpf.TaskbarNotification;
using log4net;
using Newtonsoft.Json;
using TfsBuildNotifier.BuildStatusProviders;
using TfsBuildNotifier.ValueObjects;

namespace TfsBuildNotifier.Services
{
    public class DispatchedFetch
    {
        private readonly IConfigService _configService;
        private readonly IHttpService _httpService;
        private readonly IWpfService _wpfService;
        private static readonly object StatusLock = new object();

        private static readonly ILog Log = LogManager.GetLogger(typeof(DispatchedFetch));

        public DispatchedFetch(IConfigService configService, IHttpService httpService, IWpfService wpfService)
        {
            _configService = configService;
            _httpService = httpService;
            _wpfService = wpfService;
        }

        public void SetBuildStatus()
        {
            lock (StatusLock)
            {
                Log.Info("Fetching Build Status");
                var result = FetchData();
                if (IsBuildStateRed(result))
                {
                    Log.Info("Status is RED");
                    var buildStatusDefs = result.Where(x=>x.BuildStatus == BuildState.Red);
                    _wpfService.SetRedState(buildStatusDefs);
                }
                else
                {
                    if (IsRedToGreenBuildState())
                    {
                        Log.Info("Status is RED to GREEN");
                        _wpfService.SetRedToGreenState();
                    }
                    else
                    {
                        Log.Info("Status is GREEN");
                        _wpfService.SetGreenState();
                    }
                }
                _wpfService.AdjustFirstBuildFlag();
            }
        }

        public List<BuildStatusDef> FetchData()
        {
            var result = new List<BuildStatusDef>();
            var buildDefList = FetchBuildIdList();

            if (buildDefList == null)
            {
                return result;
            }

            var buildStatusService = CreateBuildStatusService();
            var template = _configService.ReadValue("buildStatusProviderUrlTemplate");
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var buildDef in buildDefList.BuildDefs)
            {
                var url = string.Format(template, buildDef.Id);
                var buildState = buildStatusService.FetchBuildStatusFromHttp(url);
                result.Add(new BuildStatusDef {BuildId = buildDef.Id, BuildName = buildDef.Name, BuildStatus = buildState});
            }

            return result;
        }

        public BuildDefList FetchBuildIdList()
        {
            var buildIdProvider = _configService.ReadValue("buildIdProviderUrl");
            var data = _httpService.FetchDataFrom(buildIdProvider);
            return JsonConvert.DeserializeObject<BuildDefList>(data);
        }

        private BuildStatusService CreateBuildStatusService()
        {
            var buildStatusProvider = new TfsBuildStatusProvider(_httpService);
            var buildStatusService = new BuildStatusService(buildStatusProvider);
            return buildStatusService;
        }

        private bool IsBuildStateRed(IEnumerable<BuildStatusDef> result)
        {
            return result.Any(r => r.BuildStatus == BuildState.Red);
        }

        private bool IsRedToGreenBuildState()
        {
            return _wpfService.FetchCurrentBuildState() == BuildState.Red && !_wpfService.IsFirstBuildState();
        }
    }

}