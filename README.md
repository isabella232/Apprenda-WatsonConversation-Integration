# Apprenda Watson Conversation Add-on

The Apprenda Watson Conversation Add-on allows developers to consume their Watson Conversaton Bluemix Service from within Apprenda. 
The Add-on creates a Watson Conversation API App (nodeJS) within Apprenda that allows any Watson Chat client to interact with your Watson Workspace in Bluemix

## Installation

1. Clone the repository.
2. Build the Apprenda Watson Conversation Add-On.
3. Package your Add-On by simply zipping up your AddOnManifest.xml, icon.png and compiled assemblies. 
4. Upload to your System Operations Center (SOC). -> http://docs.apprenda.com/current/addons


## Usage
1. Go to the Apprenda Developer Portal and click Addons on the left side.
2. Click on the Bluemix Add-on and click the "+" symbol to provision an instance of the Add-on
    1. "Instance Alias" (how Apprenda will identify your add-on instance)
    2. "Workspace ID" as defined within your Conversation Service. 
    3. "Conversation Username" and "Conversation Password" as defined within the "Service Credentials" in your Bluemix Conversation Service.
    4. Apprenda Cloud URL (ie. "https://apps.myapprendaurl.com") 
    5. Enter the "Application Name" and "Application Alias" which will tell Apprenda how to name and reference your Watson Conversation API App.
    6. Enter your "Apprenda Username", "Apprenda Password" and "Apprenda Tenant". These credentials will determine how the API is deployed in Apprenda.
    
    ![](/readme_images/watsonconversation_provision.png)

7. If you'd like to check if your app deployed successfully, go back to the Add-ons section in the SOC and click the test button (only enter the "Application Alias"). 
