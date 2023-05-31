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
        public DeviceSettings loadDeviceSettings()
        {
            DeviceSettings deviceSettings = readSettingsFromLocalJSON();
            
            //No settings found - register device
            if (deviceSettings == null)
            {
                RegisterDeviceViewModel registerDeviceViewModel = RegisterDeviceViewModel.Instance;
                registerDeviceViewModel.startRegistration();
            }
            //already registered - get/update settings 
            else
            {               
                //Get Device settings
                Task.Run(() => deviceSettings = retrieveDeviceSettingsFromServer(deviceSettings.device_key)).Wait();
                saveSettingsToLocalAsJSON(deviceSettings);
            }
            return deviceSettings;
        }

        // HANDSHAKE - Register device and get settings
        // If already registered then just get settings
        public DeviceSettings retrieveDeviceSettingsFromServer(string device_key)
        {     
            RestService restService = new RestService();
            var task = restService.retrieveDeviceSettings(device_key);
            task.Wait();
            DeviceSettings retrievedDeviceSettings = task.Result;          
            return retrievedDeviceSettings;
        }

        

        //Read local JSON file - if exist - if not return empty DeviceSettings
        public DeviceSettings readSettingsFromLocalJSON()
        {
            //DeviceSettings tempDeviceSettings = new DeviceSettings();
            JsonSerializerOptions _serializerOptions;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            string fileName = "DeviceSettings.json";
            string fullPathJsonFileName = Path.Combine(Constants.MEDIA_DIRECTORY_PATH, fileName);
            if (File.Exists(fullPathJsonFileName))
            {
                string jsonString = File.ReadAllText(fullPathJsonFileName);
                DeviceSettings tempDeviceSettings = JsonSerializer.Deserialize<DeviceSettings>(jsonString);
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
