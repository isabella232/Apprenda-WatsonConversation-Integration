using Apprenda.SaaSGrid.Addons;
using Apprenda.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apprenda.Bluemix.AddOn
{
    class BMDeveloperOptions
    {
        public string user { get; set; }
        public string pass { get; set; }
        public string space { get; set; }
        public string api_url { get; set; }
        public string api_version { get; set; }
        public string servicename { get; set; }
        public string name { get; set; }
        public string developeralias { get; set; }
        public string developerid { get; set; }

        private static readonly ILogger log = LogManager.Instance().GetLogger(typeof(BluemixAddon));

        private static void MapToOption(BMDeveloperOptions _options, string _key, string _value)
        {
            var _developerOptions = _options.GetType().GetProperties();

            foreach(var prop in _developerOptions)
            {
                if (prop.Name.Equals(_key))
                {
                    prop.SetValue(_options, _value);
                    return;
                }
            }

            log.Error(string.Format("The developer option '{0}' was not expected.", _key));
            return;
        }

        public static BMDeveloperOptions Parse(IEnumerable<AddonParameter> _developerParameters, AddonManifest manifest)
        {
            var options = new BMDeveloperOptions();
            foreach (var parameter in manifest.Properties)
            {
                MapToOption(options, parameter.Key.ToLowerInvariant(), parameter.Value);
            }
            foreach (var parameter in _developerParameters)
            {
                MapToOption(options, parameter.Key.ToLowerInvariant(), parameter.Value);
            }
            return options;
        }
    }
}
