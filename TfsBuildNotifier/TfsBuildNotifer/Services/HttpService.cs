using System;
using System.IO;
using System.Net;
using log4net;

namespace TfsBuildNotifier.Services
{
    public class HttpService : IHttpService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (HttpService));

        public string FetchDataFrom(string uri)
        {
            Log.Info("Fetching from [ " + uri + " ]");
            try
            {
                var request = WebRequest.Create(uri);
                request.UseDefaultCredentials = true;

                var response = request.GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                return string.Empty;
            }
        }

    }
}