namespace InfoBoard.Services
{
    public static class Constants
    {
        public static string LocalDirectory = "Media";
        // URL of REST service
        //public static string RestUrl = "https://dotnetmauitodorest.azurewebsites.net/api/todoitems/{0}";

        // URL of REST service (Android does not use localhost)
        // Use http cleartext for local deployment. Change to https for production
        //public static string LocalhostUrl = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
        public static string HostUrl = "guzelboard.com";
        public static string Scheme = "https"; // or http
        public static string UserId = "10";
        public static string MEDIA_FILES_URL = $"{Scheme}://{HostUrl}/api/files.php?userid={UserId}";

        public static string TEMPORARY_CODE;

        //https://guzelboard.com/api/handshake.php?temporary_code=a3b8z2&device_type=MVP&version=1
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

        public static DirectoryInfo getMediaDirectoryInformation()
        {
            // Get the folder where the images are stored.
            string appDataPath = FileSystem.AppDataDirectory;
            string directoryName = Path.Combine(appDataPath, Constants.LocalDirectory);

            //create the directory if it doesn't exist
            DirectoryInfo directoryInfo;
            if (!Directory.Exists(directoryName))
                directoryInfo = Directory.CreateDirectory(directoryName);
            else
                directoryInfo = new DirectoryInfo(directoryName);
            return directoryInfo;
        }
    }
}
