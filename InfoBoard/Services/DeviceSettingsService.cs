using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InfoBoard.Models;

namespace InfoBoard.Services
{
    internal class DeviceSettingsService
    {
        public static DeviceSettings deviceSettings;

        //If not registered, it tries to register if registered reads 
        public DeviceSettings loadDeviceSettings()
        {
            deviceSettings = readSettingsFromLocalJSON();

            if (deviceSettings.deviceId == "NOTSET")
            {
                //Get Device settings
                Task.Run(() => deviceSettings = retrieveDeviceSettingsFromServer()).Wait();
                saveSettingsToLocalAsJSON(deviceSettings);
            }
            return deviceSettings;
        }

        // HANDSHAKE - Register device and get settings
        // If already registered then just get settings
        private DeviceSettings retrieveDeviceSettingsFromServer()
        {
            RestService restService = new RestService();
            var task = restService.retrieveDeviceSettings(deviceSettings.deviceId);
            task.Wait();
            DeviceSettings retrievedDeviceSettings = task.Result;

            if (retrievedDeviceSettings.deviceId != "NOTSET")
            {
                ;// Settings obtained succesfuly from server
            }
            return retrievedDeviceSettings;
        }

        

        //Read local JSON file - if exist - if not return empty DeviceSettings
        private DeviceSettings readSettingsFromLocalJSON()
        {
            DeviceSettings tempDeviceSettings = new DeviceSettings();
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
                tempDeviceSettings = JsonSerializer.Deserialize<DeviceSettings>(jsonString);
            }
            return tempDeviceSettings;
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
