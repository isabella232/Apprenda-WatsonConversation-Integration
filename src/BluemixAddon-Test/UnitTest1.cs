using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apprenda.Bluemix.AddOn;

namespace BluemixAddon_Test
{
    [TestClass]
    public class BluemixAddon_Test
    {
        [TestMethod]
        public void Provision()
        {
            BluemixAddon test = new BluemixAddon();
            var token = test.authenticate("cdutra%40apprenda.com","Apprenda2016!");
            var servicePlansURL = test.getServicePlansURL(token, "conversation");//"watson_vision_combined");
            var servicePlanGUID = test.getServicePlanGUID(token, servicePlansURL);
            //TODO PARAMETERIZE
            string name = "sample_watson_dialog";
            var spaceGUID = test.getSpaceGuid(token, "dev");
            var serviceInstanceGUID = test.createServiceInstance(token, name, servicePlanGUID, spaceGUID);
            var instanceDetails = test.createInstanceDetails(token, name, serviceInstanceGUID);
            System.Diagnostics.Debug.WriteLine(instanceDetails.ToString());
        }

        [TestMethod]
        public void Deprovision()
        {
            BluemixAddon test = new BluemixAddon();
            var token = test.authenticate("cdutra%40apprenda.com", "Apprenda2016!");
            //TODO PARAMETERIZE
            var name = "sample_watson_dialog";
            var serviceGUID = test.getServiceGUID(token, name);
            var status = test.deleteServiceInstance(token, name, serviceGUID);
        }
        
    }
}
