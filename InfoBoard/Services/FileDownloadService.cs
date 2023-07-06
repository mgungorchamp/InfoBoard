using InfoBoard.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using System.Collections;
using System.Linq;
using System.Text.Json; 
//using Microsoft.Maui.Graphics;
//using Microsoft.UI.Xaml.Controls;
//using Windows.Media.Protection.PlayReady;
//using Windows.Storage.Streams;

namespace InfoBoard.Services
{
    internal class FileDownloadService
    {
        //private List<FileInformation> categoryList = new List<FileInformation>(); 

        //Getting files from internet 
        //TODO: This should be optimized
        //If device lost power this process should not be repeated or WIFI connection lost
        //JSON category should be saved to local drive - and start displaying local pictutes if there are with the same name
        //If there is no picture with the same name, try to download all the picture from Internet 
        //Regardless it should compare the pictures from Internet with Local one to synchronise 
        // Delete the local one if it's no longer in the JSON
        // Redownload if the pull date is older than the category update date 
        // Download the category if there is no category with that in the local folder  

        /*
         If there is no local JSON category - PULL everything from server and store that JSON category to a local folder 
        Store the JSON category in a local folder 

        After 10 minutes request the NEW JSON FILE - 
        Compare JSON content  with the local one
        - DELETE: If a category no longer exists in the new JSON category - Delete that category from the local folder
        - Redownload if the pull date (need to store for each category?) is older than the category update date
        - Download a category if there is no category with that name in the local JSON category 
        - and overwrite the content of the local JSON category with the new JSON category   
         */

        //List<FileInformation> lastSavedFileList;
        private readonly ILogger _logger;

        public FileDownloadService()
        {
            _logger = Utilities.Logger(nameof(FileDownloadService));
        }

        public async Task<List<MediaCategory>> synchroniseMediaFiles()
        {
            List<MediaCategory> fileListFromLocal = await readMediaNamesFromLocalJSON();

            //No internet - return existing files
            if (!Utilities.isInternetAvailable())
            {
                return fileListFromLocal;
            }

            if (fileListFromLocal == null)
            {

                //Case 1: First time dowloading all the files 
                //If the local JSON category does not exist categoryList will be null
                //Then dowload all media files to local directory 
                //And save the JSON category to local directory
                _logger.LogInformation($"\tCase #1: First time dowloading all the files");
                return await downloadAllFilesFromServer();
            }
            else
            {
                _logger.LogInformation("\tCase #2: Synchronise files");
                return await oneWaySynchroniseFiles();
                //return readMediaNamesFromLocalJSON();
                // categoryList = readMediaNamesFromLocalJsonCheckDiscrepancy();
            }
        }

        private async Task<List<MediaCategory>> downloadAllFilesFromServer()
        {
            List<MediaCategory> categoryList = await getLatestMediaNamesFromServer();

            //Save those files to local directory
            await downloadMediaFilesToLocalDirectory(categoryList);

            return categoryList;
        }

        //Gets the latest media category names from the server(if there is internet) and saves to local category and reads it and returns the list
        //If no internet returns the local category
        private async Task<List<MediaCategory>> getLatestMediaNamesFromServer()
        {
            //Get category names from the server - fileListFromServer
            RestService restService = new RestService();
            await restService.updateMediaList();

            List<MediaCategory> fileList = await readMediaNamesFromLocalJSON();
            return fileList;
        }

        public List<Media> combineAllMediItemsFromCategory(List<MediaCategory> category)
        {
            List<Media> medias = new List<Media>();

            foreach (MediaCategory mediaCategory in category ?? Enumerable.Empty<MediaCategory>())
            {
                foreach (Media media in mediaCategory.media ?? Enumerable.Empty<Media>())
                {
                    medias.Add(media);
                }
            }
            return medias;
        }

        private async Task<List<MediaCategory>> oneWaySynchroniseFiles()
        {

            //If any of the local files missing - corrupted - try downloading all files
            List<MediaCategory> mediaListFromLocal = await readMediaNamesFromLocalJsonCheckDiscrepancy();            
            if (mediaListFromLocal == null)
            {
                return await downloadAllFilesFromServer();                
            }

            List<MediaCategory> fileListFromServer = await getLatestMediaNamesFromServer();

            //If fileListFromServer null just abort the operations
            //Since the device unregistered, and files has been deleted from local folder
            if (fileListFromServer == null) 
            {
                _logger.LogInformation($"#SYNC#2: fileListFromServer is null");
                return null; // NULL                 
            }

            //Find intersect files - if updated re-download them
            //https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.intersect?view=net-7.0
            //IEnumerable<FileInformation> both = categoryListFromLocal.Intersect(fileListFromServer);
            //foreach (FileInformation id in both)
            //{ 
            //TODO: DOWLOAD UPDATED FILES
            //Read local files last download or update date and compare it with from server
            //;//compare last update date with local date ... if different redownload
            //}

            // In the server but not at the local download those files
            // ref: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-find-the-set-difference-between-two-lists-linq

            //CASE 2 - Download NEW files
            //IEnumerable<MediaCategory> missingLocalFiles = fileListFromServer.Except(mediaListFromLocal);            
            //foreach (var category in missingLocalFiles ?? Enumerable.Empty<MediaCategory>())
            //{
            //    foreach (Media media in category.media ?? Enumerable.Empty<Media>())
            //    {
            //        await downloadMediaFileToLocalDirectory(media);
            //    }
            //}

            var mediListFromServer = combineAllMediItemsFromCategory(fileListFromServer);
            var mediListFromLocal = combineAllMediItemsFromCategory(mediaListFromLocal);

            //CASE 2 - Download NEW files
            IEnumerable<Media> missingLocalMedia = mediListFromServer.Except(mediListFromLocal);
            foreach (var media in missingLocalMedia ?? Enumerable.Empty<Media>())
            {
                await downloadMediaFileToLocalDirectory(media);
            }


            //First update the local JSON before deleting 
            //Any - true if the source sequence contains any elements; otherwise, false.
            // if (missingLocalFiles.Any() || filesDeletedFromServer.Any())
            //    await saveCategoryListToLocalJSON(fileListFromServer);

            //Now we can delete - since if JSON is not updated - it will try to view deleted category 
            //Delete after updating JSON

            //CASE 2 - DELETE removed files
            // In the local but not at the server delete those files
            // ref: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-find-the-set-difference-between-two-lists-linq

            IEnumerable<Media> mediaDeletedFromServer = mediListFromLocal.Except(mediListFromServer);
            foreach (var media in mediaDeletedFromServer ?? Enumerable.Empty<Media>())
            {
                deleteLocalMediaFile(media);
            }


            //IEnumerable<MediaCategory> filesDeletedFromServer = mediaListFromLocal.Except(fileListFromServer);
            //foreach (var category in filesDeletedFromServer ?? Enumerable.Empty<MediaCategory>())
            //{
            //    foreach (Media media in category.media ?? Enumerable.Empty<Media>())
            //    {
            //        deleteLocalMediaFile(media);
            //    }
            //}

            return fileListFromServer;
        }


        private async Task<List<MediaCategory>> readMediaNamesFromLocalJsonCheckDiscrepancy()
        {
            List<MediaCategory> categoryListFromLocal =  await readMediaNamesFromLocalJSON();
            // check if any of the local files missing - corrupted - return null
            // Get the folder where the images are stored.
            //string appDataPath = FileSystem.AppDataDirectory;
            //string directoryName = Path.Combine(appDataPath, Constants.LocalDirectory);
            foreach (var category in categoryListFromLocal ?? Enumerable.Empty<MediaCategory>())
            {
                foreach (Media media in category.media ?? Enumerable.Empty<Media>())
                {
                    //If the media type is not a file - continue
                    if(media.type != "file")
                    {
                        continue;
                    }

                    string fileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, media.s3key);
                    if (!File.Exists(fileName))
                    {
                        _logger.LogError($"#E2: Discrepancy  resetting the files - missing media:{fileName}");
                        return null; // Fresh start, since there is missing files                        
                    }
                }
            }
            return categoryListFromLocal;
        }
               

        private async Task downloadMediaFilesToLocalDirectory(List<MediaCategory> categoryList)
        {
            try
            {
                //All these saving should be ASYNC task list and we can waitall at the end
                //Download each category from the server to local folder
                foreach (var category in categoryList ?? Enumerable.Empty<MediaCategory>())
                {
                    foreach (Media media in category.media ?? Enumerable.Empty<Media>())
                    {
                        await downloadMediaFileToLocalDirectory(media);
                    }
                }
            } 
            catch(Exception ex) {
                Console.WriteLine("downloadMediaFilesToLocalDirectory Done: Download Exception: MURAT");
                _logger.LogError($"#77 downloadMediaFilesToLocalDirectory Download Exception: MURAT\n {ex.Message}");
            }
        }

        private async Task downloadMediaFileToLocalDirectory(Media media)
        {
            //If it's not a file - don't download
            if (media.type != "file")
            {
                return;
            }

            string localFullFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, media.s3key);

            //We are using s3key as the category name and its unique therefore
            // we don't need to dowload the category
            if (File.Exists(localFullFileName)) 
            {
                return;              
            }
            try {
                //Download the category content as byte array from presigned URL
                HttpClient httpClient = new HttpClient();
                Uri uri = new Uri(media.path);
                byte[] fileContent = await httpClient.GetByteArrayAsync(uri);
                //Save it to local folder
                await File.WriteAllBytesAsync(localFullFileName, fileContent);
                _logger.LogInformation($"\t#DOW#1: Downloading file: {media.s3key}");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"downloadMediaFileToLocalDirectory  has issues MURAT\n {ex.Message}");
                _logger.LogError($"#99-LOAD Exception downloadMediaFileToLocalDirectory\n {ex.Message}");
            }
        }

        private void deleteLocalMediaFile(Media media)
        {
            try
            {
                //If it's not a file, return
                if (media.type != "file")
                {
                    return;
                }
                //DirectoryInfo directoryInfo = getMediaFolder();
                string localFullFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, media.s3key);
                //Save it to local folder
                File.Delete(localFullFileName);
                _logger.LogInformation($"\t#DEL#1: Deleted local file: {media.s3key}");
            }
            catch(Exception ex)
            {
                Console.WriteLine("File cannot be deleted: deleteLocalMediaFile  has issues MURAT");
                _logger.LogError($"#33 File cannot be deleted: deleteLocalMediaFile  has issues MURAT\n {ex.Message}");
            }
        }
         
        //Ref: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-8-0
        public async Task saveCategoryListToLocalJSON(List<MediaCategory> fileList) 
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
                string jsonString = JsonSerializer.Serialize<List<MediaCategory>>(fileList);
                await File.WriteAllTextAsync(fullPathFileName, jsonString);
                //lastSavedFileList = categoryList;
            }
            catch(Exception ex)
            {
                Console.WriteLine("saveCategoryListToLocalJSON  has issues MURAT");
                _logger.LogError($"#11 saveCategoryListToLocalJSON  has issues MURAT\n {ex.Message}");
            }
        }

        public async Task resetMediaNamesInLocalJSonAndDeleteLocalFiles()
        {            
            try
            {
                //Delete all the files from local directory
                List<MediaCategory> fileListFromLocal = await readMediaNamesFromLocalJSON();
                foreach (MediaCategory category in fileListFromLocal ?? Enumerable.Empty<MediaCategory>()) //https://blog.jonschneider.com/2014/09/c-shorten-null-check-around-foreach.html?lr=1
                {
                    foreach (Media media in category.media ?? Enumerable.Empty<Media>())
                        deleteLocalMediaFile(media);
                }
                _logger.LogInformation($"# 88D All local files deleted");

                string fileName = "FileInformation.json";
                string fullPathFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileName);

                string jsonString = "RESETED";
                await File.WriteAllTextAsync(fullPathFileName, jsonString);
                //lastSavedFileList = null;

                _logger.LogInformation($"\n#55 Media File RESETTED\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("resetMediaNamesToLocalJSON  has issues MURAT");
                _logger.LogError($"#55 resetMediaNamesToLocalJSON  has issues MURAT\n {ex.Message}");
            }
        }

        //Read local JSON category - if exist - if not return empty categoryList
        private async Task<List<MediaCategory>> readMediaNamesFromLocalJSON()
        {
            //No need to read from local category, if this saved recently 
            //If not contine to read from category.
            //if(lastSavedFileList != null)
            //    return lastSavedFileList;

            JsonSerializerOptions _serializerOptions;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            List<MediaCategory> categoryList = null;
            try
            {
                string fileName = "FileInformation.json";
                string fullPathJsonFileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileName);

                if (File.Exists(fullPathJsonFileName))
                {
                    string jsonString = await File.ReadAllTextAsync(fullPathJsonFileName);
                    //Return - If all the pictures removed from the server but category exist in local directory
                    if (jsonString.Length < 50)
                        return null;

                    categoryList = JsonSerializer.Deserialize<List<MediaCategory>>(jsonString);
                }
                //No need to read again
                //lastSavedFileList = categoryList;
            }
            catch(Exception ex)
            {
                Console.WriteLine("readMediaNamesFromLocalJSON  has issues MURAT");
                _logger.LogError($"#22 readMediaNamesFromLocalJSON  has issues MURAT\n {ex.Message}");
            }
            return categoryList;
        }
    }
}
