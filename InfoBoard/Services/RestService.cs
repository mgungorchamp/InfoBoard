using InfoBoard.Models;
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

        
        

        public RestService()
        {
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
            if (!UtilityServices.isInternetAvailable())
            {
                //No internet - return existing files
                return await fileDownloadService.synchroniseMediaFiles(); 
            }          

            Uri uri = new Uri(string.Format(Constants.MEDIA_FILES_URL, string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    List<FileInformation> fileList = null;
                    string content = await response.Content.ReadAsStringAsync();
                    if (content.Length < 10) 
                    {
                        return null;
                    }
                    fileList = JsonSerializer.Deserialize<List<FileInformation>>(content, _serializerOptions);
                    return fileList;
                }
            }
            catch (Exception ex)
            {
                //Most likely the device unregistered and we got an error messsage
                Console.WriteLine(@"\tERROR {0} retrieveFileList MURAT", ex.Message);
                return null;
            }

            //return existing files - it should not come here
            return await fileDownloadService.synchroniseMediaFiles();
        }

        public async Task<DeviceSettings> retrieveDeviceSettings(string deviceID)
        {
            DeviceSettingsService deviceSettingsService = DeviceSettingsService.Instance;
           
            if (!UtilityServices.isInternetAvailable())
            {                
                DeviceSettings deviceSettings = await deviceSettingsService.loadDeviceSettings();
                return deviceSettings;
            }

            Uri uri = new Uri(string.Concat(Constants.DEVICE_SETTINGS_URL, deviceID));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    DeviceSettings deviceSettings;
                    string content = await response.Content.ReadAsStringAsync();
                    deviceSettings = JsonSerializer.Deserialize<DeviceSettings>(content, _serializerOptions);
                    
                    //If error is not null, there is an error then return null
                    //Device unregistered
                    if (deviceSettings.error != null) //
                        return null;

                    return deviceSettings;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"\tERROR {0} retrieveDeviceSettings MURAT", ex.Message);
            }
            
            DeviceSettings localDeviceSettings = await deviceSettingsService.loadDeviceSettings();
            return localDeviceSettings;
        }

        
        public async Task<RegisterationResult> registerDevice()
        {
            if (!UtilityServices.isInternetAvailable())
            {
                return null;
            }

            Uri uri = new Uri(Constants.HANDSHAKE_URL);
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    RegisterationResult registerationResult;
                    string content = await response.Content.ReadAsStringAsync();
                    registerationResult = JsonSerializer.Deserialize<RegisterationResult>(content, _serializerOptions);    
                    return registerationResult;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"\tERROR {0} registerDevice MURAT", ex.Message);
            }
            return null;
        }
    }
}
