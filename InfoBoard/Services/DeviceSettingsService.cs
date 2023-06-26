using System.Diagnostics;
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
 
        
        public async Task<DeviceSettings> loadDeviceSettings()
        {
            DeviceSettings localDeviceSettings = await readSettingsFromLocalJSON();

            //No internet - return existing settings
            if (!UtilityServices.isInternetAvailable())
            {
                Debug.WriteLine($"**No internet - return existing settings{localDeviceSettings.device_key}");  
                return localDeviceSettings;
            }

            //Already registered - update settings from web
            //If localDeviceSettings is null. Not registered yet - registering device will be done by GoTime
            if (localDeviceSettings != null)
            {
                //Get Device settings
                RestService restService = new RestService();
                DeviceSettings updatedDeviceSettings = await restService.retrieveDeviceSettings(localDeviceSettings.device_key);
                if (/*updatedDeviceSettings != null && */updatedDeviceSettings.error == null)
                {
                    await saveSettingsToLocalAsJSON(updatedDeviceSettings);                    
                }
                else/* if (updatedDeviceSettings == null)*/
                {
                    // Device removed from server - unregister device
                    Debug.WriteLine("Device removed from server - unregister device");
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
                else // Either user imput is expected or device registered already timer kicked in - ignore error - or key expired
                {
                    //Maybe device registered but just before it timer kicks in - ignore error
                    Debug.WriteLine("Atempting to register device... Users input expected");                    
                }
            }
            return registrationResult;
        }

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
                    if (jsonString.Length < 10)
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

                await Task.Delay(TimeSpan.FromSeconds(2));

            } catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString() + "saveSettingsToLocalAsJSON has issues MURAT");
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

                await Task.Delay(TimeSpan.FromSeconds(2));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "resetLocalSettingsFile has issues MURAT");
            }
        }

    }
}
