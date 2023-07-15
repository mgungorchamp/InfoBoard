using InfoBoard.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;
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
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        //Below code works but messy since it cannot parse the error message
        //For now know issue and might be fixed later - IT WORKS... but why not parsing 
        //During category rewrite I might address this issue
        public async Task updateMediaList()
        {
            if (!Utilities.isInternetAvailable())
            {
                //No internet - don't do anything
                return; 
            }          

            Uri uri = new Uri(string.Format(Utilities.MEDIA_FILES_URL, string.Empty));
            String mediaContent = null;
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                FileDownloadService fileDownloadService = new FileDownloadService();
                if (response.IsSuccessStatusCode)
                {
                    
                    mediaContent = await response.Content.ReadAsStringAsync();                    
                    
                    if (mediaContent.Length < 100) 
                    {
                        try
                        {
                            //{"error":{"code":3,"message":"couldn't find device for that device key"}}
                            ErrorWrapper errorWrapper = JsonSerializer.Deserialize<ErrorWrapper>(mediaContent, _serializerOptions);

                            if (errorWrapper?.error?.code == 3)
                            {
                                _logger.LogInformation( $"MEDIA API 55# DEVICE REMOVED \n" +
                                                        $"Message from server: {errorWrapper.error.message} " +
                                                        $"Server Response: {mediaContent}");
                                await fileDownloadService.resetMediaNamesInLocalJSonAndDeleteLocalFiles();
                                return;
                            }
                        }
                        catch (Exception ex2)
                        {                          
                            _logger.LogError($"MEDIA API 06 could not Deserialize\n" +
                                            $"Exception: {ex2.Message}\n" +
                                            $"API Response : {mediaContent}\n");
                        }
                        await fileDownloadService.resetMediaNamesInLocalJSonAndDeleteLocalFiles();
                        return;
                    }
                    List<MediaCategory> fileList = JsonSerializer.Deserialize<List<MediaCategory>>(mediaContent, _serializerOptions);
                    await fileDownloadService.saveCategoryListToLocalJSON(fileList);
                    return;                    
                }
            }
            catch (Exception ex3)
            {                
                Console.WriteLine($"22#FILES Posibiliy Server is having a bad day\n Exception: {ex3.Message}\n");
                _logger.LogError($"\n22#FILES Posibiliy Server is having a bad day\n" +
                                 $"Exception: {ex3.Message}\n" +
                                 $"API Response : {mediaContent}\n");
            }
            //Two reasons to be here
            //1. Server having a bad moment - return existing files
            //2. Device might be unregistered but in this case we should return before this point        
            _logger.LogError($"\t 11#FILES Should not be here - server having a bad day?\n" +
                             $"URI : {uri.ToString()}\n" +
                             $"API Response : {mediaContent}\n");
        }

        /*
         SUCCESS
            "{\"id\":343,\"user_id\":19,\"name\":\"WIN HOME\",\"type\":\"MVP\",\"version\":\"1\",\"last_handshake_temporary_code\":\"GFYMCH\",\"last_heard_from\":\"2023-07-15 02:42:52\",\"device_key\":\"062af6699ac512a04075bc764617ba7e\"}"
         FAILURE
            "{\"error\":{\"code\":3,\"message\":\"couldn't find device for that device key\"}}"
         
         */
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
                        await deviceSettingsService.saveSettingsToLocalAsJSON(deviceSettings);                        
                        _logger.LogInformation($"#33-RS Device settings updated!\nDevice_key: {deviceSettings.device_key}");
                    }
                    else
                    {
                        // Device removed from server - unregister device
                        await deviceSettingsService.resetLocalSettingsFile();
                        await deviceSettingsService.resetDeviceKeyInFile();
                        Debug.WriteLine("Device removed from server - unregister device");
                        _logger.LogWarning($"\n\t#89-SETTTINGS Device removed from server - reset device settings unregister device\n\n" +
                                            $"DeviceSettings.error: {deviceSettings.error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"\tERROR {0} retrieveDeviceSettings MURAT", ex.Message);
                _logger.LogError($"#77-SETTTINGS Exception: {ex.Message} retrieveDeviceSettings MURAT");
            }
            //await Task.Delay(TimeSpan.FromSeconds(2));
        }


        /*
         SUCCESS
            "{\"device_key\":\"062af6699ac512a04075bc764617ba7e\"}"
         ERROR
            "{\"error\":{\"code\":1,\"message\":\"unrecognized device\"}}"
         */

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
                        
                        DeviceSettingsService deviceSettingsService = DeviceSettingsService.Instance;
                        await deviceSettingsService.saveDeviceKeyToFile(registerationResult.device_key);

                        _logger.LogInformation("DEVICE REGISTERED #22: Registration succesfully completed!");
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

