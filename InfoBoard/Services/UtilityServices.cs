using System.Net.NetworkInformation;

namespace InfoBoard.Services
{
    internal class UtilityServices
    {
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
                catch(Exception) {
                    ; // Do nothing
                } 
            }
            return false;
        }
    }
}
