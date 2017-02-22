using System;

namespace TfsBuildNotifier.ExtensionMethods
{
    public static class StringUtils
    {
        public static void ValidateHttpEndPoint(this string val)
        {
            if (!val.StartsWith("http://"))
            {
                throw new Exception("Malformed TFS Url [ " + val + " ]");
            }
        }
    }
}