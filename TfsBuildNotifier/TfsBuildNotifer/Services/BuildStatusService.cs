using TfsBuildNotifier.BuildStatusProviders;
using TfsBuildNotifier.ExtensionMethods;
using TfsBuildNotifier.ValueObjects;

namespace TfsBuildNotifier.Services
{
    public class BuildStatusService
    {
        private readonly IBuildStatusProvider _buildStatusProvider;

        public BuildStatusService(IBuildStatusProvider provider)
        {
            _buildStatusProvider = provider;
        }

        public BuildState FetchBuildStatusFromHttp(string endPoint)
        {
            endPoint.ValidateHttpEndPoint();
            return _buildStatusProvider.FetchBuildStatusFromHttp(endPoint);
        }
    }
}
