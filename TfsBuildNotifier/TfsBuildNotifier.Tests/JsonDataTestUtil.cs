using System.IO;
using NUnit.Framework;

namespace TfsTrayNotification.Tests
{
    public static class JsonDataTestUtil
    {
        public static string FetchBuildData(string fileName, int buildId)
        {
            var rootDir = TestContext.CurrentContext.TestDirectory;
            var fileToFetch = Path.Combine(rootDir, "BuildData", fileName);
            var data = File.ReadAllText(fileToFetch);
            var result = data.Replace("\"id\": 9999", "\"id\":" + buildId);
            return result;
        }
    }
}