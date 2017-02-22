using System;
using System.IO;
using Newtonsoft.Json;

namespace TfsBuildNotifier.Services
{
    public class ConfigService : IConfigService
    {
        private readonly string _path;

        public ConfigService(string path)
        {
            _path = path;
        }

        public string ReadValue(string key)
        {
            if (string.IsNullOrEmpty(_path))
            {
                throw new ArgumentNullException(nameof(_path));
            }

            if (!File.Exists(_path))
            {
                throw new Exception("Could not locate config file.");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var data = File.ReadAllText(_path);
            dynamic config = JsonConvert.DeserializeObject(data);

            return config[key].ToString();
        }
    }
}