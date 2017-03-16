# Apprenda Watson Conversation Add-on

The Apprenda Watson Conversation Add-on allows developers to consume their Watson Conversaton Bluemix Service from within Apprenda. 
The Add-on creates a Watson Conversation API App (nodeJS) within Apprenda that allows any Watson Chat client to interact with your Watson Workspace in Bluemix

## Installation

1. Clone the repository.
2. Build the Apprenda Watson Conversation Add-On.
3. Package your Add-On by simply zipping up your AddOnManifest.xml, icon.png and compiled assemblies. 
4. Upload to your System Operations Center (SOC). -> http://docs.apprenda.com/current/addons


## Usage
1. In the SOC, go to "Configuration->Platform Addons->Edit"
2. Enter your Apprenda credentials on the general page. This account will deploy the Watson API on Apprenda. 


![](/readme_images/watsonconversation_general.png)


3. On the configuration tab, enter your Apprenda Tenant. 
4. Add the full Apprenda Cloud URL (ie. "https://apps.myapprendaurl.com") 


![](/readme_images/watsonconversation_config.png)


5. Next, go to the Apprenda Developer Portal and click Addons on the left side.
6. Click on the Bluemix Add-on and click the "+" symbol to provision an instance of the Add-on
    a. Enter in the "Instance Alias" (how Apprenda will identify your add-on instance)
    b. Enter the "Workspace ID" as defined within your Conversation Service. 
    c. Enter the "Conversation Username" and "Conversation Password" as defined within the "Service Credentials" in your Conversation Service.
    d. Enter the "Application Name" and "Application Alias" which will tell Apprenda how to name and reference your Watson Conversation API App.
    
7. If you'd like to check if your app deployed successfully, go back to the Add-ons section in the SOC and click the test button (only enter the "Application Alias"). 


![](/readme_images/watsonconversation_provision.png)
