using System;
using Apprenda.SaaSGrid.Addons;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Apprenda.Services.Logging;
using System.Xml;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace Apprenda.WatsonConversation.Addon
{
    public class WatsonConversationAddon : AddonBase
    {
        private string cloud_url = "";
        private string auth_url = "/authentication/api/v1/sessions/developer";
        private string api_url = "/developer/api/v1";
        private string appname;
        private string appalias;
        string valias = "v1";

        private static readonly ILogger log = LogManager.Instance().GetLogger(typeof(WatsonConversationAddon));

        public override OperationResult Deprovision(AddonDeprovisionRequest _request)
        {
            /*The Deprovision method allows you to specify the steps taken when a developer deprovisions his/her Add-On. 
             * You should use this step to clean up any provisioned artifacts. The connectiondata object inside the request should
             * have all the information needed to clean up the provisioned resource. 
             * At the end, you can return whether the operation was successful or not as shown in the sample below*/

            var deprovisionResult = new OperationResult { EndUserMessage = string.Empty, IsSuccess = false };

            var manifest = _request.Manifest;
            var devParameters = _request.DeveloperParameters;
            var devOptions = WCDeveloperOptions.Parse(devParameters, manifest);

            cloud_url = devOptions.cloudurl;

            try
            {
                var token = authenticate(devOptions.apprendausername, devOptions.apprendapassword, devOptions.apprendatenant);
                int status = deleteApp(token, devOptions.alias);
                if (status == 204)
                {
                    log.Info("WatsonConversationAddon Deprovisioned Successfully");
                }else
                {
                    throw new Exception("Failed to deprovision");
                }
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("WatsonConversationAddon Deprovision Error: {0}\n{1}", ex, ex.StackTrace);
                log.Error(errorMessage);
                deprovisionResult.EndUserMessage = errorMessage;
                return deprovisionResult;
            }

            deprovisionResult.IsSuccess = true;
            return deprovisionResult;
        }

        public override ProvisionAddOnResult Provision(AddonProvisionRequest _request)
        {
            /*The Provision method provisions the instance for the service you are setting up. Within this method, you can access the information
             requested from the developer (if any) by iterating through the request.DeveloperParameters object.
             * At the end of the provisioning process, simply return the connection information for the service that was provisioned.*/
            //Retrieving developer parameters
            //var parameter = request.DeveloperParameters.First(param => param.Key == "RequiredParameter");

            var provisionResult = new ProvisionAddOnResult(string.Empty) { IsSuccess = false };

            var manifest = _request.Manifest;
            var devParameters = _request.DeveloperParameters;
            var devOptions = WCDeveloperOptions.Parse(devParameters, manifest);
            var appURL = "";

            cloud_url = devOptions.cloudurl;
            appname = devOptions.name;
            appalias = devOptions.alias;

            JObject instanceDetails = new JObject();

            try
            {
                var token = authenticate(devOptions.apprendausername, devOptions.apprendapassword, devOptions.apprendatenant);
                createApp(token, appname, appalias, "NodeJS API Server for Watson Conversation");
                var archivePath = createArchive(devOptions.workspace, devOptions.conversationusername, devOptions.conversationpassword);
                setArchive(token, appalias, valias, archivePath);
                promoteApp(token, appalias, valias);
                appURL = getAppURL(token, appalias, valias);
                log.Info("WatsonConversationAddon Provisioned Successfully");
            }
            catch (Exception ex) {
                var errorMessage = string.Format("WatsonConversationAddon Provisioning Error: {0}\n{1}", ex, ex.StackTrace);
                log.Error(errorMessage);
                provisionResult.EndUserMessage = errorMessage;
                return provisionResult;
            }

            provisionResult.IsSuccess = true;
            provisionResult.ConnectionData = appURL;
            return provisionResult;
        }

        public override OperationResult Test(AddonTestRequest _request)
        {
            /*The test method allows you to test whether the Add-On was developed and configured properly and that any dependent systems are
             operating normally. During this method, you will want to go through a similar workflow to Provision to ensure proper functioning
             * of the Add-On.*/
            var manifest = _request.Manifest;
            var devParameters = _request.DeveloperParameters;
            var devOptions = WCDeveloperOptions.Parse(devParameters, manifest);
            var testResult = new OperationResult { EndUserMessage = string.Empty, IsSuccess = false };

            cloud_url = devOptions.cloudurl;
            appalias = devOptions.alias;

            try
            {
                var token = authenticate(devOptions.apprendausername, devOptions.apprendapassword, devOptions.apprendatenant);
                int statusCode = getApp(token, appalias);
                if (statusCode!=200)
                {
                    throw new Exception("Watson Conversation API has not successfully been deployed in Apprenda");
                }
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("WatsonConversationAddon Testing Error: {0}\n{1}", ex, ex.StackTrace);
                log.Error(errorMessage);
                testResult.EndUserMessage = errorMessage;
                return testResult;
            }

            testResult.EndUserMessage = "WatsonConversationAddon Add-On was tested successfully";
            testResult.IsSuccess = true;
            return testResult;
        }

        public string authenticate(string user, string pass, string tenant)
        {

            var client = new RestClient(cloud_url + auth_url);
            var request = new RestRequest(Method.POST);

            dynamic body = new JObject();
            body.username = user;
            body.password = pass;
            body.tenantAlias = tenant;
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            try
            {
                IRestResponse response = client.Execute(request);
                var json_response = JsonConvert.DeserializeObject<dynamic>(response.Content);
                var token = json_response.apprendaSessionToken;
                return token;
            }catch(Exception ex)
            {
                log.Info("Error authenticating: " + ex);
                throw;
            }
        }

        public string createApp(string token, string name, string alias, string desc)
        {

            var url = string.Format("{0}{1}/apps", cloud_url, api_url);

            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);

            request.AddHeader("ApprendaSessionToken", token);
            request.AddHeader("Content-Type", "application/json");

            dynamic body = new JObject();
            body.Name = name;
            body.Alias = alias;
            body.Description = desc;
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            try
            {
                IRestResponse response = client.Execute(request);
                return response.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                log.Info("Error creating application: " + ex);
                throw;
            }
        }

        public int deleteApp(string token, string alias)
        {

            var url = string.Format("{0}{1}/apps/{2}", cloud_url, api_url, alias);

            var client = new RestClient(url);
            var request = new RestRequest(Method.DELETE);

            request.AddHeader("ApprendaSessionToken", token);
            request.AddHeader("Content-Type", "application/json");

            try
            {
                IRestResponse response = client.Execute(request);
                return (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                log.Info("Error deleting application: " + ex);
                throw;
            }
        }

        public string createArchive(string workspace, string username, string password)
        {

            try
            {
                string path = Directory.GetCurrentDirectory();
                string[] dirs = Directory.GetDirectories(path, "executionCache*");
                var addonBasePath = "";

                if (dirs.Length != 0)
                {
                    addonBasePath = dirs[0];
                }

                var manifest = addonBasePath + @"\DockerArchive\DeploymentManifest.xml";
                var archive = addonBasePath + @"\DockerArchive\archive.zip";
                var linuxServices = addonBasePath + @"\DockerArchive\linuxServices";

                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(manifest);
                XmlNamespaceManager ns = new XmlNamespaceManager(xdoc.NameTable);
                ns.AddNamespace("app", "http://schemas.apprenda.com/DeploymentManifest");

                string envPath = "/app:appManifest/app:linuxServices/app:service[@name=\"docker\"]/app:environmentVariables";

                if (xdoc != null)
                {
                    xdoc.SelectSingleNode(envPath + "/app:variable[@name='WORKSPACE_ID']", ns).Attributes["value"].Value = workspace;
                    xdoc.SelectSingleNode(envPath + "/app:variable[@name='CONVERSATION_USERNAME']", ns).Attributes["value"].Value = username;
                    xdoc.SelectSingleNode(envPath + "/app:variable[@name='CONVERSATION_PASSWORD']", ns).Attributes["value"].Value = password;
                }

                xdoc.Save(manifest);

                if (File.Exists(archive))
                {
                    File.Delete(archive);
                }

                ZipFile.CreateFromDirectory(linuxServices, archive, CompressionLevel.Fastest, true);
                using (ZipArchive zip = ZipFile.Open(archive, ZipArchiveMode.Update))
                {
                    zip.CreateEntryFromFile(manifest, "DeploymentManifest.xml");
                }

                return archive;
            }
            catch (Exception ex)
            {
                log.Info("Error creating archive: " + ex);
                throw;
            }
        }

        public string setArchive(string token, string alias, string valias, string archivePath)
        {
            var url = string.Format("{0}{1}/versions/{2}/{3}/?action=setArchive", cloud_url, api_url, alias, valias);

            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);

            request.AddHeader("ApprendaSessionToken", token);
            request.AddHeader("Content-Type", "application/json");
            request.AddFile("archive.zip", archivePath);

            try
            {
                IRestResponse response = client.Execute(request);
                return response.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                log.Info("Error creating application: " + ex);
                throw;
            }
        }

        public string promoteApp(string token, string alias, string valias)
        {
            var url = string.Format("{0}{1}/versions/{2}/{3}/?action=promote&stage=Published", cloud_url, api_url, alias, valias);

            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);

            request.AddHeader("ApprendaSessionToken", token);
            request.AddHeader("Content-Type", "application/json");

            try
            {
                IRestResponse response = client.Execute(request);
                return response.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                log.Info("Error promoting application: " + ex);
                throw;
            }
        }

        public string getAppURL(string token, string alias, string valias)
        {
            var url = string.Format("{0}{1}/versions/{2}/{3}", cloud_url, api_url, alias, valias);

            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);

            request.AddHeader("ApprendaSessionToken", token);
            request.AddHeader("Content-Type", "application/json");

            try
            {
                IRestResponse response = client.Execute(request);
                var json_response = JsonConvert.DeserializeObject<dynamic>(response.Content);
                return json_response.url;
            }
            catch (Exception ex)
            {
                log.Info("Error getting application URL: " + ex);
                throw;
            }
        }

        public int getApp(string token, string alias)
        {
            var url = string.Format("{0}{1}/apps/{2}", cloud_url, api_url, alias);

            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);

            request.AddHeader("ApprendaSessionToken", token);
            request.AddHeader("Content-Type", "application/json");

            try
            {
                IRestResponse response = client.Execute(request);
                return (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                log.Info("Error getting application URL: " + ex);
                throw;
            }
        }
    }
}
