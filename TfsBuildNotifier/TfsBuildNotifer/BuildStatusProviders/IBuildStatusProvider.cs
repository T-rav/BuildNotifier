using TfsBuildNotifier.ValueObjects;

namespace TfsBuildNotifier.BuildStatusProviders
{
    public interface IBuildStatusProvider
    {
        BuildState FetchBuildStatusFromHttp(string endPoint);
    }
}