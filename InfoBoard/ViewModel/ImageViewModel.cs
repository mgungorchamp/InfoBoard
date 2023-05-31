using InfoBoard.Models;
using InfoBoard.Services;
using Microsoft.Maui.Storage;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace InfoBoard.ViewModel
{
    partial class ImageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _imageSource;
        private int _refreshInMiliSecond;
        private TimeSpan _cachingInterval; //caching interval

        private FileDownloadService fileDownloadService = new FileDownloadService();

        //I think to be deleted
        List<FileInformation> FileList = new List<FileInformation>();

        public string ImageSource {
            get => _imageSource;
            set {
                if (_imageSource == value)
                    return;
                _imageSource = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan Interval {
            get => _cachingInterval;
            set {
                if (_cachingInterval == value)
                    return;
                _cachingInterval = value;
                OnPropertyChanged();
            }
        }

        public ImageViewModel()
        {
            _cachingInterval = new TimeSpan(0, 0, 0, 00); // TimeSpan (int days, int hours, int minutes, int seconds);
            _refreshInMiliSecond = 3000;
            // Task.Run(() => RetrieveImages()).Wait();
            // DisplayAnImageEachTimelapse();
           
            DisplayAnImageFromLocalFolder();
        }


        private async void DisplayAnImageFromLocalFolder()
        {
            // Get the folder where the images are stored.
            string appDataPath = FileSystem.AppDataDirectory;
            string directoryName = Path.Combine(appDataPath, Constants.LocalDirectory);

            //string fileNames = Directory.GetFiles(directoryName);

            //fileDownloadService.updateFiles();

            //Task.Run(() => fileDownloadService.getMediaFileNamesFromServer()).Wait();
            //RetrieveImages();

            // TODO: getFileList should be TIMED EVENT not everytime - can be every 10 mins? or more
            // CAN BE PUT INTO SETTINGS TOO  - File Snyc Frequency

            List<FileInformation> fileList = fileDownloadService.getFileList();
            //No files to show
            if ( fileList.Count == 0)
            {
                _imageSource = "uploadimage.png";
                OnPropertyChanged(nameof(ImageSource));
                return;
            }

            foreach (var fileInformation in fileList)
            {
                string fileName = Path.Combine(directoryName, fileInformation.s3key);
                if (File.Exists(fileName))
                {
                    _imageSource = fileName;
                    OnPropertyChanged(nameof(ImageSource));
                    await Task.Delay(_refreshInMiliSecond);
                }
            }

            DisplayAnImageFromLocalFolder();
        }
         

        private async void DisplayAnImageEachTimelapse()
        {
            foreach (var file in FileList)
            {
                _imageSource = file.presignedURL;
                OnPropertyChanged(nameof(ImageSource));
                await Task.Delay(_refreshInMiliSecond);
            }
            DisplayAnImageEachTimelapse();
        }

        public async void RetrieveImages()
        {
            RestService restService = new RestService();

            //Get Device settings
            //Task.Run(() => FileList = restService.downloadMediaFileNames().Result).Wait();
            FileList = await (restService.retrieveFileList());

            // var task = restService.downloadMediaFileNames();
            // task.Wait();
            // FileList = task.Result;
            //FileList = await restService.RefreshDataAsync();
        }

        //https://www.labnol.org/embed/google/drive/
        //https://www.labnol.org/google-drive-image-hosting-220515
        //https://gdurl.com/
        /*
         Google API to get list of shared files
         
         
         
         */
        public async void ChangeImage()
        {
            _imageSource = "https://drive.google.com/uc?id=1D6omslsbfWey0cWa6NvBqeTI7yfGeVg8";
            //"https://innovation.wustl.edu/wp-content/uploads/2022/07/WashU-startup-wall-in-Cortex-Innovation-Community-768x512.jpg"; //https://picsum.photos/200/300
            OnPropertyChanged(nameof(ImageSource));
            await Task.Delay(_refreshInMiliSecond);

            //_imageSource = "https://drive.google.com/file/d/1D6omslsbfWey0cWa6NvBqeTI7yfGeVg8/view";

            //_imageSource = "https://gdurl.com/R-59";
            _imageSource = "https://lh3.googleusercontent.com/drive-viewer/AFGJ81qti0yDlD6Ph_LpUExWqh7lBDF10LrOXegbtMpz7yj-aC9vaVVhbbrA7R7b4NObrF39hLS0pseyuwtBERuTdpDS5cDE7g=s1600";
            OnPropertyChanged(nameof(ImageSource));
            await Task.Delay(_refreshInMiliSecond);
            /*
                        _imageSource = "https://aka.ms/campus.jpg"; //https://picsum.photos/200/300
                        OnPropertyChanged(nameof(ImageSource));
                        await Task.Delay(_refreshInMiliSecond);

                    /*
                        _imageSource = "https://vermontic.com/wp-content/uploads/2023/04/lake-champlain-scenic-water-204309-1024x768.jpg";
                        OnPropertyChanged(nameof(ImageSource));
                        await Task.Delay(_refreshInMiliSecond);

                        _imageSource = "https://media.cnn.com/api/v1/images/stellar/prod/230502171051-01-msg-misunderstood-ingredient-top.jpg";
                        OnPropertyChanged(nameof(ImageSource));
                        await Task.Delay(_refreshInMiliSecond);
                    */

            _imageSource = "https://www.champlain.edu/assets/images/Internships/Internships-Hero-Desktop-1280x450.jpg";
            OnPropertyChanged(nameof(ImageSource));
            await Task.Delay(_refreshInMiliSecond);


            _imageSource = "https://source.unsplash.com/random/1920x1080/?wallpaper,landscape,animals";
            OnPropertyChanged(nameof(ImageSource));
            await Task.Delay(_refreshInMiliSecond);

            ChangeImage();
        }

        public void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
