# Apprenda Add-On Sample Template
Empty Add-On template to help jump start your Add-On development!

The following repository is meant to help and provide Add-On developers with a ready to use template alongside the required Apprenda dependencies (version 6.5.1). 

Additional information about creating Add-Ons can be found here: 
* Understanding Add-Ons - http://docs.apprenda.com/current/addons
* Building a custom Add-On - http://docs.apprenda.com/6-5/addoncreation

# How to Use?
1. Clone the repository.
2. Build your Add-On.
3. Package your Add-On by simply zipping up your AddOnManifest.xml, icon.png and compiled assemblies. 
4. Upload to your System Operations Center (SOC). -> http://docs.apprenda.com/current/addons

# Modifying the Apprenda Target Version
This BSP is packaged with the DLL for Apprenda Version 6.5.1. If you wish to use a different Apprenda version, simply replace the SaaSGrid.API.dll file in the lib folder with the desired version installed from your SDK. 