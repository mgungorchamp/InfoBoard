using InfoBoard.Models;
using System.Text.Json;
//using Microsoft.Maui.Graphics;
//using Microsoft.UI.Xaml.Controls;
//using Windows.Media.Protection.PlayReady;
//using Windows.Storage.Streams;

namespace InfoBoard.Services
{
    internal class FileDownloadService
    {
        //private List<FileInformation> fileList = new List<FileInformation>();
       
        DeviceSettings deviceSettings;

        public List<FileInformation> getFileList() 
        {

            //Get Device settings
            //TODO: If device ID is not present synchroniseMediaFiles SHOULD not be started
            DeviceSettingsService settingsService = DeviceSettingsService.Instance;
            deviceSettings = settingsService.loadDeviceSettings();
            
            //synchronise files 
            synchroniseMediaFiles();
            return readMediaNamesFromLocalJSON(); ;
        }


        //Getting files from internet 
        //TODO: This should be optimized
        //If device lost power this process should not be repeated or WIFI connection lost
        //JSON file should be saved to local drive - and start displaying local pictutes if there are with the same name
        //If there is no picture with the same name, try to download all the picture from Internet 
        //Regardless it should compare the pictures from Internet with Local one to synchronise 
        // Delete the local one if it's no longer in the JSON
        // Redownload if the pull date is older than the file update date 
        // Download the file if there is no file with that in the local folder  

        /*
         If there is no local JSON file - PULL everything from server and store that JSON file to a local folder 
        Store the JSON file in a local folder 

        After 10 minutes request the NEW JSON FILE - 
        Compare JSON content  with the local one
        - DELETE: If a file no longer exists in the new JSON file - Delete that file from the local folder
        - Redownload if the pull date (need to store for each file?) is older than the file update date
        - Download a file if there is no file with that name in the local JSON file 
        - and overwrite the content of the local JSON file with the new JSON file   
         */

        private void synchroniseMediaFiles()
        {
            List<FileInformation> fileListFromLocal = readMediaNamesFromLocalJSON();

            //Case 1: First time dowloading all the files 
            //If the local JSON file does not exist fileList will be null
            //Then dowload all media files to local directory 
            //And save the JSON file to local directory
            if (fileListFromLocal == null)
            {
                downloadAllFilesFromServer();
            }
            else
            {                 
                oneWaySynchroniseFiles();                
               // fileList = readMediaNamesFromLocalJsonCheckDiscrepancy();
            }
        }

        private void downloadAllFilesFromServer() 
        {
            List<FileInformation> fileList = null;

            //Get file names from the server - fileListFromServer
            Task.Run(() => fileList = getFileListFromServer()).Wait();

            if (fileList != null)
            {
                //Save those files to local directory
                Task.Run(() => downloadFilesToLocalDirectory(fileList)).Wait();
                //Save media file names (as JSON) to local folder 
                saveMediaNamesToLocalJSON(fileList);
            }
        }

        private void oneWaySynchroniseFiles()
        {

            //If any of the local files missing - corrupted - try downloading all files
            List<FileInformation> fileListFromLocal = readMediaNamesFromLocalJsonCheckDiscrepancy();            
            if (fileListFromLocal == null)
            {
                downloadAllFilesFromServer();
                return;
            }

            List<FileInformation> fileListFromServer = null;
            //Get file names from the server - fileListFromServer
            Task.Run(() => fileListFromServer = getFileListFromServer()).Wait();

            //If fileListFromServer null just abort the operations
            if (fileListFromServer == null)
                return;
                    


            //Find intersect files - if updated re-download them
            //https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.intersect?view=net-7.0
            //IEnumerable<FileInformation> both = fileListFromLocal.Intersect(fileListFromServer);
            //foreach (FileInformation id in both)
            //{ 
            //TODO: DOWLOAD UPDATED FILES
            //Read local files last download or update date and compare it with from server
            //;//compare last update date with local date ... if different redownload
            //}

            // In the server but not at the local download those files
            // ref: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-find-the-set-difference-between-two-lists-linq

            //CASE 2 - Download NEW files
            IEnumerable<FileInformation> missingLocalFiles = fileListFromServer.Except(fileListFromLocal);
            foreach (FileInformation file in missingLocalFiles)
            {
                downloadFileToLocalDirectory(file);
            }

            //CASE 2 - DELETE removed files
            // In the local but not at the server delete those files
            // ref: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-find-the-set-difference-between-two-lists-linq
            IEnumerable<FileInformation> filesDeletedFromServer = fileListFromLocal.Except(fileListFromServer);
            foreach (FileInformation file in filesDeletedFromServer)
            {
                deleteLocalFile(file);
            }

            //Any - true if the source sequence contains any elements; otherwise, false.
            if (missingLocalFiles.Any() || filesDeletedFromServer.Any())
                saveMediaNamesToLocalJSON(fileListFromServer);           
        }


        private List<FileInformation> readMediaNamesFromLocalJsonCheckDiscrepancy()
        {
            List<FileInformation> fileListFromLocal = readMediaNamesFromLocalJSON();
            // check if any of the local files missing - corrupted - return null
            // Get the folder where the images are stored.
            string appDataPath = FileSystem.AppDataDirectory;
            string directoryName = Path.Combine(appDataPath, Constants.LocalDirectory);
            foreach (var fileInformation in fileListFromLocal)
            {
                string fileName = Path.Combine(directoryName, fileInformation.s3key);
                if (!File.Exists(fileName))
                {
                    fileListFromLocal = null; // Fresh start, since there is missing files
                    break;
                }
            }
            return fileListFromLocal;
        }


        // If media folder is not created it creates, if exist it retuns the existing one
        private DirectoryInfo getMediaFolder() 
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

        private void downloadFilesToLocalDirectory(List<FileInformation> fileList)
        {
            //Download each file from the server to local folder
            foreach (var file in fileList)
            {
                downloadFileToLocalDirectory(file);
            }
            //Console.WriteLine("Done: fetchAndSave");
        }

        private void downloadFileToLocalDirectory(FileInformation fileInformation)
        {
            DirectoryInfo directoryInfo = getMediaFolder();
             
            string localFullFileName = Path.Combine(directoryInfo.FullName, fileInformation.s3key);

            //We are using s3key as the file name and its unique therefore
            // we don't need to dowload the file
            if (File.Exists(localFullFileName)) 
            {
                return;              
            }

            //Download the file content as byte array from presigned URL
            HttpClient httpClient = new HttpClient();
            Uri uri = new Uri(fileInformation.presignedURL);
            byte[] fileContent = httpClient.GetByteArrayAsync(uri).Result;
            //Save it to local folder
            File.WriteAllBytes(localFullFileName, fileContent);
        }

        private void deleteLocalFile(FileInformation fileInformation)
        {
            DirectoryInfo directoryInfo = getMediaFolder();
            string localFullFileName = Path.Combine(directoryInfo.FullName, fileInformation.s3key);
            //Save it to local folder
            File.Delete(localFullFileName);
        }

        public List<FileInformation> getFileListFromServer()
        {
           // fileListFromServer = new List<FileInformation>();
            RestService restService = new RestService();
            var task = restService.retrieveFileList();
            task.Wait();
            //fileListFromServer = task.Result;
            return task.Result;
            //FileList = await restService.RefreshDataAsync();
        }        

        //Ref: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-8-0
        private void saveMediaNamesToLocalJSON(List<FileInformation> fileList) 
        {
            JsonSerializerOptions _serializerOptions;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            string fileName = "FileInformation.json";
            string fullPathFileName = Path.Combine(getMediaFolder().FullName, fileName);
            string jsonString = JsonSerializer.Serialize<List<FileInformation>>(fileList);        
            File.WriteAllText(fullPathFileName, jsonString);
        }

        //Read local JSON file - if exist - if not return empty fileList
        private List<FileInformation> readMediaNamesFromLocalJSON()
        {
            List<FileInformation> fileList = null;
            JsonSerializerOptions _serializerOptions;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            string fileName = "FileInformation.json";
            string fullPathJsonFileName = Path.Combine(getMediaFolder().FullName, fileName);
            if(File.Exists(fullPathJsonFileName))
            {
                string jsonString = File.ReadAllText(fullPathJsonFileName);
                //Return - If all the pictures removed from the server but file exist in local directory
                if (jsonString.Length < 10) 
                    return null; 

                fileList = JsonSerializer.Deserialize<List<FileInformation>>(jsonString);
            }
            return fileList;
        }
    }
}
