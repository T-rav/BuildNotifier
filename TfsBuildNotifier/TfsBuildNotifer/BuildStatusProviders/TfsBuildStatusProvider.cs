using System;
using log4net;
using Newtonsoft.Json;
using TfsBuildNotifier.Services;
using TfsBuildNotifier.ValueObjects;

namespace TfsBuildNotifier.BuildStatusProviders
{
    public class TfsBuildStatusProvider : IBuildStatusProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TfsBuildStatusProvider));

        private readonly IHttpService _httpService;

        public TfsBuildStatusProvider(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public BuildState FetchBuildStatusFromHttp(string endPoint)
        {
            if (_httpService == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            Log.Info("Fetching TFS Build Status");

            var httpData = _httpService.FetchDataFrom(endPoint);

            var tfsTestResult = ExtractTfsBuildResult(httpData);

            Log.Info("TFS Build Status is [ " + tfsTestResult + " ]");

            return tfsTestResult == "succeeded" ? BuildState.Green : BuildState.Red;

        }

        private string ExtractTfsBuildResult(string httpData)
        {
            try
            {
                dynamic tfsResultObject = JsonConvert.DeserializeObject(httpData);
                var tfsResultObjectValuePaylod = tfsResultObject.value;
                var tfsTestResult = tfsResultObjectValuePaylod[0].result;
                return tfsTestResult.ToString();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return "undefined";
            }
        }
    }
}