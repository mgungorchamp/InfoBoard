﻿using InfoBoard.Models;
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
        //https://doumer.me/improve-http-request-performance-in-dotnet-maui-xamarin/
        //https://visualstudiomagazine.com/Blogs/Tool-Tracker/2019/09/mutliple-httpclients.aspx


        //MediaManager manager = MediaManager.Instance;
        //static SentryHttpMessageHandler httpHandler = new SentryHttpMessageHandler();
        HttpClient _httpClient;
        //static HttpClient _httpClient = new HttpClient() // new HttpClient(httpHandler)
        
        JsonSerializerOptions _serializerOptions;
        private readonly ILogger _logger;

        //private static readonly RestService instance = new RestService();

        //public static RestService Instance {
        //    get {
        //        return instance;
        //    }
        //}
        //static RestService()
        //{
        //}

        public RestService()
        {
            _logger = Utilities.Logger(nameof(RestService));
            
            _httpClient = Utilities._httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(Utilities.BASE_ADDRESS);  

            _httpClient.Timeout = TimeSpan.FromMinutes(5); //Default is 100 seconds
                                                       //_httpClient.Timeout = TimeSpan.FromSeconds(200); //Default is 100 seconds

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
        //https://guzelboard.com/api/categories.php?device_key=a9cbf288fdd3f70e0264d3784dab810a
        public async Task updateMediaList()
        {
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
            await semaphoreSlim.WaitAsync();
            try
            {
                if (!Utilities.isInternetAvailable())
                {
                    //No internet - don't do anything
                    return;
                }

                Uri uri = new Uri(string.Format(Utilities.MEDIA_FILES_URL, string.Empty));
                string apiServiceUrl = uri.AbsoluteUri.Replace(Utilities.BASE_ADDRESS, "");


                String mediaContent = null;
                try
                {
                    //HttpClient _httpClient = new HttpClient();
                    HttpResponseMessage response = await _httpClient.GetAsync(apiServiceUrl);//.ConfigureAwait(false);                    
                    //FileDownloadService fileDownloadService = FileDownloadService.Instance;
                    FileDownloadService fileDownloadService = new FileDownloadService();
                    if (response.IsSuccessStatusCode)
                    {

                        mediaContent = await response.Content.ReadAsStringAsync();

                        //Device Registered but no categories assigned
                        if (mediaContent.Length < 10) // empty content received  []
                        {
                            _logger.LogInformation($"MEDIA API 33" +
                                             $"Categories needs to be assigned to Device! Response : {mediaContent}\n");
                            await fileDownloadService.resetMediaNamesInLocalJSonAndDeleteLocalFiles();
                            return;
                        }

                        //Device Unregistered and expecting error message
                        if (mediaContent.Length < 100)
                        {
                            try
                            {
                                //{"error":{"code":3,"message":"couldn't find device for that device key"}}
                                ErrorWrapper errorWrapper = JsonSerializer.Deserialize<ErrorWrapper>(mediaContent, _serializerOptions);

                                if (errorWrapper?.error?.code == 3)
                                {
                                    _logger.LogInformation($"MEDIA API 55# DEVICE REMOVED \n" +
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
                        _logger.LogInformation($"RestService API 736 Category (Media) List Updated\n");
                        return;
                    }
                }
                catch (System.Net.WebException ex)
                {
                    Console.WriteLine($"2023-08#WebException WebException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");
                    _logger.LogError($"2023-08#WebException WebException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    //https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socketexception?view=net-7.0
                    Console.WriteLine($"2023-08#updateMediaList SocketException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");
                    _logger.LogError($"2023-08#updateMediaList SocketException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");

                }
#if ANDROID
                catch (Java.IO.IOException ex)
                {
                    Console.WriteLine($"2023-08#updateMediaList Java.IO.IOException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");
                    _logger.LogError($"2023-08#updateMediaList Java.IO.IOException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");

                }
#endif
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
            finally
            {
                //Very important to release
                semaphoreSlim.Release();
            }
        }

        /*
         SUCCESS
            "{\"id\":343,\"user_id\":19,\"name\":\"WIN HOME\",\"type\":\"MVP\",\"version\":\"1\",\"last_handshake_temporary_code\":\"GFYMCH\",\"last_heard_from\":\"2023-07-15 02:42:52\",\"device_key\":\"062af6699ac512a04075bc764617ba7e\"}"
         FAILURE
            "{\"error\":{\"code\":3,\"message\":\"couldn't find device for that device key\"}}"
         
         */
        public async Task updateDeviceSettings(string deviceKey)
        {
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
            await semaphoreSlim.WaitAsync();
            try
            {
                //No internet - return 
                if (!Utilities.isInternetAvailable())
                {
                    return;
                }

                Uri uri = new Uri(string.Concat(Utilities.DEVICE_SETTINGS_URL, deviceKey));
                string apiServiceUrl = uri.AbsoluteUri.Replace(Utilities.BASE_ADDRESS, "");

                try
                {
                    //HttpClient _httpClient = new HttpClient();
                    HttpResponseMessage response = await _httpClient.GetAsync(apiServiceUrl);//.ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        DeviceSettings deviceSettings;
                        string content = await response.Content.ReadAsStringAsync();
                        deviceSettings = JsonSerializer.Deserialize<DeviceSettings>(content, _serializerOptions);


                        //DeviceSettingsService deviceSettingsService = DeviceSettingsService.Instance;
                        DeviceSettingsService deviceSettingsService = new DeviceSettingsService();

                        //If error is null, there is no error then update the settings
                        if (deviceSettings.error == null)
                        {
                            await deviceSettingsService.saveSettingsToLocalAsJSON(deviceSettings);
                            _logger.LogInformation($"#33-RS Device settings updated!\tDevice_key: {deviceSettings.device_key}");
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
                catch (System.Net.WebException ex)
                {
                    Console.WriteLine($"2023-08#WebException SocketException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");
                    _logger.LogError($"2023-08#WebException SocketException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");
                }

                catch (System.Net.Sockets.SocketException ex)
                {
                    Console.WriteLine($"2023-08#updateDeviceSettings SocketException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");
                    _logger.LogError($"2023-08#updateDeviceSettings SocketException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");

                }
#if ANDROID
                catch (Java.IO.IOException ex)
                {
                    Console.WriteLine($"2023-08#12 Java.IO.IOException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");
                    _logger.LogError($"2023-08#12 Java.IO.IOException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");

                }
#endif
                catch (Exception ex)
                {
                    Console.WriteLine(@"\tERROR {0} retrieveDeviceSettings MURAT", ex.Message);
                    _logger.LogError($"#77-SETTTINGS Exception: {ex.Message}" +
                         $"URI : {uri.ToString()}\n" +
                         $"updateDeviceSettings MURAT");
                }
                //await Task.Delay(TimeSpan.FromSeconds(2));
            }
            finally
            {
                //Very important to release
                semaphoreSlim.Release();
            }
        }


        /*
         SUCCESS
            "{\"device_key\":\"062af6699ac512a04075bc764617ba7e\"}"
         ERROR
            "{\"error\":{\"code\":1,\"message\":\"unrecognized device\"}}"
         */

        public async Task<string> registerDevice()
        {
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
            await semaphoreSlim.WaitAsync();
            try
            {
                if (!Utilities.isInternetAvailable())
                {
                    return "No internet";// No internet - return
                }

                try
                {
                    Uri uri = new Uri(Utilities.HANDSHAKE_URL);
                    string apiServiceUrl = uri.AbsoluteUri.Replace(Utilities.BASE_ADDRESS, "");


                    //HttpClient _httpClient = new HttpClient();
                    HttpResponseMessage response = await _httpClient.GetAsync(apiServiceUrl);//.ConfigureAwait(false);
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

                            //DeviceSettingsService deviceSettingsService = DeviceSettingsService.Instance;
                            DeviceSettingsService deviceSettingsService = new DeviceSettingsService();
                            await deviceSettingsService.saveDeviceKeyToFile(registerationResult.device_key);

                            _logger.LogInformation("DEVICE REGISTERED #22: Registration succesfully completed!");
                            return "Registration succesfully completed!";
                        }
                        else // Either user imput is expected or device registered already timer kicked in - ignore error - or key expired
                        {
                            //Maybe device registered but just before it timer kicks in - ignore error
                            Debug.WriteLine("Atempting to register device... Users input expected");
                            _logger.LogWarning("Atempting to register device... Users input expected");
                            return $"\nUser input expected!" +
                                    $"\n\nServer says: {registerationResult.error.message}";
                        }
                    }
                }
                catch (System.Net.WebException ex)
                {
                    Console.WriteLine($"2023-08#66 WebException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");
                    _logger.LogError($"2023-08#66 WebException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    Console.WriteLine($"2023-08#registerDevice SocketException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");
                    _logger.LogError($"2023-08#registerDevice SocketException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");

                }
#if ANDROID
                catch (Java.IO.IOException ex)
                {
                    Console.WriteLine($"2023-08#44 Java.IO.IOException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");
                    _logger.LogError($"2023-08#44 Java.IO.IOException\n Exception: {ex.Message}\n Help Link: {ex.HelpLink} \n Target Site: {ex.TargetSite}\n");

                }
#endif
                catch (Exception ex)
                {
                    Console.WriteLine(@"\tERROR {0} registerDevice MURAT", ex.Message);
                    _logger.LogError($"Device Registration Exception:ERROR {ex.Message} registerDevice MURAT");
                    return $"Device Registration Exception: ERROR {ex.Message} registerDevice MURAT";
                }
                _logger.LogError($"Another registration message waiting to be more informative");
                return "Another registration message waiting to be more informative";
            }
            finally
            {
                //Very important to release
                semaphoreSlim.Release();
            }
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
//        HttpResponseMessage response = await _httpClient.GetAsync(uri);
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

