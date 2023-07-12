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

        private string mediaSource;
        private string mediaInformation;
        private bool imageSourceVisible;
        private bool webViewVisible;
        private int _refreshInMiliSecond;
        private TimeSpan _cachingInterval; //caching interval
        public INavigation _navigation;

        private FileDownloadService fileDownloadService;
        
        public string MediaSource {
            get => mediaSource;
            set {
                if (mediaSource == value)
                    return;
                mediaSource = value;
                OnPropertyChanged();
            }
        }      

        public string MediaInformation {
            get => mediaInformation;
            set {
                if (mediaInformation == value)
                    return;
                mediaInformation = value;
                OnPropertyChanged();
            }
        }

        public bool ImageSourceVisible {
            get => imageSourceVisible;
            set {
                if (imageSourceVisible == value)
                    return;
                imageSourceVisible = value;
                OnPropertyChanged();
            }
        }

        public bool WebViewVisible {
            get => webViewVisible;
            set {
                if (webViewVisible == value)
                    return;
                webViewVisible = value;
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
            
            imageSourceVisible = true;
            webViewVisible = false;

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
            _logger.LogInformation("\n\n+++ STOP Timer 4 Files And DeviceSettings is called\n\n");
        }

        private void StartTimer4FilesAndDeviceSettings()
        {
            timer4FileSync.IsRepeating = true;
            timer4FileSync.Start();

            timer4DeviceSettingsSync.IsRepeating = true;
            timer4DeviceSettingsSync.Start();
            _logger.LogInformation("\n\n+++ START Timer 4 Files And DeviceSettings is called\n\n");
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
                //Reset all the files - if the device activated before
                _logger.LogInformation("\nReset local current files if the device used before to clean start\n");
                await fileDownloadService.resetMediaNamesInLocalJSonAndDeleteLocalFiles();
                //Navigate to RegisterView
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

        List<MediaCategory> categoryList;
        Media current, next;
        private async void SetupAndStartTimers()
        {

            categoryList = await fileDownloadService.synchroniseMediaFiles();
            //categoryList = fileDownloadService.readMediaNamesFromLocalJSON();

            //TODO SLEEP HERE TO WAIT FOR FILE DOWNLOAD
            //await Task.Delay(TimeSpan.FromSeconds(3));

            current = getRandomMedia();
            await DisplayImageEvent();
            

            //Set up the timer for Display Image
            timer4DisplayImage.Interval = TimeSpan.FromSeconds(5);
            timer4DisplayImage.Tick += async (sender, e) => await DisplayImageEvent();
            

            //Set up the timer for Syncronise Media Files             
            timer4FileSync.Interval = TimeSpan.FromSeconds(20);
            timer4FileSync.Tick += async (sender, e) => categoryList = await fileDownloadService.synchroniseMediaFiles();
            //timer4FileSync.Tick += (sender, e) => categoryList = fileDownloadService.readMediaNamesFromLocalJSON();


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

        public async Task GoToWebView()
        {
            Media mediaInfo = new Media();
            mediaInfo.path = "https://vermontic.com/";
            var navigationParameter = new Dictionary<string, object>
            {
                { "MediaInformationParam", mediaInfo }
            };
            //await Shell.Current.GoToAsync($"beardetails", navigationParameter);
            await Shell.Current.GoToAsync(nameof(WebSiteView), navigationParameter);
        }

        //bool showImage = true; 

        //int timing;
        
        private async Task DisplayImageEvent()//(object sender, EventArgs e)
        {
            current = getRandomMedia();

            MediaSource = getMediaPath(current);

            timer4DisplayImage.Interval = TimeSpan.FromSeconds(current.timing);

            MediaInformation = $"Source\t:{MediaSource}\n" +
                               $"Duration\t: {current.timing}\n" +
                               $"TimeSpan Timing:{timer4DisplayImage.Interval}";


            
            if (current.type == "file")
            {
                WebViewVisible = false;
                //webViewVisible = false;
                //OnPropertyChanged(nameof(WebViewVisible));
                //OnPropertyChanged(nameof(MediaSource));

                ImageSourceVisible = true;
                //imageSourceVisible = true;
                //OnPropertyChanged(nameof(ImageSourceVisible));
                //showImage = false;
            }
            else//IF WEBSITE
            {
                //If not internet, don't try to show websites.
                if (Utilities.isInternetAvailable())
                {
                    //Give some time website load
                    await Task.Delay(TimeSpan.FromSeconds(4));
                    ImageSourceVisible = false;
                    //OnPropertyChanged(nameof(ImageSourceVisible));
                    //OnPropertyChanged(nameof(MediaSource));

                    WebViewVisible = true;
                    //OnPropertyChanged(nameof(WebViewVisible));
                }
                
                //showImage = true;
                //  timer4DisplayImage.Interval = TimeSpan.FromSeconds(10);
                //  await GoToWebView();                
                //  await Task.Delay(TimeSpan.FromSeconds(10));
                //  showImage = true;
                //  await Shell.Current.GoToAsync(nameof(ImageDisplay));
            }

            //next = getRandomMedia();
            //timer4DisplayImage.Interval = TimeSpan.FromSeconds(next.timing);

            //current = next;
            

            //await Task.Delay(TimeSpan.FromSeconds(3));//It gives control to UI thread to update the UI


            //No settings found - register device and update deviceSettings
            if (deviceSettings == null)
            {
                await GoTime();
                return;
            }
            //If internet is not available stop file syncronisation
            if (!Utilities.isInternetAvailable() && timer4FileSync.IsRunning)
            {
                StopTimer4FilesAndDeviceSettings();  
            }
            else if (Utilities.isInternetAvailable() && !timer4FileSync.IsRunning)
            {
                StartTimer4FilesAndDeviceSettings();
            }
        }
  

        private static Random random = new Random();
        int index = 0;
        private Media getRandomMedia()
        {
            //TODO : File list should be a member variable and should be updated in a timed event
            //List<FileInformation> categoryList = fileDownloadService.readMediaNamesFromLocalJSON();
            try
            {
                List<Media> allMedia = fileDownloadService.combineAllMediItemsFromCategory(categoryList);

                //No files to show
                if (allMedia == null || allMedia.Count == 0)
                {
                    Debug.WriteLine("No files to show");
                    _logger.LogInformation($"\n\t #433 No files to show {nameof(ImageViewModel)}\n\n");
                    index = 0;
                    Media noMedia = new Media();
                    return noMedia;
                }
                if (index >= allMedia.Count)
                    index = 0;
                Media randomMedia = allMedia[index]; ;// allMedia[random.Next(allMedia.Count)];
                index++;
                return randomMedia;
            } 
            catch (Exception ex) 
            {
                _logger.LogError($"\n\t #411 Index exception ocurred {nameof(ImageViewModel)}\n " +
                                $"\tException {ex.Message}\n");
                Media noMedia = new Media();
                return noMedia;
            }
            

            //MediaCategory randomCategory = categoryList[random.Next(categoryList.Count)];
            //Media randomMedia;
            //if (randomCategory.current.Count > 0)
            //    return randomCategory.current[random.Next(randomCategory.current.Count)];
            //else 
            //    return getRandomMedia();

            //return randomMedia;
            // Pick some other picture
        }

        private string getMediaPath(Media media) 
        {
            if (media.type == "file")
            {
                string fileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, media.s3key);
                if (File.Exists(fileName))
                {
                    return fileName;
                }
                if (media.s3key == "uploadimage.png")
                {
                    return "uploadimage.png";
                }
                return "welcome.jpg"; // TODO : Missing image - image must have deleted from the local file
            }
            return media.path;
        }

         
        public async void ChangeImage()
        {
            mediaSource = "https://drive.google.com/uc?id=1D6omslsbfWey0cWa6NvBqeTI7yfGeVg8";
            //"https://innovation.wustl.edu/wp-content/uploads/2022/07/WashU-startup-wall-in-Cortex-Innovation-Community-768x512.jpg"; //https://picsum.photos/200/300
            OnPropertyChanged(nameof(MediaSource));
            await Task.Delay(_refreshInMiliSecond);

            //_imageSource = "https://drive.google.com/file/d/1D6omslsbfWey0cWa6NvBqeTI7yfGeVg8/view";

            //_imageSource = "https://gdurl.com/R-59";
            mediaSource = "https://lh3.googleusercontent.com/drive-viewer/AFGJ81qti0yDlD6Ph_LpUExWqh7lBDF10LrOXegbtMpz7yj-aC9vaVVhbbrA7R7b4NObrF39hLS0pseyuwtBERuTdpDS5cDE7g=s1600";
            OnPropertyChanged(nameof(MediaSource));
            await Task.Delay(_refreshInMiliSecond);
            /*
                        _imageSource = "https://aka.ms/campus.jpg"; //https://picsum.photos/200/300
                        OnPropertyChanged(nameof(ImageSource));
                        await Task.Delay(_refreshInMiliSecond);

                    /*
                        _imageSource = "https://vermontic.com/wp-content/uploads/2023/04/lake-champlain-scenic-water-204309-1024x768.jpg";
                        OnPropertyChanged(nameof(ImageSource));
                        await Task.Delay(_refreshInMiliSecond);

                        _imageSource = "https://current.cnn.com/api/v1/images/stellar/prod/230502171051-01-msg-misunderstood-ingredient-top.jpg";
                        OnPropertyChanged(nameof(ImageSource));
                        await Task.Delay(_refreshInMiliSecond);
                    */

            mediaSource = "https://www.champlain.edu/assets/images/Internships/Internships-Hero-Desktop-1280x450.jpg";
            OnPropertyChanged(nameof(MediaSource));
            await Task.Delay(_refreshInMiliSecond);


            mediaSource = "https://source.unsplash.com/random/1920x1080/?wallpaper,landscape,animals";
            OnPropertyChanged(nameof(MediaSource));
            await Task.Delay(_refreshInMiliSecond);            
        }

        public void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
