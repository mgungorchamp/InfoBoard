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

        public List<FileInformation> Items { get; private set; }

        public RestService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<List<FileInformation>> downloadMediaFileNames()
        {
            Items = new List<FileInformation>();

            Uri uri = new Uri(string.Format(Constants.MEDIA_FILES_URL, string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Items = JsonSerializer.Deserialize<List<FileInformation>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return Items;
        }

        public async Task<DeviceSettings> retrieveDeviceSettings(string deviceID)
        {
            DeviceSettings deviceSettings = new DeviceSettings();   

            Uri uri = new Uri(string.Concat(Constants.DEVICE_SETTINGS_URL, deviceID));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    try // THIS MIGHT BE UNNEDED 
                    {
                        deviceSettings = JsonSerializer.Deserialize<DeviceSettings>(content, _serializerOptions);
                    }
                    catch (JsonException jsonException)
                    {
                        Console.WriteLine(@"\tERROR {0}", jsonException);
                    }
                    //deviceSettings.deviceId = "MURAT";            
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return deviceSettings;
        }

        public async Task<ErrorInfo> registerDevice()
        {
            ErrorInfo errorInfo = new ErrorInfo();

            Uri uri = new Uri(string.Format(Constants.HANDSHAKE_URL, string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    errorInfo = JsonSerializer.Deserialize<ErrorInfo>(content, _serializerOptions);                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return errorInfo;
        }
    }
}
