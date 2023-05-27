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

        public static string TEMPORARY_CODE = getHandshakeKey();

        //https://guzelboard.com/api/handshake.php?temporary_code=a3b8z2&device_type=MVP&version=1
        public static string HANDSHAKE_URL = $"{Scheme}://{HostUrl}/api/handshake.php?temporary_code={TEMPORARY_CODE}&device_type=MVP&version=1";

        //public static string Port = "5001";
        //public static string RestUrl = $"{Scheme}://{LocalhostUrl}:{Port}/api/todoitems/{{0}}";



        // Random alphanumeric string to use as a security key for the handshake
        public static string getHandshakeKey()
        {

            Random random = new Random();

            // String that contain both alphabets and numbers
            String keyPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int size = 8;

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

            return randomID;

        }
    }
}
