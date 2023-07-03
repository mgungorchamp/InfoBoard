using InfoBoard.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;


/*
 * https://guzelboard.com/views/login.php
 Ref: https://github.com/dotnet/maui-samples/tree/main/7.0/WebServices/TodoREST/TodoREST
 */

namespace InfoBoard.Services
{
    public class RestService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;
        private readonly ILogger _logger;

        public RestService()
        {
            _logger = Utilities.Logger(nameof(RestService));
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<List<FileInformation>> retrieveFileList()
        {
            FileDownloadService fileDownloadService = new FileDownloadService();
            if (!Utilities.isInternetAvailable())
            {
                //No internet - return existing files
                return await fileDownloadService.synchroniseMediaFiles(); 
            }          

            Uri uri = new Uri(string.Format(Utilities.MEDIA_FILES_URL, string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    List<FileInformation> fileList = null;
                    string content = await response.Content.ReadAsStringAsync();
                    if (content.Length < 20) 
                    {
                        _logger.LogInformation($"02# Content is empty retrieveFileList MURAT{uri}");
                        return null;
                    }
                    fileList = JsonSerializer.Deserialize<List<FileInformation>>(content, _serializerOptions);
                    return fileList;
                }
            }
            catch (Exception ex)
            {
                //Most likely the device unregistered and we got an error messsage
                Console.WriteLine(@"\tDevice Unregistered ERROR JSON Received {0} retrieveFileList MURAT", ex.Message);
                _logger.LogError(@"\t 04# Posibiliy Device Unregistered ERROR JSON Received {0} retrieveFileList MURAT\n", ex.Message);
                return null;
            }
            _logger.LogError("03# Posible ERROR retrieveFileList MURAT");
            //return existing files - it should not come here
            return await fileDownloadService.synchroniseMediaFiles();
        }

        public async Task updateDeviceSettings(string deviceKey)
        {  
            //No internet - return 
            if (!Utilities.isInternetAvailable())
            {                 
                return;
            }

            Uri uri = new Uri(string.Concat(Utilities.DEVICE_SETTINGS_URL, deviceKey));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    DeviceSettings deviceSettings;
                    string content = await response.Content.ReadAsStringAsync();
                    deviceSettings = JsonSerializer.Deserialize<DeviceSettings>(content, _serializerOptions);


                    DeviceSettingsService deviceSettingsService = DeviceSettingsService.Instance;
                   
                    //If error is null, there is no error then update the settings
                    if (deviceSettings.error == null)
                    {
                        _logger.LogInformation("Device settings updated");
                        await deviceSettingsService.saveSettingsToLocalAsJSON(deviceSettings);
                    }
                    else
                    {
                        // Device removed from server - unregister device
                        Debug.WriteLine("Device removed from server - unregister device");
                        _logger.LogWarning("\n\n\t\t@@@@@ Device removed from server - reset device settings unregister device\n\n");
                        await deviceSettingsService.resetLocalSettingsFile();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"\tERROR {0} retrieveDeviceSettings MURAT", ex.Message);
                _logger.LogError($"ERROR {ex.Message} retrieveDeviceSettings MURAT");
            }
        }


        public async Task<string> registerDevice()
        {
            if (!Utilities.isInternetAvailable())
            {
                return "No internet";// No internet - return
            }
            
            try
            {
                Uri uri = new Uri(Utilities.HANDSHAKE_URL);
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    RegisterationResult registerationResult;
                    string content = await response.Content.ReadAsStringAsync();
                    registerationResult = JsonSerializer.Deserialize<RegisterationResult>(content, _serializerOptions);

                    //Registeration succesful - no error
                    if (registerationResult.error == null)
                    {
                        //Registeration succesful and request full settings and save it to local settings
                        await updateDeviceSettings(registerationResult.device_key);
                        _logger.LogInformation("Registration succesfully completed!");
                        return "Registration succesfully completed!";
                    }
                    else // Either user imput is expected or device registered already timer kicked in - ignore error - or key expired
                    {
                        //Maybe device registered but just before it timer kicks in - ignore error
                        Debug.WriteLine("Atempting to register device... Users input expected");
                        _logger.LogWarning("Atempting to register device... Users input expected");
                        return  $"\nUser input expected!" +                                
                                $"\n\nServer says: {registerationResult.error.message}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"\tERROR {0} registerDevice MURAT", ex.Message);
                _logger.LogError($"Device Registration Exception:ERROR {ex.Message} registerDevice MURAT");
                return $"Device Registration Exception: ERROR {ex.Message} registerDevice MURAT";
            }
            _logger.LogError($"Another registration message waiting to be more informative");
            return "Another registration message waiting to be more informative";
        }
    }
}



//public async Task<DeviceSettings> retrieveDeviceSettingsOLD(string deviceKey)
//{
//    DeviceSettingsService deviceSettingsService = DeviceSettingsService.Instance;

//    if (!UtilityServices.isInternetAvailable())
//    {
//        DeviceSettings deviceSettings = await deviceSettingsService.loadDeviceSettings();
//        return deviceSettings;
//    }

//    Uri uri = new Uri(string.Concat(Constants.DEVICE_SETTINGS_URL, deviceKey));
//    try
//    {
//        HttpResponseMessage response = await _client.GetAsync(uri);
//        if (response.IsSuccessStatusCode)
//        {
//            DeviceSettings deviceSettings;
//            string content = await response.Content.ReadAsStringAsync();
//            deviceSettings = JsonSerializer.Deserialize<DeviceSettings>(content, _serializerOptions);

//            //If error is not null, there is an error then return null
//            //Device unregistered
//            //if (deviceSettings.error != null) //
//            //   return null;

//            return deviceSettings;
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine(@"\tERROR {0} retrieveDeviceSettings MURAT", ex.Message);
//    }

//    DeviceSettings localDeviceSettings = await deviceSettingsService.loadDeviceSettings();
//    return localDeviceSettings;
//}

