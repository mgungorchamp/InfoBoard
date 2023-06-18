 using System.Text.Json; 
using InfoBoard.Models;
 

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
            DeviceSettings localDeviceSettings = await readSettingsFromLocalJSON();

            //No internet - return existing settings
            if (!UtilityServices.isInternetAvailable())
            {
                return localDeviceSettings;
            }

            //Already registered - update settings from web
            //If localDeviceSettings is null. Not registered yet - registering device will be done by GoTime
            if (localDeviceSettings != null)
            {
                //Get Device settings
                RestService restService = new RestService();
                DeviceSettings updatedDeviceSettings = await restService.retrieveDeviceSettings(localDeviceSettings.device_key);
                if (updatedDeviceSettings != null && updatedDeviceSettings.error == null)
                {
                    await saveSettingsToLocalAsJSON(updatedDeviceSettings);                    
                }
                else if (updatedDeviceSettings == null)
                {
                    // Device removed from server - unregister device
                    await resetLocalSettingsFile();
                }
                
            }
            
            return await readSettingsFromLocalJSON();
        }

        public async Task<RegisterationResult> RegisterDeviceViaServer()
        {
            //First time registration
            RestService restService = new RestService();
            RegisterationResult registrationResult = await restService.registerDevice();

            //We get a response from server
            if (registrationResult != null)                
            {
                //Registeration succesful - no error
                if (registrationResult.error == null)
                {
                    //Registeration succesful and request full settings and save it to local settings
                    DeviceSettings deviceSettings = await restService.retrieveDeviceSettings(registrationResult.device_key);                                     
                    await saveSettingsToLocalAsJSON(deviceSettings);                    
                }
                else // Registration failed - error returned
                {
                    //TODO : Handle error here
                    //if we empty the device_key in local settings, it will try to register again
                    await resetLocalSettingsFile();
                }
            }
            return registrationResult;
        }

        //Read local JSON file - if exist - if not return NULL 
        private async Task<DeviceSettings> readSettingsFromLocalJSON()
        {
            string fileName = "DeviceSettings.json";
            string fullPathJsonFileName = Path.Combine(Constants.MEDIA_DIRECTORY_PATH, fileName);
            if (File.Exists(fullPathJsonFileName))
            {
                string jsonString = await File.ReadAllTextAsync(fullPathJsonFileName);
                if (jsonString.Length < 10)
                {
                    return null;
                }                

                DeviceSettings deviceSettings = JsonSerializer.Deserialize<DeviceSettings>(jsonString);
                
                if (deviceSettings?.device_key == null) 
                {
                    return null;
                }
                //To update the media files url
                Constants.updateMediaFilesUrl(deviceSettings.device_key);
                return deviceSettings;
            }
            return null;
        }

        private async Task saveSettingsToLocalAsJSON(DeviceSettings deviceSettings)
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

            } catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString() + "saveSettingsToLocalAsJSON has issues");
            }
        }
        
        private async Task resetLocalSettingsFile()
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

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "resetLocalSettingsFile has issues");
            }
        }

    }
}
