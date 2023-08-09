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


        public static bool isInternetAvailable()
        {
            Debug.WriteLine("isInternetAvailable+!");
            return true;
            //NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            //if (accessType == NetworkAccess.Internet)
            //{
            //    return true;
            //}
            //else
            //{
            // with await
            //var taskResult = Task.Run<bool>(async () => await checkInternetConnection());
            //    return taskResult.Result;
            
            //}
            //return false;
        }


        //https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-7.0
        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
        private static readonly HttpClient client = new HttpClient();
        
        public static bool HasInternetConnection = false;
        static async Task<bool> checkInternetConnection()
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {

                using HttpResponseMessage response = await client.GetAsync("https://guzelboard.com/");
                
                await Task.Delay(TimeSpan.FromSeconds(1));
                
                response.EnsureSuccessStatusCode();

                //HasInternetConnection = true;
                Debug.WriteLine("isInternetAvailable-! TRUE");
                return true;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("\nException Caught!");
                Debug.WriteLine("Message :{0} ", e.Message);
                //HasInternetConnection = false;
                Debug.WriteLine("isInternetAvailable-! FALSE");
                return false;
            }
            catch (IOException ex)
            {
                // For System.Net.Sockets.SocketException: 'Permission denied'
                Debug.WriteLine($"ERROR #IA01 at Utilities isInternetAvailable IOException: {ex.Message}"); // Do nothing

                return false;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                // For System.Net.Sockets.SocketException: 'Permission denied'
                Debug.WriteLine($"ERROR #IA02 at Utilities isInternetAvailable System.Net.Sockets.SocketException: {ex.Message}"); // Do nothing
                return false;
            }
        }
    }
}


/*
  public static bool isInternetAvailable()
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType == NetworkAccess.Internet)
            {
                return true;
            }
            else
            {
                Ping ping = new Ping();
                try
                {
                    PingReply reply = ping.Send("8.8.8.8", 3000);
                    if (reply.Status == IPStatus.Success)
                    {
                        return true;
                    }
                }
                catch (IOException ex) {
                    // For System.Net.Sockets.SocketException: 'Permission denied'
                    Console.WriteLine($"ERROR #IA01 at Utilities isInternetAvailable IOException: {ex.Message}"); // Do nothing
                    _logger.LogError($"ERROR #IA01 at Utilities isInternetAvailable IOException: {ex.Message}");
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    // For System.Net.Sockets.SocketException: 'Permission denied'
                    Console.WriteLine($"ERROR #IA02 at Utilities isInternetAvailable System.Net.Sockets.SocketException: {ex.Message}"); // Do nothing
                    _logger.LogError($"ERROR #IA02 at Utilities isInternetAvailable System.Net.Sockets.SocketException: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR #IA03 at Utilities isInternetAvailable Exception: {ex.Message}"); // Do nothing
                    _logger.LogError($"ERROR #IA03 at Utilities isInternetAvailable Exception: {ex.Message}");
                }
            }
            return false;
        }
 
 */