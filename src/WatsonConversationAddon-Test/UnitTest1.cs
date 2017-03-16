using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apprenda.WatsonConversation.Addon;

namespace BluemixAddon_Test
{
    [TestClass]
    public class BluemixAddon_Test
    {
        [TestMethod]
        public string authenticate()
        {
            WatsonConversationAddon test = new WatsonConversationAddon();
            var token = test.authenticate("", "", "");
            System.Diagnostics.Debug.WriteLine(token);
            return token;
        }

        [TestMethod]
        public void createArchive()
        {
            WatsonConversationAddon test = new WatsonConversationAddon();
            var archivePath = test.createArchive("", "", "");
            System.Diagnostics.Debug.WriteLine(archivePath);
        }

        [TestMethod]
        public void createApp()
        {
            WatsonConversationAddon test = new WatsonConversationAddon();
            var token = authenticate();
            var resp = test.createApp(token, "watson-api", "watsonapi", "watson test api");
            System.Diagnostics.Debug.WriteLine(resp);
        }

        [TestMethod]
        public void setArchive()
        {
            WatsonConversationAddon test = new WatsonConversationAddon();
            var token = authenticate();
            var resp = test.setArchive(token, "watsonapi", "v1", @"DockerArchive\archive.zip");
            System.Diagnostics.Debug.WriteLine(resp);
        }

        [TestMethod]
        public void promoteApp()
        {
            WatsonConversationAddon test = new WatsonConversationAddon();
            var token = authenticate();
            var resp = test.promoteApp(token, "watsonapi", "v1");
            System.Diagnostics.Debug.WriteLine(resp);
        }

        [TestMethod]
        public void getAppURL()
        {
            WatsonConversationAddon test = new WatsonConversationAddon();
            var token = authenticate();
            var resp = test.getAppURL(token, "watsonapi", "v1");
            System.Diagnostics.Debug.WriteLine(resp);
        }


        [TestMethod]
        public void provision()
        {
            WatsonConversationAddon test = new WatsonConversationAddon();
            var token = authenticate();
            var archivePath = test.createArchive("", "", "");
            test.createApp(token, "watson-api", "watsonapi", "watson test api");
            test.setArchive(token, "watsonapi", "v1", archivePath);
            test.promoteApp(token, "watsonapi", "v1");
            var url = test.getAppURL(token, "watsonapi", "v1");
            System.Diagnostics.Debug.WriteLine(url);
        }
    }
}
