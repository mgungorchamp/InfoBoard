using System.Diagnostics;
using System.Text.Json; 
using InfoBoard.Models;
 

namespace InfoBoard.Services
{
    public sealed class DeviceSettingsService
    {
        private static readonly DeviceSettingsService instance = new DeviceSettingsService();
              
        private DeviceSettingsService()
        {
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
            if (!UtilityServices.isInternetAvailable())
            {                
                Debug.WriteLine($"**No internet - return existing settings{localDeviceSettings.device_key}");  
                return localDeviceSettings;
            }

            //if there is internet and device already registered
            //update the settings from web and return latest settings
            //Get latest device settings and update the local settings file
            if (localDeviceSettings != null)
            {
                RestService restService = new RestService();
                await restService.updateDeviceSettings(localDeviceSettings.device_key);
                //Read the updated settings file and return the latest settings
                return await readSettingsFromLocalJSON();
            }
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
                string fullPathJsonFileName = Path.Combine(Constants.MEDIA_DIRECTORY_PATH, fileName);
                if (File.Exists(fullPathJsonFileName))
                {                    
                    string jsonString = await File.ReadAllTextAsync(fullPathJsonFileName);
                    if (jsonString.Length < 20)
                    {
                        return null;
                    }

                    DeviceSettings deviceSettings = JsonSerializer.Deserialize<DeviceSettings>(jsonString);

                    //To update the media files url
                    Constants.updateMediaFilesUrl(deviceSettings.device_key);

                    if (deviceSettings?.device_key == null)
                    {
                        return null;
                    }
                    
                    return deviceSettings;
                }
            } 
            catch(Exception ex) {
                Debug.WriteLine($"{ex.Message} readSettingsFromLocalJSON  MURAT");
            }
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
                Constants.updateMediaFilesUrl(deviceSettings.device_key);

                string fileName = "DeviceSettings.json";
                string fullPathFileName = Path.Combine(Constants.MEDIA_DIRECTORY_PATH, fileName);
                string jsonString = JsonSerializer.Serialize<DeviceSettings>(deviceSettings);

               

                await File.WriteAllTextAsync(fullPathFileName, jsonString);

                await Task.Delay(TimeSpan.FromSeconds(2));

            } catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString() + "saveSettingsToLocalAsJSON has issues MURAT");
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
                Constants.updateMediaFilesUrl("UNREGISTERED");

                string fileName = "DeviceSettings.json";
                string fullPathFileName = Path.Combine(Constants.MEDIA_DIRECTORY_PATH, fileName);
                string jsonString = "RESETED";
                await File.WriteAllTextAsync(fullPathFileName, jsonString);

                await Task.Delay(TimeSpan.FromSeconds(2));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "resetLocalSettingsFile has issues MURAT");
            }
        }

    }
}
