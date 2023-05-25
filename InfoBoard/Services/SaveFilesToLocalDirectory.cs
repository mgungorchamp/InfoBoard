using InfoBoard.Models;
//using Microsoft.Maui.Graphics;
//using Microsoft.UI.Xaml.Controls;
//using Windows.Media.Protection.PlayReady;
//using Windows.Storage.Streams;

namespace InfoBoard.Services
{
    internal class SaveFilesToLocalDirectory
    {
        public List<FileInformation> FileList;


        public async void fetchAndSave()
        {
            // Get the folder where the images are stored.
            string appDataPath = FileSystem.AppDataDirectory;

            string directoryName = Path.Combine(appDataPath, Constants.LocalDirectory);

            DirectoryInfo directoryInfo = Directory.CreateDirectory(directoryName);

            Task.Run(() => RetrieveImages()).Wait();
            //RetrieveImages();

            foreach (var file in FileList)
            {
                string localFileName = Path.Combine(directoryInfo.FullName, file.s3key);
                //TODO this needs to be streamed 
                HttpClient _client = new HttpClient();
                Uri uri = new Uri(file.presignedURL);
                byte[] fileContent = _client.GetByteArrayAsync(uri).Result;
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

        public void RetrieveImages()
        {
            FileList = new List<FileInformation>();

            RestService restService = new RestService();
            var task = restService.RefreshDataAsync();
            task.Wait();
            FileList = task.Result;

            //FileList = await restService.RefreshDataAsync();
        }

    }
}
