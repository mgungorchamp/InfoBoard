using MetroLog.MicrosoftExtensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace InfoBoard.Services
{
    public static class Utilities
    {
        //private static readonly ILogger _logger = Utilities.Logger(nameof(Utilities));
        static public IHttpClientFactory _httpClientFactory;

        private static string LocalDirectory = "Media";
        // URL of REST service
        //public static string RestUrl = "https://dotnetmauitodorest.azurewebsites.net/api/todoitems/{0}";

        // URL of REST service (Android does not use localhost)
        // Use http cleartext for local deployment. Change to https for production
        //public static string LocalhostUrl = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";

#if DEBUG
        public static string HostUrl = "infopanel.vermontic.com";
#else        
        public static string HostUrl = "app.guzelboard.com";
#endif

        public static string Scheme = "https"; // or http        

        public static string BASE_ADDRESS = $"{Scheme}://{HostUrl}";

        public static string MEDIA_FILES_URL = "UnSet";

        public static string deviceKey = "NoDeviceKey";

        public static int maximumDisplayWidth = -1;

        //https://HostUrl/api/categories.php?device_key=DEVICE_KEY
        public static void updateMediaFilesUrl(string device_key)
        {
            MEDIA_FILES_URL = $"{Scheme}://{HostUrl}/api/categories.php?device_key={device_key}";
        }

        public static string TEMPORARY_CODE;

        //https://HostUrl/api/handshake.php?temporary_code=a3b8z2&device_type=MVP&version=1
        public static string HANDSHAKE_URL;
        private static void updateHandshakeUrl(string temporaryCode)
        {
            HANDSHAKE_URL = $"{Scheme}://{HostUrl}/api/handshake.php?temporary_code={temporaryCode}&device_type=MVP&version=1";
        }

        public static string DEVICE_SETTINGS_URL = $"{Scheme}://{HostUrl}/api/settings.php?device_key=";


        //public static string Port = "5001";
        //public static string RestUrl = $"{Scheme}://{LocalhostUrl}:{Port}/api/todoitems/{{0}}";


        public static string MEDIA_DIRECTORY_PATH = getMediaDirectoryInformation().FullName;
        public static string QR_IMAGE_NAME_4_TEMP_CODE = "qrCodeImageFile.png";


        public static JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };





        // Random alphanumeric string to use as a security key for the handshake
        public static void resetTemporaryCodeAndHandshakeURL()
        {
            Random random = new Random();

            // String that contain both alphabets and numbers
            //String keyPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String keyPool = "ABCDEFGHKLMNPQRSTUVWXYZ123456789";
            int size = 6;

            // Initializing the empty string
            String randomID = "";

            for (int i = 0; i < size; i++)
            {

                // Selecting a index randomly
                int index = random.Next(keyPool.Length);

                // Appending the character at the 
                // index to the random alphanumeric string.
                randomID = randomID + keyPool[index];
            }

            TEMPORARY_CODE = randomID;
            updateHandshakeUrl(TEMPORARY_CODE);
        }

        // If media folder is not created it creates, if exist it retuns the existing one
        public static DirectoryInfo getMediaDirectoryInformation()
        {
            // Get the folder where the images are stored.
            string appDataPath = FileSystem.Current.AppDataDirectory;
            string directoryName = Path.Combine(appDataPath, Utilities.LocalDirectory);

            //create the directory if it doesn't exist
            DirectoryInfo directoryInfo;
            if (!Directory.Exists(directoryName))
                directoryInfo = Directory.CreateDirectory(directoryName);
            else
                directoryInfo = new DirectoryInfo(directoryName);

            return directoryInfo;
        }


        private static ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddStreamingFileLogger(options =>
        {
            options.RetainDays = 2;               
            options.FolderPath = Path.Combine(FileSystem.CacheDirectory, "InfoBoardLogs");
        }));

        public static ILogger Logger(string categoryName) {            
            return loggerFactory.CreateLogger(categoryName);
        }


        private static bool HasInternetConnection = true;

        public static async Task UpdateInternetStatus()
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType == NetworkAccess.Internet)
            {
                HasInternetConnection = true;
                return;
            }
            else
            {
                Ping ping = new Ping();
                try
                {
                    PingReply reply = await ping.SendPingAsync("8.8.8.8", 3000);
                    if (reply.Status == IPStatus.Success)
                    {
                        HasInternetConnection = true;
                        return;
                    }
                }               
                catch (Exception ex)
                {
                    Debug.WriteLine($"ERROR #IA03 at Utilities isInternetAvailable Exception: {ex.Message}"); // Do nothing
                }
            }
            HasInternetConnection = false;
        }

        public static bool isInternetAvailable()
        {
            return HasInternetConnection;
        }


        //https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-7.0
        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
               
        //public static async Task UpdateInternetStatus()
        //{
        //    Debug.WriteLine("isInternetAvailable+!");
        //    NetworkAccess accessType = Connectivity.Current.NetworkAccess;
        //    if (accessType == NetworkAccess.Internet)
        //    {
        //        HasInternetConnection = true;
        //    }
        //    else
        //    {   // Call asynchronous network methods in a try/catch block to handle exceptions.
        //        try
        //        {
        //            using HttpResponseMessage response = await client.GetAsync("https://google.com/");
        //            response.EnsureSuccessStatusCode();
        //            Debug.WriteLine("isInternetAvailable-! TRUE");
        //            HasInternetConnection = true;
        //        }
        //        catch (HttpRequestException e)
        //        {
        //            Debug.WriteLine("\nException Caught!");
        //            Debug.WriteLine("Message :{0} ", e.Message);
        //            Debug.WriteLine("isInternetAvailable-! FALSE");
        //            HasInternetConnection = false;
        //        }               
        //    }
        //}
    }
}


 
 