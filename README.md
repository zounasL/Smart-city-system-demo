# Azure Functions
1) Install Azurite for local storage emulation: https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage
2) Setup Azure IoT Hub in Azure if IoT Hub messages are wanted to be tested
3) Setup Custom Vision if image detection is wanted to be tested
4) Setup blob and message queu into the Azure if Azurite is not used
5) Setup Cosmos DB. Cosmos DB can be emulated locally: https://learn.microsoft.com/en-us/azure/cosmos-db/emulator
5) Add Azure IoT Hub, Custom Vision, Cosmos DB, Blob storage and Queue connection strings to the `local.settings.json`.

# Unity Digital Twin
1) Install Unity and Editor version 2021.3.24f1
2) Setup ArcGIS Cloud and install ArcGIS-SDK: https://developers.arcgis.com/unity/get-started/
3) Import the ArcGIS sdk to the project
4) Add ArcGIS cloud layers and the API key to the `ArcGISMap` Unity object from the inspector tool
5) Edit the Vehicle and IoT objects Azure properties from the inspector tool

# Azure Map
1) Add the blob storage connection string into the appsettings.json.
For local Azurite blob storage you can use folowing connection string:
`DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;`
2) After this create a `layers` blob container and add example layers files into it.
3) Lastly add your Azure Map Client id and Primary Key to the `AzureMapTripsVisualaizer.js` (line 59 - 60)

# SUMO
1) Install sumo if wanted to create own traffic simulations: https://sumo.dlr.de/docs/Installing/index.html
2) Run `osmWebWizard` to select an area where traffic simulation will be done: https://sumo.dlr.de/docs/Tools/Import/OSM.html#osmwebwizardpy
3) Run `sumo-gui` to run the simulation