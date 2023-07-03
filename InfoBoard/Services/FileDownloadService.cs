using InfoBoard.Models;
using Microsoft.Extensions.Logging;
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

        List<FileInformation> lastSavedFileList;
        private readonly ILogger _logger;

        public FileDownloadService()
        {
            _logger = Utilities.Logger(nameof(FileDownloadService));
        }

        public async Task<List<FileInformation>> synchroniseMediaFiles()
        {
            List<FileInformation> fileListFromLocal = await readMediaNamesFromLocalJSON();

            //No internet - return existing files
            if (!Utilities.isInternetAvailable())
            {
                return fileListFromLocal;
            }

            if (fileListFromLocal == null)
            {

                //Case 1: First time dowloading all the files 
                //If the local JSON file does not exist fileList will be null
                //Then dowload all media files to local directory 
                //And save the JSON file to local directory
                _logger.LogInformation($"**Case 1: First time dowloading all the files");
                return await downloadAllFilesFromServer();
            }
            else
            {
                _logger.LogInformation("--Case 2: Synchronise files");
                return await oneWaySynchroniseFiles();
                //return readMediaNamesFromLocalJSON();
                // fileList = readMediaNamesFromLocalJsonCheckDiscrepancy();
            }
        }

        private async Task<List<FileInformation>> downloadAllFilesFromServer() 
        {
            //Get file names from the server - fileListFromServer
            RestService restService = new RestService();
            List<FileInformation> fileList = await restService.retrieveFileList();            

            if (fileList != null)
            {
                //Save those files to local directory
                await downloadFilesToLocalDirectory(fileList);
                //Save media file names (as JSON) to local folder 
                await saveMediaNamesToLocalJSON(fileList);
               
            }
            return fileList;
        }

        private async Task<List<FileInformation>> oneWaySynchroniseFiles()
        {

            //If any of the local files missing - corrupted - try downloading all files
            List<FileInformation> fileListFromLocal = await readMediaNamesFromLocalJsonCheckDiscrepancy();            
            if (fileListFromLocal == null)
            {
                return await downloadAllFilesFromServer();                
            }
          
            //Get file names from the server - fileListFromServer
            RestService restService = new RestService();
            List<FileInformation> fileListFromServer = await restService.retrieveFileList();

            //If fileListFromServer null just abort the operations, delete all the existing files
            //Since the device unregistered
            if (fileListFromServer == null) 
            {
                if (Utilities.isInternetAvailable()) 
                {
                    //Before deleting all the files - check if the device is registered
                    //DeviceSettingsService deviceSettingsService = DeviceSettingsService.Instance;
                    //DeviceSettings deviceSettings = await deviceSettingsService.loadDeviceSettings();
                    //if (deviceSettings != null) 
                    //{
                    //    return fileListFromLocal;                    
                    //}

                    //Delete all the files from local directory
                    foreach (FileInformation file in fileListFromLocal)
                    {                        
                        deleteLocalFile(file);
                    }
                    _logger.LogInformation($"# 88D All the files deleted");
                    //EMPTY the local file list 
                    List<FileInformation> fileList = new List<FileInformation>();
                    await saveMediaNamesToLocalJSON(fileList);
                    return null; // NULL 
                }                
            }

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
                await downloadFileToLocalDirectory(file);
            }

            //CASE 2 - DELETE removed files
            // In the local but not at the server delete those files
            // ref: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-find-the-set-difference-between-two-lists-linq
            IEnumerable<FileInformation> filesDeletedFromServer = fileListFromLocal.Except(fileListFromServer);
       
            //First update the local JSON before deleting 
            //Any - true if the source sequence contains any elements; otherwise, false.
            if (missingLocalFiles.Any() || filesDeletedFromServer.Any())
                await saveMediaNamesToLocalJSON(fileListFromServer);

            //Now we can delete - since if JSON is not updated - it will try to view deleted file 
            //Delete after updating JSON
            foreach (FileInformation file in filesDeletedFromServer)
            {                
                deleteLocalFile(file);
            }

            return await readMediaNamesFromLocalJSON();
        }


        private async Task<List<FileInformation>> readMediaNamesFromLocalJsonCheckDiscrepancy()
        {
            List<FileInformation> fileListFromLocal =  await readMediaNamesFromLocalJSON();
            // check if any of the local files missing - corrupted - return null
            // Get the folder where the images are stored.
            //string appDataPath = FileSystem.AppDataDirectory;
            //string directoryName = Path.Combine(appDataPath, Constants.LocalDirectory);
            foreach (var fileInformation in fileListFromLocal)
            {
                string fileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileInformation.s3key);
                if (!File.Exists(fileName))
                {
                    _logger.LogError($"#E2: Discrepancy  resetting the files - missing file {fileName}");
                    fileListFromLocal = null; // Fresh start, since there is missing files
                    break;
                }
            }
            return fileListFromLocal;
        }
               

        private async Task downloadFilesToLocalDirectory(List<FileInformation> fileList)
        {
            try
            {
                //All these saving should be ASYNC task list and we can waitall at the end
                //Download each file from the server to local folder
                foreach (var file in fileList)
                {
                    await downloadFileToLocalDirectory(file);
                }
            } 
            catch(Exception ex) {
                Console.WriteLine("downloadFilesToLocalDirectory Done: Download Exception: MURAT");
                _logger.LogError($"#77 downloadFilesToLocalDirectory Download Exception: MURAT\n {ex.Message}");
            }
        }

        private async Task downloadFileToLocalDirectory(FileInformation fileInformation)
        {
            //DirectoryInfo directoryInfo = getMediaFolder();
             
            string localFullFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileInformation.s3key);

            //We are using s3key as the file name and its unique therefore
            // we don't need to dowload the file
            if (File.Exists(localFullFileName)) 
            {
                return;              
            }
            try {
                //Download the file content as byte array from presigned URL
                HttpClient httpClient = new HttpClient();
                Uri uri = new Uri(fileInformation.presignedURL);
                byte[] fileContent = await httpClient.GetByteArrayAsync(uri);
                //Save it to local folder
                await File.WriteAllBytesAsync(localFullFileName, fileContent);
                _logger.LogInformation($"Downloading file: {fileInformation.name}");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"downloadFileToLocalDirectory  has issues MURAT\n {ex.Message}");
                _logger.LogError($"#66 Exception downloadFileToLocalDirectory\n {ex.Message}");
            }
        }

        private void deleteLocalFile(FileInformation fileInformation)
        {
            try
            {
                _logger.LogInformation($"#D1: Deleting local file: {fileInformation.name}");
                //DirectoryInfo directoryInfo = getMediaFolder();
                string localFullFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileInformation.s3key);
                //Save it to local folder
                File.Delete(localFullFileName);
            }
            catch(Exception ex)
            {
                Console.WriteLine("File cannot be deleted: deleteLocalFile  has issues MURAT");
                _logger.LogError($"#33 File cannot be deleted: deleteLocalFile  has issues MURAT\n {ex.Message}");
            }
        }
         
        //Ref: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-8-0
        private async Task saveMediaNamesToLocalJSON(List<FileInformation> fileList) 
        {
            JsonSerializerOptions _serializerOptions;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            try
            {
                string fileName = "FileInformation.json";
                string fullPathFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileName);
                string jsonString = JsonSerializer.Serialize<List<FileInformation>>(fileList);
                await File.WriteAllTextAsync(fullPathFileName, jsonString);
                lastSavedFileList = fileList;
            }
            catch(Exception ex)
            {
                Console.WriteLine("saveMediaNamesToLocalJSON  has issues MURAT");
                _logger.LogError($"#11 saveMediaNamesToLocalJSON  has issues MURAT\n {ex.Message}");
            }
        }

        //Read local JSON file - if exist - if not return empty fileList
        private async Task<List<FileInformation>> readMediaNamesFromLocalJSON()
        {
            //No need to read from local file, if this saved recently 
            //If not contine to read from file.
            if(lastSavedFileList != null)
                return lastSavedFileList;

            JsonSerializerOptions _serializerOptions;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            List<FileInformation> fileList = null;
            try
            {
                string fileName = "FileInformation.json";
                string fullPathJsonFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileName);

                if (File.Exists(fullPathJsonFileName))
                {
                    string jsonString = await File.ReadAllTextAsync(fullPathJsonFileName);
                    //Return - If all the pictures removed from the server but file exist in local directory
                    if (jsonString.Length < 20)
                        return null;

                    fileList = JsonSerializer.Deserialize<List<FileInformation>>(jsonString);
                }
                //No need to read again
                lastSavedFileList = fileList;
            }
            catch(Exception ex)
            {
                Console.WriteLine("readMediaNamesFromLocalJSON  has issues MURAT");
                _logger.LogError($"#22 readMediaNamesFromLocalJSON  has issues MURAT\n {ex.Message}");
            }
            return fileList;
        }
    }
}
