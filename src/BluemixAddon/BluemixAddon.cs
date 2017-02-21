using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apprenda.SaaSGrid.Addons;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Apprenda.Services.Logging;

namespace Apprenda.Bluemix.AddOn
{
    public class BluemixAddon : AddonBase
    {
        string end_point;
        string bm_version;

        private static readonly ILogger log = LogManager.Instance().GetLogger(typeof(BluemixAddon));

        public override OperationResult Deprovision(AddonDeprovisionRequest _request)
        {
            /*The Deprovision method allows you to specify the steps taken when a developer deprovisions his/her Add-On. 
             * You should use this step to clean up any provisioned artifacts. The connectiondata object inside the request should
             * have all the information needed to clean up the provisioned resource. 
             * At the end, you can return whether the operation was successful or not as shown in the sample below*/

            var deprovisionResult = new OperationResult { EndUserMessage = string.Empty, IsSuccess = false };

            var manifest = _request.Manifest;
            var devParameters = _request.DeveloperParameters;
            var devOptions = BMDeveloperOptions.Parse(devParameters, manifest);

            end_point = devOptions.api_url;
            bm_version = devOptions.api_version;

            try
            {
                var token = authenticate(devOptions.user, devOptions.pass);
                string name = devOptions.name;
                var serviceGUID = getServiceGUID(token, name);
                var status = deleteServiceInstance(token, name, serviceGUID);
                log.Info("BluemixAddon Deprovisioned Successfully");
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("BluemixAddon Deprovision Error: {0}\n{1}", ex, ex.StackTrace);
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
            var devOptions = BMDeveloperOptions.Parse(devParameters, manifest);

            end_point = devOptions.api_url;
            bm_version = devOptions.api_version;

            JObject instanceDetails = new JObject();

            try
            {
                var token = authenticate(devOptions.user, devOptions.pass);
                var servicePlansURL = getServicePlansURL(token, devOptions.servicename); //"watson_vision_combined"
                var servicePlanGUID = getServicePlanGUID(token, servicePlansURL);
                string name = devOptions.name;
                var spaceGUID = getSpaceGuid(token, devOptions.space); //"dev"
                var serviceInstanceGUID = createServiceInstance(token, name, servicePlanGUID, spaceGUID);
                instanceDetails = createInstanceDetails(token, name, serviceInstanceGUID);
                log.Info("BluemixAddon Provisioned Successfully");
            }
            catch (Exception ex) {
                var errorMessage = string.Format("BluemixAddon Provisioning Error: {0}\n{1}", ex, ex.StackTrace);
                log.Error(errorMessage);
                provisionResult.EndUserMessage = errorMessage;
                return provisionResult;
            }

            provisionResult.IsSuccess = true;
            provisionResult.ConnectionData = instanceDetails.ToString(Formatting.None);
            return provisionResult;
        }

        public override OperationResult Test(AddonTestRequest _request)
        {
            /*The test method allows you to test whether the Add-On was developed and configured properly and that any dependent systems are
             operating normally. During this method, you will want to go through a similar workflow to Provision to ensure proper functioning
             * of the Add-On.*/
            var manifest = _request.Manifest;
            var devParameters = _request.DeveloperParameters;
            var devOptions = BMDeveloperOptions.Parse(devParameters, manifest);
            var testResult = new OperationResult { EndUserMessage = string.Empty, IsSuccess = false };

            end_point = devOptions.api_url;
            bm_version = devOptions.api_version;

            try
            {
                authenticate(devOptions.user, devOptions.pass);
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("BluemixAddon Testing Error: {0}\n{1}", ex, ex.StackTrace);
                log.Error(errorMessage);
                testResult.EndUserMessage = errorMessage;
                return testResult;
            }

            testResult.EndUserMessage = "BluemixAddon Add-On was tested successfully";
            testResult.IsSuccess = true;
            return testResult;
        }

        public string authenticate(string user, string pass)
        {
            var username = Uri.EscapeDataString(user);
            var password = Uri.EscapeDataString(pass);
            var client = new RestClient("https://login.ng.bluemix.net/UAALoginServerWAR/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", "Basic Y2Y6");
            var authString = string.Format("grant_type=password&username={0}&password={1}", username, password);
            request.AddParameter("application/x-www-form-urlencoded", authString, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var json_response = JsonConvert.DeserializeObject<dynamic>(response.Content);
            var token = json_response.access_token;
            return token;
        }

        public string getServicePlansURL(string token, string name)
        {
            var url = string.Format("{0}/{1}/services?q=label:{2}", end_point, bm_version, name);
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", "bearer " + token);
            IRestResponse response = client.Execute(request);
            var responseObject = JsonConvert.DeserializeObject<dynamic>(response.Content);
            var responseArray = responseObject.resources;
            var service_plans_url = responseArray[0].entity.service_plans_url;
            return service_plans_url;
        }

        public string getServicePlanGUID(string token, string service_plans_url)
        {
            var url = string.Format("{0}{1}", end_point, service_plans_url);
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", "bearer " + token);
            IRestResponse response = client.Execute(request);
            var responseObject = JsonConvert.DeserializeObject<dynamic>(response.Content);
            return responseObject.resources[0].metadata.guid;
        }

        public string getServiceGUID(string token, string name)
        {
            var url = string.Format("{0}/{1}/service_instances?q=name:{2}", end_point, bm_version, name);
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", "bearer " + token);
            IRestResponse response = client.Execute(request);
            var responseObject = JsonConvert.DeserializeObject<dynamic>(response.Content);
            return responseObject.resources[0].metadata.guid;
        }

        public string createServiceInstance(string token, string name, string servicePlanGUID, string spaceGUID)
        {
            var url = string.Format("{0}/{1}/service_instances", end_point, bm_version);
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);

            dynamic body = new JObject();
            body.name = name;
            body.space_guid = spaceGUID;
            body.service_plan_guid = servicePlanGUID;
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            try
            {
                IRestResponse response = client.Execute(request);
                log.Info("Service instance created successfully");
                var responseObject = JsonConvert.DeserializeObject<dynamic>(response.Content);
                return responseObject.metadata.guid;
            }
            catch (Exception ex)
            {
                log.Info("Error creating service instance: " + ex);
                return null;
            }

        }

        public string getSpaceGuid(string token, string name)
        {
            var url = string.Format("{0}/{1}/spaces?q=name:{2}", end_point, bm_version, name);
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", "bearer " + token);
            IRestResponse response = client.Execute(request);
            var responseObject = JsonConvert.DeserializeObject<dynamic>(response.Content);
            var responseArray = responseObject.resources;
            return responseArray[responseArray.Count - 1].metadata.guid;
        }

        public JObject createInstanceDetails(string token, string name, string serviceInstanceGUID)
        {
            var url = string.Format("{0}/{1}/service_keys", end_point, bm_version);
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", "bearer " + token);

            dynamic body = new JObject();
            body.name = name;
            body.service_instance_guid = serviceInstanceGUID;
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            try
            {
                IRestResponse response = client.Execute(request);
                var responseObject = JsonConvert.DeserializeObject<dynamic>(response.Content);
                return responseObject.entity.credentials;
            }catch(Exception ex)
            {
                log.Info("Error requesting instance details: " + ex);
                return null;
            }
        }

        public string deleteServiceInstance(string token, string name, string serviceInstanceGUID)
        {
            var url = string.Format("{0}/{1}/service_instances/{2}?recursive=true&async=true", end_point, bm_version, serviceInstanceGUID);
            var client = new RestClient(url);
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("authorization", "bearer " + token);

            try
            {
                IRestResponse response = client.Execute(request);
                log.Info("Service instance deleted successfully");
                return response.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                log.Info("Error creating service instance: " + ex);
                return null;
            }

        }

        public string flattenJSON(JObject json)
        {
            var flattenedJSON = string.Empty;
            foreach(var item in json)
            {
                flattenedJSON = flattenedJSON + ';' + item;
            }
            return flattenedJSON;
        }

    }
}
