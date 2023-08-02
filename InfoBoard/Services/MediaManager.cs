using InfoBoard.Models;
using InfoBoard.Services;
using InfoBoard.Views;
using InfoBoard.Views.MediaViews;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace InfoBoard.ViewModel
{
    public class MediaManager
    {
        INavigation Navigation;
        private readonly ILogger _logger;
        private IDispatcherTimer timer4MediaDisplaying;
        private IDispatcherTimer timer4FileSync;
        private IDispatcherTimer timer4DeviceSettingsSync;

        private DeviceSettings deviceSettings;
        private FileDownloadService fileDownloadService;

        private List<MediaCategory> categoryList;
        private List<Media> allMedia;
        private Media currentMedia;


        private static readonly MediaManager instance = new MediaManager();        

        public static MediaManager Instance {
            get {
                return instance;
            }
        }

        private MediaManager()
        {
            _logger = Utilities.Logger(nameof(MediaManager));
            fileDownloadService = new FileDownloadService();

            timer4MediaDisplaying = Application.Current?.Dispatcher.CreateTimer();
            timer4FileSync = Application.Current?.Dispatcher.CreateTimer();
            timer4DeviceSettingsSync = Application.Current?.Dispatcher.CreateTimer();
        }

        public void SetNavigation(INavigation Navigation)
        {
            this.Navigation = Navigation;
        }



        private void StartTimersNow4MediaDisplayAndFilesAndSettings()
        {
            _logger.LogInformation("\t\t+++ START StartTimersNow4MediaDisplayAndFilesAndSettings() is called");

            timer4MediaDisplaying.IsRepeating = true;
            timer4MediaDisplaying.Start();

            StartTimer4FilesAndDeviceSettings();
        } 
        private void StopTimersNow4MediaDisplayAndFilesAndSettings()
        {
            _logger.LogInformation("\t\t--- STOP StopTimersNow4MediaDisplayAndFilesAndSettings() is called");
            
            timer4MediaDisplaying.IsRepeating = false;
            timer4MediaDisplaying.Stop();

            StopTimer4FilesAndDeviceSettings();
        }

        private void StopTimer4FilesAndDeviceSettings() 
        {
            timer4FileSync.IsRepeating = false;
            timer4FileSync.Stop();

            timer4DeviceSettingsSync.IsRepeating = false;
            timer4DeviceSettingsSync.Stop();
            _logger.LogInformation("\t\t--- STOP Timer 4 Files And DeviceSettings is called\n");
        }

        private void StartTimer4FilesAndDeviceSettings()
        {
            timer4FileSync.IsRepeating = true;
            timer4FileSync.Start();

            timer4DeviceSettingsSync.IsRepeating = true;
            timer4DeviceSettingsSync.Start();
            _logger.LogInformation("\t\t+++ START Timer 4 Files And DeviceSettings is called\n");
        }        
        public async Task GoTime() 
        {
            try
            {
                Debug.WriteLine("\n\n+++ GoTime() is called\n\n");
                //Stop timer - if running
                StopTimersNow4MediaDisplayAndFilesAndSettings();

                deviceSettings = await UpdateDeviceSettingsEventAsync();

                //No settings found - register device and update deviceSettings
                if (deviceSettings == null)
                {
                    //Reset all the files - if the device activated before
                    _logger.LogInformation("\nReset local currentMedia files if the device used before to clean start\n");
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
            DeviceSettingsService deviceSettingsService = DeviceSettingsService.Instance;
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

            //Set up the timer for Syncronise Media Files             
            timer4FileSync.Interval = TimeSpan.FromSeconds(101);
            timer4FileSync.Tick += async (sender, e) => await UpdateMediaEventAsync();
            
            //Get latest settings from server - every 15 seconds
            timer4DeviceSettingsSync.Interval = TimeSpan.FromSeconds(107);
            timer4DeviceSettingsSync.Tick += async (sender, e) => await UpdateDeviceSettingsEventAsync();
            
            //Start the timers
            StartTimersNow4MediaDisplayAndFilesAndSettings();

            //await Shell.Current.Navigation.PopToRootAsync();
            await Navigation.PushModalAsync(new EmptyPage(), false);
            await DisplayMediaEvent();//FIRST TIME CALL
        }

        static Page webPage = new WebViewViewer();

        private async Task DisplayMediaEvent()//(object sender, EventArgs e)
        {
            try
            {
                if (allMedia == null || allMedia.Count == 0)
                {
                    Information info = new Information();
                    info.Title = "Assign Media Categories to Your Device";
                    info.Message = "Welcome to GuzelBoard\n" +
                        "Congratulations! If you're reading this message, it means you've successfully completed the device registration process. Well done!\n" +
                        "There is one last step that requires your attention.\n" +
                        "Assign the categories that will be displayed on your device, via our web portal.\nhttps://guzelboard.com";
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "PickCategories", info }
                    };
                    await Shell.Current.GoToAsync(nameof(InformationView), true, navigationParameter);

                    //Wait 10 seconds and call the function again                    
                    await DoDelay(15);
                    await GoTime(); //IF THE DEVICE REMOVED FROM THE PORTAL - IT WILL BE REGISTERED AGAIN
                    return;
                }

                currentMedia = getMedia();

                

                if (currentMedia.type == "file")
                {
                    currentMedia.path = getMediaPath(currentMedia);
                    _logger.LogInformation($"Navigating to Image View @: {currentMedia.name}");
                    Debug.WriteLine($"Navigating to Image View @: {currentMedia.name}");

                    MiniMedia miniMedia = new MiniMedia(currentMedia);
                    var mediaParameter = new Dictionary<string, object>
                    {
                        { "MyMedia", miniMedia }
                    };

                    //await Shell.Current.GoToAsync("..");
                    //Shell.Current.CurrentPage;
                    //Shell.Current.


                    //await Shell.Current.Navigation.PopAsync();
                    //await Shell.Current.Navigation.PushAsync(page(ImageViewer), true);

                    ImageViewer.contextMedia = currentMedia;    
                    //await Shell.Current.GoToAsync($"{nameof(ImageViewer)}", true);
                    await Navigation.PushModalAsync(new ImageViewer(), true);  
                    //await Shell.Current.GoToAsync(nameof(ImageViewer), true, mediaParameter);
                    await DoDelay(currentMedia.timing);
                    //await Shell.Current.GoToAsync("..");
                    await Navigation.PopModalAsync();
                }
                else//IF WEBSITE
                {
                    //If not internet, don't try to show websites.
                    if (Utilities.isInternetAvailable())
                    {
                        _logger.LogInformation($"Navigating to Web Media #: {currentMedia.name}");
                        Debug.WriteLine($"Navigating to Web Media #: {currentMedia.name}");

                        MiniMedia miniMedia = new MiniMedia(currentMedia);
                        var mediaParameter = new Dictionary<string, object>
                        {
                            { "MyMedia", miniMedia }
                        };
                        WebViewViewer.contextMedia = currentMedia;
                        //await Shell.Current.GoToAsync($"{nameof(WebViewViewer)}", true);
                        await Navigation.PushModalAsync(webPage, true);
                        //await Shell.Current.GoToAsync(nameof(WebViewViewer), true, mediaParameter);
                        await DoDelay(currentMedia.timing);
                        //await Shell.Current.GoToAsync("..");
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        //No Internet
                        //MediaInformation += "\tNo internet connection!";                        
                        await DoDelay(1);
                    }
                }

                //INavigation nav = Shell.Current.Navigation;
                Debug.WriteLine($"URL PATH Count: {Navigation.NavigationStack.Count}");

                //No settings found (device removed) - register device and update deviceSettings
                if (deviceSettings == null)
                {
                    allMedia.Clear();
                    await GoTime(); // DEVICE REMOVED GO TO REGISTER
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
            await DisplayMediaEvent(); //RECURSIVE CALL
        }
  

        private static Random random = new Random();
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
        private async Task DoDelay(int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
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
    }
}
 