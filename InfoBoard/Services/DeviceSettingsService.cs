using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InfoBoard.Models;
using InfoBoard.ViewModel;

namespace InfoBoard.Services
{
    public sealed class DeviceSettingsService
    {
        private static readonly DeviceSettingsService instance = new DeviceSettingsService();
        static DeviceSettingsService()
        {
        }
        private DeviceSettingsService()
        {
        }
        public static DeviceSettingsService Instance {
            get {
                return instance;
            }
        }    

        //If not registered, it tries to register if registered reads 
        public async Task<DeviceSettings> loadDeviceSettings()
        {
            DeviceSettings deviceSettings = readSettingsFromLocalJSON();

            //Check for updates
            if (deviceSettings != null)
            {                       
                //Get Device settings
                deviceSettings = await retrieveDeviceSettingsFromServer(deviceSettings.device_key);
                saveSettingsToLocalAsJSON(deviceSettings);
            }

            //This is needed here - if read from local file - it will not have the correct url or retrieved from web           
            if (deviceSettings != null)
            {
                Constants.updateMediaFilesUrl(deviceSettings.device_key);
            }
            return deviceSettings;
        }

        // HANDSHAKE - Register device and get settings
        // If already registered then just get settings
        public async Task<DeviceSettings> retrieveDeviceSettingsFromServer(string device_key)
        {     
            RestService restService = new RestService();
            var task = restService.retrieveDeviceSettings(device_key);        
            DeviceSettings retrievedDeviceSettings = await task;          
            return retrievedDeviceSettings;
        }

        

        //Read local JSON file - if exist - if not return empty DeviceSettings
        public DeviceSettings readSettingsFromLocalJSON()
        {
            string fileName = "DeviceSettings.json";
            string fullPathJsonFileName = Path.Combine(Constants.MEDIA_DIRECTORY_PATH, fileName);
            if (File.Exists(fullPathJsonFileName))
            {

                string jsonString = File.ReadAllText(fullPathJsonFileName);
                if(jsonString.Length < 10)
                {
                    return null;
                }
               
                JsonSerializerOptions _serializerOptions;
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                DeviceSettings tempDeviceSettings = JsonSerializer.Deserialize<DeviceSettings>(jsonString);
                
                if (tempDeviceSettings.device_key == null) 
                {
                    return null;
                }

                return tempDeviceSettings;
            }
            return null;
        }

        public void saveSettingsToLocalAsJSON(DeviceSettings deviceSettings)
        {
            JsonSerializerOptions _serializerOptions;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            string fileName = "DeviceSettings.json";
            string fullPathFileName = Path.Combine(Constants.MEDIA_DIRECTORY_PATH, fileName);
            string jsonString = JsonSerializer.Serialize<DeviceSettings>(deviceSettings);
            File.WriteAllText(fullPathFileName, jsonString);
        }
    }
}
