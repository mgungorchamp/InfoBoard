using System.Diagnostics;
using System.Text.Json; 
using InfoBoard.Models;
using Microsoft.Extensions.Logging;

namespace InfoBoard.Services
{
    public sealed class DeviceSettingsService
    {
        private static readonly DeviceSettingsService instance = new DeviceSettingsService();
        private readonly ILogger _logger;
        private DeviceSettingsService()
        {
            _logger = Utilities.Logger(nameof(DeviceSettingsService));
        }
        public static DeviceSettingsService Instance {
            get {
                return instance;
            }
        }
 
        
        public async Task<DeviceSettings> loadDeviceSettings()
        {
            DeviceSettings localDeviceSettings = await readSettingsFromLocalJSON();
            //No internet - return existing settings
            if (!Utilities.isInternetAvailable())
            {                
                Debug.WriteLine($"**No internet - return existing settings");  
                _logger.LogInformation($"**No internet - return existing settings");
                return localDeviceSettings;
            }

            //if there is internet and device already registered
            //update the settings from web and return latest settings
            //Get latest device settings and update the local settings file
            if (localDeviceSettings != null)
            {
                RestService restService = new RestService();
                await restService.updateDeviceSettings(localDeviceSettings.device_key);
                _logger.LogInformation($"LD-01 **updateDeviceSettings \n{localDeviceSettings.name}");
                //Read the updated settings file and return the latest settings
                return await readSettingsFromLocalJSON();
            }
            _logger.LogInformation($"**return existing settings loadDeviceSettings");
            return localDeviceSettings;
        }


        //public async Task<DeviceSettings> loadDeviceSettingsOLD()
        //{
        //    DeviceSettings localDeviceSettings = await readSettingsFromLocalJSON();

        //    //No internet - return existing settings
        //    if (!UtilityServices.isInternetAvailable())
        //    {
        //        Debug.WriteLine($"**No internet - return existing settings{localDeviceSettings.device_key}");
        //        return localDeviceSettings;
        //    }

        //    //Already registered - update settings from web
        //    //If localDeviceSettings is null. Not registered yet - registering device will be done by GoTime
        //    if (localDeviceSettings != null)
        //    {
        //        //Get Device settings
        //        RestService restService = new RestService();
        //        DeviceSettings updatedDeviceSettings = await restService.updateDeviceSettings(localDeviceSettings.device_key);
        //        if (/*updatedDeviceSettings != null && */updatedDeviceSettings.error == null)
        //        {
        //            await saveSettingsToLocalAsJSON(updatedDeviceSettings);
        //        }
        //        else/* if (updatedDeviceSettings == null)*/
        //        {
        //            // Device removed from server - unregister device
        //            Debug.WriteLine("Device removed from server - unregister device");
        //            await resetLocalSettingsFile();
        //        }
        //    }
        //    return await readSettingsFromLocalJSON();
        //}

        //public async Task RegisterDeviceViaServer()
        //{
        //    //First time registration
        //    RestService restService = new RestService();
        //    await restService.registerDevice();
        //}

        //Read local JSON file - if exist - if not return NULL 
        private async Task<DeviceSettings> readSettingsFromLocalJSON()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            try
            {
                string fileName = "DeviceSettings.json";
                string fullPathJsonFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileName);
                if (File.Exists(fullPathJsonFileName))
                {                    
                    string jsonString = await File.ReadAllTextAsync(fullPathJsonFileName);
                    if (jsonString.Length < 20)
                    {
                        _logger.LogWarning($"4 **return null readSettingsFromLocalJSON");
                        return null;
                    }

                    DeviceSettings deviceSettings = JsonSerializer.Deserialize<DeviceSettings>(jsonString);

                    //To update the media files url
                    Utilities.updateMediaFilesUrl(deviceSettings.device_key);

                    if (deviceSettings?.device_key == null)
                    {
                        _logger.LogWarning($"6 **return null readSettingsFromLocalJSON");
                        return null;
                    }
                    _logger.LogInformation($"8 **return deviceSettings readSettingsFromLocalJSON");    
                    return deviceSettings;
                }
                else
                {
                    _logger.LogWarning($"10 **return null readSettingsFromLocalJSON - most likely file does not exist");
                    return null;
                }
            } 
            catch(Exception ex) {
                Debug.WriteLine($"{ex.Message} readSettingsFromLocalJSON  MURAT");
                _logger.LogError($"9 {ex.Message} readSettingsFromLocalJSON  MURAT");
            }
            _logger.LogWarning($"11 **return null readSettingsFromLocalJSON - MURAT ");
            return null;
        }

        public async Task saveSettingsToLocalAsJSON(DeviceSettings deviceSettings)
        {
            JsonSerializerOptions _serializerOptions;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            try
            {
                //To update the media files url
                Utilities.updateMediaFilesUrl(deviceSettings.device_key);

                string fileName = "DeviceSettings.json";
                string fullPathFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileName);
                string jsonString = JsonSerializer.Serialize<DeviceSettings>(deviceSettings);

                await File.WriteAllTextAsync(fullPathFileName, jsonString);

                await Task.Delay(TimeSpan.FromSeconds(2));

            } catch (Exception ex) 
            {
                
                Console.WriteLine(ex.ToString() + "saveSettingsToLocalAsJSON has issues MURAT");
                _logger.LogError(ex.ToString() + "saveSettingsToLocalAsJSON has issues MURAT");
            }
        }
        
        public async Task resetLocalSettingsFile()
        {
            JsonSerializerOptions _serializerOptions;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            try
            {
                //To update the media files url
                Utilities.updateMediaFilesUrl("UNREGISTERED");

                string fileName = "DeviceSettings.json";
                string fullPathFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileName);
                string jsonString = "RESETED";
                await File.WriteAllTextAsync(fullPathFileName, jsonString);

                await Task.Delay(TimeSpan.FromSeconds(2));
                _logger.LogCritical("Log file has resetted resetLocalSettingsFile");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "resetLocalSettingsFile has issues MURAT");
                _logger.LogError(ex.ToString() + "resetLocalSettingsFile has issues MURAT");
            }
        }

    }
}
