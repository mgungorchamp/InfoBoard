using InfoBoard.Models;
using InfoBoard.Services;
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

        private string _webMediaSource;
        private string _imageMediaSource;
        private int _display_width;
        private string mediaInformation;
        private bool imageSourceVisible;
        private bool webViewVisible;
        private string imageName;
        private int imageTiming;
        private bool showNoInternetIcon;
        private bool _itemVisible;

        //private Media media;

        MediaManager manager = new MediaManager();

        public ImageViewModel()
        {
            _logger = Utilities.Logger(nameof(ImageViewModel));

            webViewVisible = false;
            imageSourceVisible = true;
            showNoInternetIcon = false;
            imageTiming = 2;
#if DEBUG
            _itemVisible = true;
#else
           _itemVisible = false;     
#endif

        }
        public void OnPropertyChanged([CallerMemberName] string name = null) =>
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        //public Media Media {
        //    get => media;
        //    set {
        //        if (media == value)
        //            return;
        //        media = value;               
        //        OnPropertyChanged();
        //    }
        //}

        //public async void displayMedia()
        //{
        //    MediaManager mediaManager = MediaManager.Instance;
        //    if (media.type == "file")
        //    {
        //        media.path = mediaManager.getMediaPath(media);
        //        _logger.LogInformation($"Navigating to Image View @: {media.name}");
        //        Debug.WriteLine($"Navigating to Image View @: {media.name}");

        //        WebViewVisible = false;

        //        WebMediaSource = media.path;
        //        ImageSourceVisible = true;

        //        //ImageName = media.name;
        //        //ImageTiming = media.timing;
        //        //MediaInformation = "IMAGE";         

        //        await mediaManager.DoDelay(media.timing);                
        //    }
        //    else//IF WEBSITE
        //    {
        //        //If not internet, don't try to show websites.
        //        if (Utilities.isInternetAvailable())
        //        {
        //            _logger.LogInformation($"Navigating to Web Media #: {media.name}");
        //            Debug.WriteLine($"Navigating to Web Media #: {media.name}");

        //            ImageSourceVisible = false;                

        //            WebMediaSource = media.path;
        //            //DisplayWidth = media.display_width;

        //            WebViewVisible = true;

        //            //ImageName = media.name;
        //            //ImageTiming = media.timing;
        //            MediaInformation = "WEB SITE";

        //            await mediaManager.DoDelay(media.timing);

        //        }
        //        else
        //        {
        //            //No Internet
        //            //MediaInformation += "\tNo internet connection!";                        
        //            await mediaManager.DoDelay(1);
        //        }
        //    }

        //}

        
        public async Task GoTimeNow(/*IHttpClientFactory _httpClientFactory*/)
        {
            _logger.LogInformation("\n\n+++ GoTimeNow() is called\n\n");

            //Utilities._httpClientFactory = _httpClientFactory;

            //MediaManager manager = MediaManager.Instance;
           
            manager.SetImageViewModel(this);
           
            //manager.SetHttpClientFactory(_httpClientFactory);
            

            //To start with a default image
            imageTiming = 3;
            _imageMediaSource = "welcome.jpg"; 
            WebViewVisible = false;
            ImageSourceVisible = true;

            await manager.GoTime();
        }

        public void StopTimersNow()
        {
            _logger.LogInformation("\n\n--- StopTimersNow4MediaDisplayAndFilesAndSettings() is called\n\n");
            //MediaManager manager = MediaManager.Instance;
            manager.StopTimersNow4MediaDisplayAndFilesAndSettings();
        }

        public string WebMediaSource {
            get => _webMediaSource;
            set {
                if (_webMediaSource == value)
                    return;
                _webMediaSource = value;
                OnPropertyChanged();
            }
        }

        public string ImageMediaSource {
            get => _imageMediaSource;
            set {
                if (_imageMediaSource == value)
                    return;
                _imageMediaSource = value;
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
        
        public bool ItemVisible {
            get => _itemVisible;
            set {
                if (_itemVisible == value)
                    return;
                _itemVisible = value;
                OnPropertyChanged();
            }
        }

        public bool ShowNoInternetIcon {
            get => showNoInternetIcon;
            set {
                if (showNoInternetIcon == value)
                    return;
                showNoInternetIcon = value;
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

        public string ImageName {
            get => imageName;
            set {
                if (imageName == value)
                    return;
                imageName = value;
                OnPropertyChanged();
            }
        }
        public int ImageTiming {
            get => imageTiming;
            set {
                if (imageTiming == value)
                    return;
                imageTiming = value;
                OnPropertyChanged();
            }
        }

        public int DisplayWidth {
            get => _display_width;
            set {
                if (_display_width == value)
                    return;
                _display_width = value;
                OnPropertyChanged();
            }
        }

    }
}





//    public void StartTimersNow()
//    {
//        timer4DisplayImage.IsRepeating = true;
//        timer4DisplayImage.Start();

//        StartTimer4FilesAndDeviceSettings();
//        _logger.LogInformation("\n\n+++ StartTimersNow4MediaDisplayAndFilesAndSettings() is called\n\n");
//    }

//    private void StopTimer4FilesAndDeviceSettings()
//    {
//        timer4FileSync.IsRepeating = false;
//        timer4FileSync.Stop();

//        timer4DeviceSettingsSync.IsRepeating = false;
//        timer4DeviceSettingsSync.Stop();
//        _logger.LogInformation("\n\n--- STOP Timer 4 Files And DeviceSettings is called\n\n");
//    }

//    private void StartTimer4FilesAndDeviceSettings()
//    {
//        timer4FileSync.IsRepeating = true;
//        timer4FileSync.Start();

//        timer4DeviceSettingsSync.IsRepeating = true;
//        timer4DeviceSettingsSync.Start();
//        _logger.LogInformation("\n\n+++ START Timer 4 Files And DeviceSettings is called\n\n");
//    }


//    //[UnsupportedOSPlatform("iOS")]
//    private async Task GoTime()
//    {
//        try
//        {
//            Debug.WriteLine("\n\n+++ GoTime() is called\n\n");
//            //Stop timer - if running
//            StopTimersNow();

//            deviceSettings = await UpdateDeviceSettingsEventAsync();

//            //No settings found - register device and update deviceSettings
//            if (deviceSettings == null)
//            {
//                //Reset all the files - if the device activated before
//                _logger.LogInformation("\nReset local media files if the device used before to clean start\n");
//                await fileDownloadService.resetMediaNamesInLocalJSonAndDeleteLocalFiles();
//                //Navigate to RegisterView
//                _logger.LogInformation("\n\n+++ No settings found - register device and update deviceSettings\n\n");
//                await Shell.Current.GoToAsync(nameof(RegisterView));

//            }
//            else//Registered device - start timer for image display and file/settings sync
//            {
//                _logger.LogInformation("\n\n+++ Registered device - start timer for image display and file/settings sync\n\n");
//                SetupAndStartTimers();
//            }
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError($"GoTime #396 Exception\n" +
//                            $"Exception: {ex.Message}");
//            await GoTime();
//        }
//    }

//    private async Task<DeviceSettings> UpdateDeviceSettingsEventAsync()
//    {
//        //Load Device Settings
//        DeviceSettingsService deviceSettingsService = DeviceSettingsService.Instance;
//        deviceSettings = await deviceSettingsService.loadDeviceSettings();
//        return deviceSettings;
//    }

//    private List<MediaCategory> categoryList;
//    public static List<Media> allMedia;
//    Media media;
//    private async Task UpdateMediaEventAsync()
//    {
//        //Update Device Settings
//        categoryList = await fileDownloadService.synchroniseMediaFiles();
//        allMedia = fileDownloadService.combineAllMediItemsFromCategory(categoryList);
//    }

//    private async void SetupAndStartTimers()
//    {
//        await UpdateMediaEventAsync();

//        //Set up the timer for Syncronise Media Files             
//        timer4FileSync.Interval = TimeSpan.FromSeconds(20);
//        timer4FileSync.Tick += async (sender, e) => await UpdateMediaEventAsync();

//        //StartTimer4DeviceSettings
//        //Get latest settings from server - every 15 seconds
//        timer4DeviceSettingsSync.Interval = TimeSpan.FromSeconds(15);
//        timer4DeviceSettingsSync.Tick += async (sender, e) => await UpdateDeviceSettingsEventAsync();

//        //TODO SLEEP HERE TO WAIT FOR FILE DOWNLOAD
//        //await Task.Delay(TimeSpan.FromSeconds(3));
//        //media = previousMedia = getMedia();
//        //Set up the timer for Display Image
//        //timer4MediaDisplaying.Interval = TimeSpan.FromSeconds(5);
//        //timer4MediaDisplaying.Tick += async (sender, e) => await DisplayMediaEvent();

//        //Start the timers
//        StartTimersNow();
//        await DisplayMediaEvent();
//    }

//    public async Task GoToWebView()
//    {
//        Media mediaInfo = new Media();
//        mediaInfo.path = "https://vermontic.com/";
//        var navigationParameter = new Dictionary<string, object>
//            {
//                { "MediaInformationParam", mediaInfo }
//            };
//        //await Shell.Current.GoToAsync($"beardetails", navigationParameter);
//        await Shell.Current.GoToAsync(nameof(WebSiteView), navigationParameter);
//    }

//    //bool showImage = true; 

//    //int timing;

//    private async Task DoDelay(int time)
//    {
//        await Task.Delay(TimeSpan.FromSeconds(time));
//    }

//    private async Task DisplayMediaEvent()//(object sender, EventArgs e)
//    {
//        try
//        {
//            if (allMedia.Count == 0)
//            {
//                StopTimersNow();
//                Information info = new Information();
//                info.Title = "Assign Media Categories to Your Device";
//                info.Message = "Welcome to GuzelBoard\n" +
//                    "Congratulations! If you're reading this message, it means you've successfully completed the device registration process. Well done!\n" +
//                    "There is one last step that requires your attention.\n" +
//                    "Assign the categories that will be displayed on your device, via our web portal.\nhttps://guzelboard.com";
//                var navigationParameter = new Dictionary<string, object>
//                {
//                    { "PickCategories", info }
//                };
//                await Shell.Current.GoToAsync(nameof(InformationView), true, navigationParameter);
//                await Task.Delay(TimeSpan.FromSeconds(10));
//                await Shell.Current.GoToAsync("..");
//                return;
//            }

//            media = getMedia();

//            ImageSourceVisible = WebViewVisible = false;

//            //timer4MediaDisplaying.Interval = TimeSpan.FromSeconds(previousMedia.timing);
//#if DEBUG
//            MediaInformation = $"Source\t:{getMediaPath(media)}\n" +
//                               $"Duration\t: {media.timing}";// +
//                                                                    //$"\nTimeSpan Timing:{timer4MediaDisplaying.Interval}";
//#endif
//            if (media.type == "file")
//            {
//                var navigationParameter = new Dictionary<string, object>
//                    {
//                        { "ImageMedia", media }
//                    };

//                media.path = getMediaPath(media);
//                await Shell.Current.GoToAsync(nameof(ImageViewer), true, navigationParameter);
//                //await Task.Delay(TimeSpan.FromSeconds(media.timing));
//                await DoDelay(media.timing);
//                await Shell.Current.GoToAsync("..");

//                /*
//                WebMediaSource = getMediaPath(media); // This has to be separte from website for offline situations si
//                //WebViewVisible = false;
//                setDisplayWidth();
//                ImageSourceVisible = true;
//                await Task.Delay(TimeSpan.FromSeconds(media.timing));*/
//            }
//            else//IF WEBSITE
//            {
//                //If not internet, don't try to show websites.
//                if (Utilities.isInternetAvailable())
//                {
//                    var navigationParameter = new Dictionary<string, object>
//                        {
//                            { "WebMedia", media }
//                        };

//                    await Shell.Current.GoToAsync(nameof(WebViewViewer), true, navigationParameter);
//                    //await Task.Delay(TimeSpan.FromSeconds(media.timing));
//                    await DoDelay(media.timing);
//                    await Shell.Current.GoToAsync("..");
//                    /*
//                    WebMediaSource = getMediaPath(media);
//                    //Give some time website load
//                    //await Task.Delay(TimeSpan.FromSeconds(1));
//                    //ImageSourceVisible = false;
//                    //await Task.Delay(TimeSpan.FromSeconds(1));
//                    setDisplayWidth();
//                    WebViewVisible = true;
//                    await Task.Delay(TimeSpan.FromSeconds(media.timing));*/
//                }
//                else
//                {
//                    MediaInformation += "\tNo internet connection!";
//                    await Task.Delay(TimeSpan.FromSeconds(1));
//                    //timer4MediaDisplaying.Interval = TimeSpan.FromSeconds(0);
//                }
//            }
//            //previousMedia = media;
//            // media = getMedia();

//            //await Task.Delay(TimeSpan.FromSeconds(3));//It gives control to UI thread to update the UI

//            //No settings found - register device and update deviceSettings
//            if (deviceSettings == null)
//            {
//                await GoTime();
//                return;
//            }
//            //If internet is not available stop file syncronisation
//            if (!Utilities.isInternetAvailable() && timer4FileSync.IsRunning)
//            {
//                StopTimer4FilesAndDeviceSettings();
//            }
//            else if (Utilities.isInternetAvailable() && !timer4FileSync.IsRunning)
//            {
//                StartTimer4FilesAndDeviceSettings();
//            }
//            await DisplayMediaEvent(); //RECURSIVE CALL
//        }
//        catch (Exception ex)
//        {
//            Debug.WriteLine($"#861 Exception: {ex.Message} {nameof(ImageViewModel)}");
//            _logger.LogError($"\n\t #861 Exception {ex.Message} {nameof(ImageViewModel)}\n\n");
//        }
//    }

//    private void setDisplayWidth()
//    {
//        if (media.display_width <= -1)
//        {
//            DisplayWidth = Utilities.maximumDisplayWidth;
//        }
//        else
//        {
//            DisplayWidth = media.display_width;
//        }
//    }


//    private static Random random = new Random();
//    int index = 0;
//    private Media getMedia()
//    {
//        //TODO : File list should be a member variable and should be updated in a timed event
//        //List<FileInformation> categoryList = fileDownloadService.readMediaNamesFromLocalJSON();
//        try
//        {

//            //No files to show
//            if (allMedia.Count == 0)
//            {
//                Debug.WriteLine("No files to show");
//                _logger.LogInformation($"\n\t #433 No files to show {nameof(ImageViewModel)}\n\n");
//                index = 0;
//                Media noMedia = new Media();
//                return noMedia;
//                //return null;
//            }
//            if (index >= allMedia.Count)
//                index = 0;
//            Media randomMedia = allMedia[index]; ;// allMedia[random.Next(allMedia.Count)];
//            index++;
//            return randomMedia;
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError($"\n\t #411 Index exception ocurred {nameof(ImageViewModel)}\n " +
//                            $"\tException {ex.Message}\n");
//            Media noMedia = new Media();
//            return noMedia;
//        }


//        //MediaCategory randomCategory = categoryList[random.Next(categoryList.Count)];
//        //Media randomMedia;
//        //if (randomCategory.media.Count > 0)
//        //    return randomCategory.media[random.Next(randomCategory.media.Count)];
//        //else 
//        //    return getMedia();

//        //return randomMedia;
//        // Pick some other picture
//    }

//    private string getMediaPath(Media media)
//    {
//        if (media.type == "file")
//        {
//            string fileName = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, media.s3key);
//            if (File.Exists(fileName))
//            {
//                return fileName;
//            }
//            if (media.s3key == "uploadimage.png")
//            {
//                return "uploadimage.png";
//            }
//            return "welcome.jpg"; // TODO : Missing image - image must have deleted from the local file
//        }
//        return media.path;
//    }


//    public async void ChangeImage()
//    {
//        _webMediaSource = "https://drive.google.com/uc?id=1D6omslsbfWey0cWa6NvBqeTI7yfGeVg8";
//        //"https://innovation.wustl.edu/wp-content/uploads/2022/07/WashU-startup-wall-in-Cortex-Innovation-Community-768x512.jpg"; //https://picsum.photos/200/300
//        OnPropertyChanged(nameof(WebMediaSource));
//        await Task.Delay(_refreshInMiliSecond);

//        //_imageSource = "https://drive.google.com/file/d/1D6omslsbfWey0cWa6NvBqeTI7yfGeVg8/view";

//        //_imageSource = "https://gdurl.com/R-59";
//        _webMediaSource = "https://lh3.googleusercontent.com/drive-viewer/AFGJ81qti0yDlD6Ph_LpUExWqh7lBDF10LrOXegbtMpz7yj-aC9vaVVhbbrA7R7b4NObrF39hLS0pseyuwtBERuTdpDS5cDE7g=s1600";
//        OnPropertyChanged(nameof(WebMediaSource));
//        await Task.Delay(_refreshInMiliSecond);
//        /*
//                    _imageSource = "https://aka.ms/campus.jpg"; //https://picsum.photos/200/300
//                    OnPropertyChanged(nameof(ImageSource));
//                    await Task.Delay(_refreshInMiliSecond);

//                /*
//                    _imageSource = "https://vermontic.com/wp-content/uploads/2023/04/lake-champlain-scenic-water-204309-1024x768.jpg";
//                    OnPropertyChanged(nameof(ImageSource));
//                    await Task.Delay(_refreshInMiliSecond);

//                    _imageSource = "https://media.cnn.com/api/v1/images/stellar/prod/230502171051-01-msg-misunderstood-ingredient-top.jpg";
//                    OnPropertyChanged(nameof(ImageSource));
//                    await Task.Delay(_refreshInMiliSecond);
//                */

//        _webMediaSource = "https://www.champlain.edu/assets/images/Internships/Internships-Hero-Desktop-1280x450.jpg";
//        OnPropertyChanged(nameof(WebMediaSource));
//        await Task.Delay(_refreshInMiliSecond);


//        _webMediaSource = "https://source.unsplash.com/random/1920x1080/?wallpaper,landscape,animals";
//        OnPropertyChanged(nameof(WebMediaSource));
//        await Task.Delay(_refreshInMiliSecond);
//    }


//}

//public ImageViewModel(INavigation navigation)
//{
//    this.Navigation = navigation;           
//    fileDownloadService = new FileDownloadService();
//    _cachingInterval = new TimeSpan(0, 0, 3, 00); // TimeSpan (int days, int hours, int minutes, int seconds);
//    _refreshInMiliSecond = 3000;


//    timer4MediaDisplaying = Application.Current.Dispatcher.CreateTimer();
//    timer4FileSync = Application.Current.Dispatcher.CreateTimer();
//    timer4DeviceSettingsSync = Application.Current.Dispatcher.CreateTimer();            

//    //GoTime();
//}

//public async void NavigateToRegisterViewAndStartTimer4RegisteringDevice()
//{
//    //await MainThread.InvokeOnMainThreadAsync(async () =>
//    //{
//    //    await _navigation.PushAsync(new RegisterView(this), true);               
//    //}); 


//}