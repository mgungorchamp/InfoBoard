using InfoBoard.Models;
using InfoBoard.ViewModel;
using InfoBoard.Views;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime;

namespace InfoBoard.Services
{
    public class MediaManager
    {
        private readonly ILogger _logger;
        //Ref: https://www.ilkayilknur.com/a-new-modern-timer-api-in-dotnet-6-periodictimer
        //Ref https://github.com/dotnet/maui/wiki/Memory-Leaks
        //****private IDispatcherTimer timer4MediaDisplaying;
        //private IDispatcherTimer timer4FileSync;
        //private IDispatcherTimer timer4DeviceSettingsSync;
        //private IDispatcherTimer timer4InternetCheck;
        //
        PeriodicTimer timer4MediaAndSettings;

        private DeviceSettings deviceSettings;
        private FileDownloadService fileDownloadService;

        private List<MediaCategory> categoryList;
        private List<Media> allMedia;
        private Media currentMedia;
        private ImageViewModel imageViewModel;
        private ImageDisplay imageDisplay;

        private static readonly MediaManager instance = new MediaManager();

        public static MediaManager Instance {
            get {
                return instance;
            }
        }
        static MediaManager()
        {
        }

        private MediaManager()
        {
            _logger = Utilities.Logger(nameof(MediaManager));
            //fileDownloadService = FileDownloadService.Instance;
            fileDownloadService = new FileDownloadService();


            //timer4MediaDisplaying = Application.Current?.Dispatcher.CreateTimer();
            //timer4FileSync = Application.Current?.Dispatcher.CreateTimer();
            //timer4DeviceSettingsSync = Application.Current?.Dispatcher.CreateTimer();
            //timer4InternetCheck = Application.Current?.Dispatcher.CreateTimer();
        }

        public void SetImageViewModel(ImageViewModel imageViewModel)
        {
            this.imageViewModel = imageViewModel;
        }

        public void SetImageDisplay(ImageDisplay imageDisplay)
        {
            this.imageDisplay = imageDisplay;
        }

        //public IHttpClientFactory _httpClientFactory;
        //public void SetHttpClientFactory(IHttpClientFactory httpClientFactory)
        //{
        //    _httpClientFactory = httpClientFactory;
        //    //fileDownloadService.SetHttpClientFactoryToCreateHttpClient(httpClientFactory);  
        //}

        private void StartTimersNow4MediaDisplayAndFilesAndSettings()
        {
            _logger.LogInformation("\t\t+++ START StartTimersNow4MediaDisplayAndFilesAndSettings() is called");

            //timer4MediaDisplaying.IsRepeating = true;
            //timer4MediaDisplaying.Start();

            StartTimer4FilesAndDeviceSettings();
        }
        public void StopTimersNow4MediaDisplayAndFilesAndSettings()
        {
            _logger.LogInformation("\t\t--- STOP StopTimersNow4MediaDisplayAndFilesAndSettings() is called");

            //timer4MediaDisplaying.IsRepeating = false;
            //timer4MediaDisplaying.Stop();

            StopTimer4FilesAndDeviceSettings();
        }

        private void StopTimer4FilesAndDeviceSettings()
        {

            //timer4FileSync.IsRepeating = false;
            //timer4FileSync.Stop();

            //timer4DeviceSettingsSync.IsRepeating = false;
            //timer4DeviceSettingsSync.Stop();


            timer4MediaAndSettings?.Dispose();
            timer4MediaAndSettings = null;


            _logger.LogInformation("\t\t--- STOP Timer 4 Files And DeviceSettings is called\n");
        }

        private async void StartTimer4FilesAndDeviceSettings()
        {
            _logger.LogInformation("\t\t+++ START Timer 4 Files And DeviceSettings is called\n");

            timer4MediaAndSettings = new PeriodicTimer(TimeSpan.FromSeconds(33));

            // Wire it to fire an event after the specified period
            while (timer4MediaAndSettings != null && await timer4MediaAndSettings.WaitForNextTickAsync())
            {
                //Console.WriteLine($"Firing at {DateTime.Now}");
                await UpdateMediaEventAsync();
                await DoDelay(3);
                await UpdateDeviceSettingsEventAsync();
            }

            //Get latest settings from server - every 15 seconds
            //timer4DeviceSettingsSync.Interval = TimeSpan.FromSeconds(107);//107 29
            //timer4DeviceSettingsSync.Tick += async (sender, e) => await UpdateDeviceSettingsEventAsync(); 

            //timer4FileSync.IsRepeating = true;
            //timer4FileSync.Start();

            //timer4DeviceSettingsSync.IsRepeating = true;
            //timer4DeviceSettingsSync.Start();
            //_logger.LogInformation("\t\t+++ START Timer 4 Files And DeviceSettings is called\n");
        }

        private async void StartTimer4InternetCheck()
        {
            using (var timer = new PeriodicTimer(TimeSpan.FromSeconds(10)))
            {
                // Wire it to fire an event after the specified period
                while (await timer.WaitForNextTickAsync())
                {
                    //Console.WriteLine($"Firing at {DateTime.Now}");
                    //await Utilities.UpdateInternetStatus();
                    Utilities.UpdateInternetStatus();
                }
            }

            //if (timer4InternetCheck.IsRunning)
            //{
            //    return;
            //}

            //timer4InternetCheck.IsRepeating = true;
            //timer4InternetCheck.Start();

            ////Set up the timer for Internet
            //timer4InternetCheck.Interval = TimeSpan.FromMinutes(1);// Every Minute
            //timer4InternetCheck.Tick += async (sender, e) => await Utilities.UpdateInternetStatus();
            //_logger.LogInformation("+++ START Timer 4 Internet Connection Check\n");
        }

        //private void StopTimer4InternetCheck()
        //{
        //    timer4InternetCheck.IsRepeating = false;
        //    timer4InternetCheck.Stop();          
        //    _logger.LogInformation("--- STOP Timer 4 Internet Connection Check\n");
        //}


        public async Task GoTime()
        {
            try
            {
                Debug.WriteLine("\n\n+++ GoTime() is called\n\n");
                StartTimer4InternetCheck();
                //Stop timer - if running
                StopTimersNow4MediaDisplayAndFilesAndSettings();

                deviceSettings = await UpdateDeviceSettingsEventAsync();

                //No settings found - register device and update deviceSettings
                if (deviceSettings == null)
                {
                    //Reset all the files - if the device activated before
                    _logger.LogInformation("\nReset local media files if the device used before to clean start\n");
                    await fileDownloadService.resetMediaNamesInLocalJSonAndDeleteLocalFiles();
                    //Navigate to RegisterView
                    _logger.LogInformation("\n\n+++ No settings found - register device and update deviceSettings\n\n");
                    await Shell.Current.GoToAsync(nameof(RegisterView));

                }
                else//Registered device - start timer for image display and file/settings sync
                {
                    _logger.LogInformation("\t\t+++ SETUP Timer for image display and file/settings sync\n");
                    SetupAndStartTimers4MediaAndSettings();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"GoTime #396 Exception\n" +
                                $"Exception: {ex.Message}");
                await GoTime(); // IF EXCEPTION - TRY AGAIN
            }
        }

        private async Task<DeviceSettings> UpdateDeviceSettingsEventAsync()
        {
            //Load Device Settings
            //DeviceSettingsService deviceSettingsService = DeviceSettingsService.Instance;
            DeviceSettingsService deviceSettingsService = new DeviceSettingsService();
            deviceSettings = await deviceSettingsService.loadDeviceSettings();
            return deviceSettings;
        }


        private async Task UpdateMediaEventAsync()
        {
            //Update Device Settings
            categoryList = await fileDownloadService.synchroniseMediaFiles();
            allMedia = fileDownloadService.combineAllMediItemsFromCategory(categoryList);
        }


        private async void SetupAndStartTimers4MediaAndSettings()
        {
            await UpdateMediaEventAsync();



            //Start the timers
            StartTimersNow4MediaDisplayAndFilesAndSettings();

            //await Shell.Current.Navigation.PopToRootAsync();
            //await Application.Current.MainPage.Navigation.PushAsync(new EmptyPage(), false);
            await DisplayMediaEvent();//FIRST TIME CALL
        }

        //Page webPage = new WebViewViewer();

        private async Task DisplayMediaEvent()//(object sender, EventArgs e)
        {
            while (true)
            {
                try
                {
                    imageViewModel.ShowNoInternetIcon = !Utilities.isInternetAvailable();

                    if (allMedia == null || allMedia.Count == 0)
                    {
                        Information info = new Information();
                        info.Title = "Assign Media Categories to Your Device";
                        info.Message = $"Welcome to GuzelBoard\n" +
                            "Congratulations! If you're reading this message, it means you've successfully completed the device registration process. Well done!\n" +
                            "There is one last step that requires your attention.\n" +
                            "Assign the categories that will be displayed on your device, via our web portal.\n";

                        info.Message += Utilities.BASE_ADDRESS;

                        var navigationParameter = new Dictionary<string, object>
                        {
                            { "PickCategories", info }
                        };
                        await Shell.Current.GoToAsync(nameof(InformationView), true, navigationParameter);

                        //Show the message and navigate to main page
                        await DoDelay(15);

                        //IF THE DEVICE REMOVED FROM THE PORTAL - IT WILL BE REGISTERED AGAIN OR IF CATEGORIES ASSIGNED IT WILL START TO DISPLAY
                        await Shell.Current.GoToAsync("..");  // This calls GoTime! 

                        continue;
                        //await GoTime(); 
                        //return;
                    }
                    currentMedia = getMedia();

                    //imageViewModel.Media = currentMedia;
                    //imageViewModel.displayMedia();

                    if (currentMedia.type == "file")
                    {
                        _logger.LogInformation($"Navigating to Image View @: {currentMedia.name}");
                        Debug.WriteLine($"Navigating to Image View @: {currentMedia.name}");

                        imageViewModel.ImageMediaSource = getMediaPath(currentMedia); //  currentMedia.path = getMediaPath(currentMedia);

                        await DoDelay(1);
                        imageViewModel.WebViewVisible = false;
                        imageViewModel.ImageSourceVisible = true;
#if DEBUG
                        imageViewModel.ImageName = currentMedia.name;
                        imageViewModel.ImageTiming = currentMedia.timing;
                        imageViewModel.MediaInformation = "IMAGE";
#endif
                        await DoDelay(currentMedia.timing);
                    }
                    else//IF WEBSITE
                    {
                        //If not internet, don't try to show websites.
                        if (Utilities.isInternetAvailable())
                        {
                            _logger.LogInformation($"Navigating to Web Media #: {currentMedia.name}");
                            Debug.WriteLine($"Navigating to Web Media #: {currentMedia.name}");

                            imageViewModel.WebMediaSource = currentMedia.path;
                            imageViewModel.DisplayWidth = currentMedia.display_width;

                            //SHOW WEB VIEW
                            imageDisplay.AddWebView(currentMedia.path, currentMedia.display_width);

                            await DoDelay(3);
                            imageViewModel.ImageSourceVisible = false;
                            imageViewModel.WebViewVisible = true;
#if DEBUG
                            imageViewModel.ImageName = currentMedia.name;
                            imageViewModel.ImageTiming = currentMedia.timing;
                            imageViewModel.MediaInformation = "WEB SITE";
#endif
                            await DoDelay(currentMedia.timing);

                            //POP WEB VIEW
                            imageDisplay.PopWebView();

                            //https://learn.microsoft.com/en-us/dotnet/api/system.gc.collect?view=net-7.0
#if DEBUG && WINDOWS
                            Debug.WriteLine("Memory used before collection:       {0:N0}", GC.GetTotalMemory(false));
                            //GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                            Debug.WriteLine("Memory used after full collection:   {0:N0}", GC.GetTotalMemory(true));
#endif
                        }
                        else
                        {
                            //No Internet
                            //MediaInformation += "\tNo internet connection!";
                            //imageViewModel.ShowNoInternetIcon = true;
                            await DoDelay(1);
                        }
                    }







                    //if (currentMedia.type == "file")
                    //{
                    //    currentMedia.path = getMediaPath(currentMedia);
                    //    _logger.LogInformation($"Navigating to Image View @: {currentMedia.name}");
                    //    Debug.WriteLine($"Navigating to Image View @: {currentMedia.name}");

                    //    MiniMedia miniMedia = new MiniMedia(currentMedia);
                    //    var mediaParameter = new Dictionary<string, object>
                    //    {
                    //        { "MyMedia", miniMedia }
                    //    };

                    //    //await Shell.Current.GoToAsync("..");
                    //    //Shell.Current.CurrentPage;
                    //    //Shell.Current.


                    //    //await Shell.Current.Navigation.PopAsync();
                    //    //await Shell.Current.Navigation.PushAsync(page(ImageViewer), true);

                    //    ImageViewer.contextMedia = currentMedia;    
                    //    await Shell.Current.GoToAsync($"{nameof(ImageViewer)}", false);
                    //    //await Application.Current.MainPage.Navigation.PushAsync(new ImageViewer(), true);  
                    //    //await Shell.Current.GoToAsync(nameof(ImageViewer), true, mediaParameter);
                    //    await DoDelay(currentMedia.timing);
                    //    //await Shell.Current.GoToAsync("..");

                    //    await Shell.Current.Navigation.PopAsync(false);
                    //    //await DoDelay(1);

                    //    //await Application.Current.MainPage.Navigation.PopAsync();
                    //    //await Application.Current.MainPage.Navigation.PopToRootAsync();
                    //    //await Shell.Current.GoToAsync($"//{nameof(WelcomeView)}");
                    //}
                    //else//IF WEBSITE
                    //{
                    //    //If not internet, don't try to show websites.
                    //    if (Utilities.isInternetAvailable())
                    //    {
                    //        _logger.LogInformation($"Navigating to Web Media #: {currentMedia.name}");
                    //        Debug.WriteLine($"Navigating to Web Media #: {currentMedia.name}");

                    //        MiniMedia miniMedia = new MiniMedia(currentMedia);
                    //        var mediaParameter = new Dictionary<string, object>
                    //        {
                    //            { "MyMedia", miniMedia }
                    //        };
                    //        //webPage.contextMedia = media;
                    //        WebViewViewer.contextMedia = currentMedia;
                    //        await Shell.Current.GoToAsync($"{nameof(WebViewViewer)}", false);


                    //        //await Navigation.PushAsync(webPage, true);
                    //        //await Application.Current.MainPage.Navigation.PushAsync(new WebViewViewer(), true);
                    //        //await Shell.Current.GoToAsync(nameof(WebViewViewer), true, mediaParameter);
                    //        await DoDelay(currentMedia.timing);
                    //        //await Shell.Current.GoToAsync("..");

                    //        await Shell.Current.Navigation.PopAsync(false);
                    //        //await DoDelay(1);

                    //        //await Shell.Current.Navigation.PopAsync();
                    //        //await Application.Current.MainPage.Navigation.PopToRootAsync();

                    //        /*In user interface design, “modal” refers to something that requires user interaction before the application can continue.*/
                    //    }
                    //    else
                    //    {
                    //        //No Internet
                    //        //MediaInformation += "\tNo internet connection!";                        
                    //        await DoDelay(1);
                    //    }
                    //}

                    //https://github.com/dotnet/maui/issues/9300
                    //INavigation nav = Shell.Current.Navigation;
                    //Debug.WriteLine($"URL PATH Count: {Navigation.NavigationStack.Count}");                                
                    Debug.WriteLine($"URL PATH Count: {Shell.Current.Navigation.NavigationStack.Count}");


                    //No settings found (device removed) - register device and update deviceSettings
                    if (deviceSettings == null)
                    {
                        allMedia.Clear();
                        await GoTime(); // DEVICE REMOVED GO TO REGISTER
                        return;
                    }
                    //If internet is not available stop file syncronisation
                    if (!Utilities.isInternetAvailable() && timer4MediaAndSettings != null)
                    {
                        StopTimer4FilesAndDeviceSettings();
                    }
                    else if (Utilities.isInternetAvailable() && timer4MediaAndSettings == null)
                    {
                        StartTimer4FilesAndDeviceSettings();
                    }
                }
                //Ref https://learning.oreilly.com/library/view/c-cookbook/0596003390/ch05s09.html
                catch (System.Runtime.InteropServices.COMException ce)
                {
                    _logger.LogError($"\n\t #036 Exception Error Code: {(uint)ce.ErrorCode}\n" +
                           $"Path: {currentMedia.path}\n" +
                           $"s3key: {currentMedia.s3key}\n");
                }
                catch (System.UriFormatException exFormat)
                {
                    _logger.LogError($"\n\t #044 Exception: {exFormat.Message}\n" +
                           $"Path: {currentMedia.path}\n");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"#879 Exception: {ex.Message}");
                    _logger.LogError($"\n\t #879 Exception: {ex.Message}\n" +
                        $"Path: {currentMedia.path}\n" +
                        $"s3key: {currentMedia.s3key}\n");
                    await DoDelay(currentMedia.timing);
                }
                //await Shell.Current.Navigation.PopToRootAsync();
                //await DisplayMediaEvent(); //RECURSIVE CALL
            }
        }





        //private static Random random = new Random();
        int index = 0;
        private Media getMedia()
        {
            try
            {
                //No files to show
                if (allMedia == null || allMedia.Count == 0)
                {
                    Debug.WriteLine("No files to show");
                    _logger.LogInformation($"\n\t #433 No files to show {nameof(MediaManager)}\n\n");
                    index = 0;
                    Media noMedia = new Media();
                    return noMedia;
                    //return null;
                }
                if (index >= allMedia.Count)
                    index = 0;
                Media randomMedia = allMedia[index]; ;// allMedia[random.Next(allMedia.Count)];
                index++;
                return randomMedia;
            }
            catch (Exception ex)
            {
                _logger.LogError($"\n\t #411 Index exception ocurred {nameof(MediaManager)}\n " +
                                $"\tException {ex.Message}\n");
                Media noMedia = new Media();
                return noMedia;
            }
        }
        public async Task DoDelay(int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        public string getMediaPath(Media media)
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
    }
}
