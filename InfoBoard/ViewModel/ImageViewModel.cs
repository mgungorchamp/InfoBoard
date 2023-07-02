using InfoBoard.Models;
using InfoBoard.Services;
using InfoBoard.Views;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;



namespace InfoBoard.ViewModel
{
    public partial class ImageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ILogger _logger;
        private IDispatcherTimer timer4DisplayImage;
        private IDispatcherTimer timer4FileSync;
        private IDispatcherTimer timer4DeviceSettingsSync;

        private string _imageSource;
        private int _refreshInMiliSecond;
        private TimeSpan _cachingInterval; //caching interval
        public INavigation _navigation;

        private FileDownloadService fileDownloadService;
        
        public string ImageSource {
            get => _imageSource;
            set {
                if (_imageSource == value)
                    return;
                _imageSource = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan CachingTime {
            get => _cachingInterval;
            set {
                if (_cachingInterval == value)
                    return;
                _cachingInterval = value;
                OnPropertyChanged();
            }
        }

        DeviceSettings deviceSettings;

        public INavigation NavigationSet {
            get => _navigation;
            set {
                if (_navigation == value)
                    return;
                _navigation = value;
            }
        }
        public ImageViewModel()
        {
            _logger = Utilities.Logger(nameof(ImageViewModel));
            fileDownloadService = new FileDownloadService();
            _cachingInterval = new TimeSpan(0, 0, 3, 00); // TimeSpan (int days, int hours, int minutes, int seconds);
            _refreshInMiliSecond = 3000;


            timer4DisplayImage = Application.Current?.Dispatcher.CreateTimer();
            timer4FileSync = Application.Current?.Dispatcher.CreateTimer();
            timer4DeviceSettingsSync = Application.Current?.Dispatcher.CreateTimer();
        }



        //public ImageViewModel(INavigation navigation)
        //{
        //    this.Navigation = navigation;           
        //    fileDownloadService = new FileDownloadService();
        //    _cachingInterval = new TimeSpan(0, 0, 3, 00); // TimeSpan (int days, int hours, int minutes, int seconds);
        //    _refreshInMiliSecond = 3000;

           
        //    timer4DisplayImage = Application.Current.Dispatcher.CreateTimer();
        //    timer4FileSync = Application.Current.Dispatcher.CreateTimer();
        //    timer4DeviceSettingsSync = Application.Current.Dispatcher.CreateTimer();            
           
        //    //GoTime();
        //}

        public async Task GoTimeNow()
        {
            _logger.LogInformation("\n\n+++ GoTimeNow() is called\n\n");
            await GoTime();
        }

        public void StopTimersNow()
        {
            timer4DisplayImage.IsRepeating = false;
            timer4DisplayImage.Stop();

            StopTimer4FilesAndDeviceSettings();
            _logger.LogInformation("\n\n+++ StopTimersNow() is called\n\n");
        }
        public void StartTimersNow()
        {
            timer4DisplayImage.IsRepeating = true;
            timer4DisplayImage.Start();

            StartTimer4FilesAndDeviceSettings();
            _logger.LogInformation("\n\n+++ StartTimersNow() is called\n\n");
        }

        private void StopTimer4FilesAndDeviceSettings() 
        {
            timer4FileSync.IsRepeating = false;
            timer4FileSync.Stop();

            timer4DeviceSettingsSync.IsRepeating = false;
            timer4DeviceSettingsSync.Stop();
        }

        private void StartTimer4FilesAndDeviceSettings()
        {
            timer4FileSync.IsRepeating = true;
            timer4FileSync.Start();

            timer4DeviceSettingsSync.IsRepeating = true;
            timer4DeviceSettingsSync.Start();
        }


        //[UnsupportedOSPlatform("iOS")]
        private async Task GoTime() 
        {
            Debug.WriteLine("\n\n+++ GoTime() is called\n\n");
            //Stop timer - if running
            StopTimersNow();

            deviceSettings = await UpdateDeviceSettingsEventAsync();           

            //No settings found - register device and update deviceSettings
            if (deviceSettings == null)
            {
                _logger.LogInformation("\n\n+++ No settings found - register device and update deviceSettings\n\n");
                await Shell.Current.GoToAsync(nameof(RegisterView));
            }
            else//Registered device - start timer for image display and file/settings sync
            {
                _logger.LogInformation("\n\n+++ Registered device - start timer for image display and file/settings sync\n\n"); 
                SetupAndStartTimers();              
            }
        }

        private async Task<DeviceSettings> UpdateDeviceSettingsEventAsync()
        {
            //Load Device Settings
            DeviceSettingsService deviceSettingsService = DeviceSettingsService.Instance;
            deviceSettings = await deviceSettingsService.loadDeviceSettings();
            return deviceSettings;
        } 

        //public async void NavigateToRegisterViewAndStartTimer4RegisteringDevice()
        //{
        //    //await MainThread.InvokeOnMainThreadAsync(async () =>
        //    //{
        //    //    await _navigation.PushAsync(new RegisterView(this), true);               
        //    //}); 

           
        //}

        List<FileInformation> fileList;
        private async void SetupAndStartTimers()
        {

            fileList = await fileDownloadService.synchroniseMediaFiles();
            //fileList = fileDownloadService.readMediaNamesFromLocalJSON();

            //TODO SLEEP HERE TO WAIT FOR FILE DOWNLOAD
            //await Task.Delay(TimeSpan.FromSeconds(3));

            await DisplayImageEvent();
           
            //Set up the timer for Display Image
            timer4DisplayImage.Interval = TimeSpan.FromSeconds(5);
            timer4DisplayImage.Tick += async (sender, e) => await DisplayImageEvent();
            

            //Set up the timer for Syncronise Media Files             
            timer4FileSync.Interval = TimeSpan.FromSeconds(20);
            timer4FileSync.Tick += async (sender, e) => fileList = await fileDownloadService.synchroniseMediaFiles();
            //timer4FileSync.Tick += (sender, e) => fileList = fileDownloadService.readMediaNamesFromLocalJSON();


            //await MainThread.InvokeOnMainThreadAsync(async () =>
            //{
            //    await _navigation.PopToRootAsync(true);
            //});

            //await Shell.Current.GoToAsync("imagedisplay");

            //StartTimer4DeviceSettings
            //Get latest settings from server - every 15 seconds
            timer4DeviceSettingsSync.Interval = TimeSpan.FromSeconds(15);
            timer4DeviceSettingsSync.Tick += async (sender, e) => await UpdateDeviceSettingsEventAsync();
            
            StartTimersNow();
        }


        private async Task DisplayImageEvent()//(object sender, EventArgs e)
        {
            _imageSource = getRandomImageName();
            OnPropertyChanged(nameof(ImageSource));

            await Task.Delay(TimeSpan.FromSeconds(3));//It gives control to UI thread to update the UI


            //No settings found - register device and update deviceSettings
            if (deviceSettings == null)
            {
                await GoTime();
                return;
            }
            //If internet is not available stop file syncronisation
            if (!UtilityServices.isInternetAvailable() && timer4FileSync.IsRunning)
            {
                StopTimer4FilesAndDeviceSettings();  
            }
            else if (UtilityServices.isInternetAvailable() && !timer4FileSync.IsRunning)
            {
                StartTimer4FilesAndDeviceSettings();
            }
        }
  

        private static Random random = new Random();
        private string getRandomImageName()
        {           
            //TODO : File list should be a member variable and should be updated in a timed event
            //List<FileInformation> fileList = fileDownloadService.readMediaNamesFromLocalJSON();

            //No files to show
            if ( fileList == null || fileList.Count == 0)
            {      
                Debug.WriteLine("No files to show");    
                return "uploadimage.png";
            }

            // Get the folder where the images are stored.
            //string appDataPath = FileSystem.AppDataDirectory;
            //string directoryName = Path.Combine(appDataPath, Constants.LocalDirectory);


            var fileInformation = fileList[random.Next(fileList.Count)];
            string fileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, fileInformation.s3key);            
            if (File.Exists(fileName))
            {
                return fileName;
            }
            return "welcome.jpg"; // TODO : Missing image - we should not come to this point
            // Pick some other picture
        }

         
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
        }

        public void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
