using InfoBoard.Models;
//using Microsoft.Maui.Graphics;
//using Microsoft.UI.Xaml.Controls;
//using Windows.Media.Protection.PlayReady;
//using Windows.Storage.Streams;

namespace InfoBoard.Services
{
    internal class FileDownloadService
    {
        public List<FileInformation> FileList;


        public async void downloadMediaFiles()
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

            //Get file names from the server
            Task.Run(() => getMediaFileNames()).Wait();
            
            //Download each file from the server to local folder
            foreach (var file in FileList)
            {
                string localFileName = Path.Combine(directoryInfo.FullName, file.s3key);
                
                //Download the file content as byte array from presigned URL
                HttpClient _client = new HttpClient();
                Uri uri = new Uri(file.presignedURL);
                byte[] fileContent = _client.GetByteArrayAsync(uri).Result;
                //Save it to local folder
                File.WriteAllBytes(localFileName, fileContent);

                /*
                using (Stream s = _client.GetStreamAsync(uri).Result)
                using (StreamReader sr = new StreamReader(s))
                {
                     File.WriteAllText(localFileName, sr.ReadToEnd());
                }*/

            }

            //Console.WriteLine("Done: fetchAndSave");
        }

        public void getMediaFileNames()
        {
            FileList = new List<FileInformation>();
            RestService restService = new RestService();
            var task = restService.downloadMediaFileNames();
            task.Wait();
            FileList = task.Result;
            //FileList = await restService.RefreshDataAsync();
        }

    }
}
