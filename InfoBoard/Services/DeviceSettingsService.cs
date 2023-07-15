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

                if (localDeviceSettings == null)
                {
                    //Check if there is DeviceKey in local storage - second check
                    return await secondCheckIfSettingsNull();
                }
                else
                {
                    return localDeviceSettings;
                }
            }

            //if there is internet and device already registered
            //update the settings from web and return latest settings
            //Get latest device settings and update the local settings file
            if (localDeviceSettings != null)
            {
                RestService restService = new RestService();
                await restService.updateDeviceSettings(localDeviceSettings.device_key);
                //_logger.LogInformation($"LD-01-DS ** updateDeviceSettings  Device Name: {localDeviceSettings.name}");
                //Read the updated settings file and return the latest settings
                _logger.LogInformation($"SETTINGS #098 Updated from WebServer");
                return await readSettingsFromLocalJSON();
            }
            _logger.LogInformation($"INFO:44 Return existing settings loadDeviceSettings");

            if (localDeviceSettings == null)
            {
                //Check if there is DeviceKey in local storage - second check
                return await secondCheckIfSettingsNull();
            }
            else
            {
                return localDeviceSettings;
            }            
        }

        private async Task<DeviceSettings> secondCheckIfSettingsNull() 
        {
            //Check if there is DeviceKey in local storage - second check
            string deviceKey = await readDeviceKeyFromFile();
            // return null settings since device is NOT REGISTERED or UNREGISTERED
            if (deviceKey == "UNREGISTERED" || deviceKey == "UNKNOWN")
            {
                _logger.LogCritical($"SETTING #5533 DEVICE MIGHT BE {deviceKey}");
                return null;
            }
            else
            {
                _logger.LogCritical($"SETTING #6677 DEVICE REGISTERED with deviceKey{deviceKey} but settings file comes null?!");

                //This is needed, if the device still remembers the old device key but actually 
                //it has been removed from the server, we need to make sure about it, to reset the device key
                RestService restService = new RestService();
                await restService.updateDeviceSettings(deviceKey);

                DeviceSettings partialDeviceSettings = new DeviceSettings();
                partialDeviceSettings.device_key = deviceKey;
                return partialDeviceSettings;
            }
        }

        //Read local JSON file - if exist - if not return NULL 
        private async Task<DeviceSettings> readSettingsFromLocalJSON()
        {
            //await Task.Delay(TimeSpan.FromSeconds(2));
            try
            {
                string fileName = "DeviceSettings.json";
                string fullPathJsonFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileName);
                if (File.Exists(fullPathJsonFileName))
                {                    
                    string jsonString = await File.ReadAllTextAsync(fullPathJsonFileName);
                    if (jsonString.Length < 20)
                    {
                        _logger.LogWarning($"ERROR#802 SETTINGS FILE  readSettingsFromLocalJSON \n" +
                            $"File Content:{jsonString}\n");
                        return null;
                    }

                    DeviceSettings deviceSettings = JsonSerializer.Deserialize<DeviceSettings>(jsonString);

                    //To update the media files url
                    Utilities.updateMediaFilesUrl(deviceSettings.device_key);

                    if (deviceSettings?.device_key == null)
                    {
                        _logger.LogWarning($"SETTINGS#66 Device Key is Null readSettingsFromLocalJSON");
                        return null;
                    }
                    _logger.LogInformation($"SETTINGS#66 All is well readSettingsFromLocalJSON: " +
                        $"\nFile Content:{jsonString}\n");    
                    return deviceSettings;
                }
                else
                {
                    _logger.LogWarning($"\"ERROR#569 SETTINGS Returning null for settings readSettingsFromLocalJSON - most likely file does not exist\n" +
                                       $"File Name: {fullPathJsonFileName}");
                    return null;
                }
            } 
            catch(Exception ex) {
                Debug.WriteLine($"{ex.Message} readSettingsFromLocalJSON  MURAT");
                _logger.LogError($"ERROR#926 SETTINGS readSettingsFromLocalJSON\n" +
                    $"Exception: {ex.Message}");
            }
            _logger.LogWarning($"SETTINGS#66 NULL settings returned readSettingsFromLocalJSON - MURAT ");
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

                if(jsonString.Length < 20)
                {
                    _logger.LogError($"ERROR#882 SETTINGS FILE  saveSettingsToLocalAsJSON \n" +
                                               $"jsonString:{jsonString}\n" +
                                               $"device_key:{deviceSettings.device_key}");
                    return;
                }

                await File.WriteAllTextAsync(fullPathFileName, jsonString);
                _logger.LogInformation($"SETTINGS#987 Settings Updated saveSettingsToLocalAsJSON\n" +
                                       $"jsonString: {jsonString}");

                //await Task.Delay(TimeSpan.FromSeconds(2));

            } 
            catch (Exception ex) 
            {
                
                Console.WriteLine(ex.ToString() + "saveSettingsToLocalAsJSON has issues MURAT");
                _logger.LogError($"ERROR # 682 SETTINGS saveSettingsToLocalAsJSON has issues MURAT\n" +
                                $"Exception: {ex.Message}");
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
                string jsonString = "UNREGISTERED";
                await File.WriteAllTextAsync(fullPathFileName, jsonString);

                //await Task.Delay(TimeSpan.FromSeconds(2));
                _logger.LogCritical("Local file has resetted resetLocalSettingsFile");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "resetLocalSettingsFile has issues MURAT");
                _logger.LogError(ex.ToString() + "resetLocalSettingsFile has issues MURAT");
            }
        }


        public async Task saveDeviceKeyToFile(string deviceKey)
        {           
            try
            {
                //To update the media files url
                Utilities.updateMediaFilesUrl(deviceKey);
                Utilities.deviceKey = deviceKey;


                string fileName = "DeviceKey.txt";
                string fullPathFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileName);
                

                await File.WriteAllTextAsync(fullPathFileName, deviceKey);
                _logger.LogInformation($"SETTINGS#147 Device Registered\n" +
                                       $"deviceKey: {deviceKey}");

                //await Task.Delay(TimeSpan.FromSeconds(2));

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString() + "saveDeviceKeyToFile has issues MURAT");
                _logger.LogError(ex.ToString() + "saveDeviceKeyToFile has issues MURAT");
            }
        }

        public async Task<string> readDeviceKeyFromFile()
        {
            string deviceKey = "UNKNOWN";
            try
            {     
                string fileName = "DeviceKey.txt";
                string fullPathFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileName);

                if (File.Exists(fullPathFileName))
                {
                    deviceKey = await File.ReadAllTextAsync(fullPathFileName);
                    _logger.LogInformation($"SETTINGS#365 Device Key Read from File\n" +
                                       $"deviceKey: {deviceKey}");

                    if(deviceKey.Length < 5)
                        return "UNKNOWN";

                    //To update the media files url
                    Utilities.updateMediaFilesUrl(deviceKey);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "readDeviceKeyFromFile has issues MURAT");
                _logger.LogError(ex.ToString() + "DEVICE KEY ERROR #459 readDeviceKeyFromFile has issues MURAT");
            }
            return deviceKey;
        }

        public async Task resetDeviceKeyInFile()
        {
            try
            {
                string deviceKey = "UNREGISTERED";
                //To update the media files url
                Utilities.updateMediaFilesUrl(deviceKey);
                Utilities.deviceKey = deviceKey;

                string fileName = "DeviceKey.txt";
                string fullPathFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileName);


                await File.WriteAllTextAsync(fullPathFileName, deviceKey);
                _logger.LogInformation($"SETTINGS#258 Device UnRegistered\n" +
                                       $"deviceKey: {deviceKey}");

                //await Task.Delay(TimeSpan.FromSeconds(2));

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString() + "resetDeviceKeyInFile has issues MURAT");
                _logger.LogError(ex.ToString() + "resetDeviceKeyInFile has issues MURAT");
            }
        }

    }
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