# Watson Conversation Integration

The Apprenda Watson Conversation Integration allows developers to consume their Watson Conversaton Bluemix Service from within Apprenda. The Add-on creates a Watson Conversation API App (nodeJS) within Apprenda that allows any Watson Chat client to interact with your Watson Workspace in Bluemix

### Release Notes

#### v1.0
  * Initial Release
  * Deploys a nodeJS Conversation API within Apprenda

## Installation
1. Clone the repository.
2. Build the Apprenda Bluemix Add-On.
3. Package your Add-On by simply zipping up your AddOnManifest.xml, icon.png and compiled assemblies. 
4. Upload to your System Operations Center (SOC). -> http://docs.apprenda.com/current/addons

### Developer Parameters.

These are the properties that the developer will specify. The below table outlines the developer parameters that can be used to provision an instance of the addon. This version of the integration only supports developer parameters as it was intended to be used by consumers of the Apprenda Cloud Platform (public alpha) running on Bluemix. 

| Name (Alias) | Description | Example | 
| ------------ | ----------- | ------- |
| Workspace ID | Your Watson Conversation ID which can be found by clicking "View Details"  | hex formatted string | 
| Conversation Username | Your Watson Conversation Service Credentials. If using the Apprenda-Bluemix-Integration, this is what is returned as a connection string | hex formatted string |
| Conversation Password | Your Watson Conversation Service Credentials. If using the Apprenda-Bluemix-Integration, this is what is returned as a connection string  | hex formatted string |
| Apprenda Cloud URL | The Apprenda Cloud URL that you are deploying on. This will match the url you see while editing entering this information | https://apps.myapprendaurl.com |
| Application Name | Tells Apprenda how to name your Watson Conversation API App. | My Chat App |
| Application Alias | Tells Apprenda how to reference your Watson Conversation API App| mychatapp |
| Apprenda Username | These credentials will determine how the API is deployed in Apprenda | user@gmail.com |
| Apprenda Password | These credentials will determine how the API is deployed in Apprenda | password |

![](/readme_images/watsonconversation_provision.png)

