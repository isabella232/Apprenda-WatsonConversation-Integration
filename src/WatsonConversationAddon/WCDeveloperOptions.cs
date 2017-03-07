using Apprenda.SaaSGrid.Addons;
using Apprenda.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apprenda.WatsonConversation.Addon
{
    class WCDeveloperOptions
    {
        public string user { get; set; }
        public string pass { get; set; }
        public string tenant { get; set; }
        public string workspace { get; set; }
        public string conversationusername { get; set; }
        public string conversationpassword { get; set; }
        public string cloudurl { get; set; }
        public string name { get; set; }
        public string alias { get; set; }
        public string developeralias { get; set; }
        public string developerid { get; set; }
        public string instancealias { get; set; }

        private static readonly ILogger log = LogManager.Instance().GetLogger(typeof(WatsonConversationAddon));

        private static void MapToOption(WCDeveloperOptions _options, string _key, string _value)
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

        public static WCDeveloperOptions Parse(IEnumerable<AddonParameter> _developerParameters, AddonManifest manifest)
        {
            var options = new WCDeveloperOptions();
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
