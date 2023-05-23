using InfoBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoBoard.Services
{
    internal class SaveFilesToLocalDirectory
    {
        List<FileInformation> FileList;
        public void fetchAndSave() 
        {
            // Get the folder where the notes are stored.
            string appDataPath = FileSystem.AppDataDirectory;

            string directoryName = Path.Combine(appDataPath, Constants.LocalDirectory);

            DirectoryInfo directoryInfo = Directory.CreateDirectory(directoryName);

            Task.Run(() => RetrieveImages()).Wait();
            //RetrieveImages();

            foreach (var file in FileList)
            {
                string fileName = Path.Combine(directoryInfo.FullName, file.s3key);
                File.WriteAllText(fileName, file.presignedURL);
            }
        }

        private void RetrieveImages()
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
