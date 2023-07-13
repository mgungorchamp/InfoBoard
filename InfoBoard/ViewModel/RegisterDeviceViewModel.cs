using InfoBoard.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using QRCoder;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using InfoBoard.Models;
using InfoBoard.Views;
using Microsoft.Extensions.Logging;

namespace InfoBoard.ViewModel
{
    public sealed class RegisterDeviceViewModel : INotifyPropertyChanged
    {          
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ILogger _logger;
        //System.Timers.Timer aRegistrationTimer = new System.Timers.Timer();
        IDispatcherTimer timer4Registration;

        private string _registerKeyLabel;
        private string _qrImageButton;
        private string _status;
        private int counter;

        public Command OnRegenerateQrCodeCommand { get; set; }
        public Command OnOpenRegisterDeviveWebPageCommand { get; set; }
        public RegisterDeviceViewModel() 
        {
            _logger = Utilities.Logger(nameof(RegisterDeviceViewModel));
            counter = 0;
            //Initial Code Generation t
            generateQrCode();

            OnRegenerateQrCodeCommand = new Command(
               execute: () =>
               {
                   generateQrCode();                   
               });

            OnOpenRegisterDeviveWebPageCommand = new Command(
                 execute: async () =>
                 {
                    // Navigate to the specified URL in the system browser.
                     await Launcher.Default.OpenAsync($"https://guzelboard.com/index.php?action=devices&temporary_code={Utilities.TEMPORARY_CODE}");

                 });

            timer4Registration = Application.Current.Dispatcher.CreateTimer();
            //Set timer to call to register with new code
            //startTimedRegisterationEvent();
            _logger.LogInformation($"**RegisterDeviceViewModel** Constructor");
        }
             

        public void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string RegisterationKey {
            get => _registerKeyLabel;
            set {
                if (_registerKeyLabel == value)
                    return;
                _registerKeyLabel = value;
                OnPropertyChanged();
            }
        }
      
        public string QRImageButton {
            get => _qrImageButton;
            set {
                if (_qrImageButton == value)
                    return;
                _qrImageButton = value;
                OnPropertyChanged();
            }
        }
        public string Status {
            get => _status ;
            set {
                if (_status == value)
                    return;
                _status = value;
                OnPropertyChanged();
                Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
        //private ImageViewModel imageViewModel;

        public async void StartTimed4DeviceRegisterationEvent()
        {
            generateQrCode();
          //  this.imageViewModel = imageViewModel;
            await StartTimer4DeviceRegistration();

            timer4Registration.Interval = TimeSpan.FromSeconds(20);
            timer4Registration.Tick += async (sender, e) => await StartTimer4DeviceRegistration();
            timer4Registration.IsRepeating = true;
            timer4Registration.Start();

            _status = "Activating device...";
            _logger.LogInformation($"**RegisterDeviceViewModel** StartTimed4DeviceRegisterationEvent");
            OnPropertyChanged();
        }

        public void StopTimed4DeviceRegisterationEvent()
        {
         
            timer4Registration.IsRepeating = false;
            timer4Registration.Stop();
            _status = "Timer stopped...";
            _logger.LogInformation($"**RegisterDeviceViewModel** StopTimed4DeviceRegisterationEvent");
            OnPropertyChanged();
        }

        private async Task StartTimer4DeviceRegistration()
        {
            counter++;
            
            if (!Utilities.isInternetAvailable())
            {             
                _status = "No Internet Connection";
                OnPropertyChanged(nameof(Status));
                return;
            }

            _status =   $"Activating" +
                        $"\nAttempt #{counter}"; 
            OnPropertyChanged(nameof(Status));

            //Register Device
            RestService restService = new RestService();
            string registrationMessage= await restService.registerDevice();


            DeviceSettingsService deviceSettingsService = DeviceSettingsService.Instance;            
            DeviceSettings deviceSettings = await deviceSettingsService.loadDeviceSettings();

            if(deviceSettings == null)
            {
                _status = $"Attempt #{counter}" +
                          $"\n{registrationMessage}"+                    
                          $"\n\nI'm not about to give up. " +
                          $"\nI'll keep pushing forward, " +
                          $"\nno matter how many trials it takes " +
                          $"\nLet's go!";
                OnPropertyChanged(nameof(Status));
                return;
            }            

            _status =   $"\n{registrationMessage}" +                         
                        $"\n\nDevice ID: {deviceSettings.device_key}" +
                        $"\nUpdating MediaInfo Files... " +
                        $"\nand going to front page!";
            OnPropertyChanged(nameof(Status));
            _logger.LogInformation($"Registration Succesful**RegisterDeviceViewModel**: {registrationMessage}");
            await Task.Delay(TimeSpan.FromSeconds(3));
            //Ref: https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/alerts/toast?tabs=android
            //var toast = Toast.Make("Updating MediaInfo Files... going back to front page!");
            //await toast.Show();


            //await Task.Delay(TimeSpan.FromSeconds(7));
            //change to ImageDisplayView               
            //await imageViewModel.GoTimeNow();
            await Shell.Current.GoToAsync(nameof(ImageDisplay));
            counter = 1;
            _status = "Welcome!";

            //    }
            //    else // Registration failed - error returned
            //    {
            //        _status = $"Attempting... {counter} \nServer says: {registrationResult.error.message}";
            //        OnPropertyChanged(nameof(Status));
            //    }               
            //}
            //else
            //{                
            //    _status = $"Something strange ocurrred... Please restart your device{counter}";  
            //    OnPropertyChanged(nameof(Status));
            //}         
        }

        private void generateQrCode()
        {
            //Reset the temporary code and handshake URL
            Utilities.resetTemporaryCodeAndHandshakeURL();

            RegisterationKey = Utilities.TEMPORARY_CODE;

            //Give full path to API with QR Code 
            //string qrCodeContent = Constants.HANDSHAKE_URL + Constants.TEMPORARY_CODE;
            //createQrCrCodeImage(Utilities.HANDSHAKE_URL);
            createQrCrCodeImage($"https://guzelboard.com/index.php?action=devices&temporary_code={Utilities.TEMPORARY_CODE}");
            //_qrImageButton = Path.Combine(Utilities.MEDIA_DIRECTORY_PATH, Utilities.QR_IMAGE_NAME_4_TEMP_CODE);
            _qrImageButton = Path.Combine(FileSystem.CacheDirectory, Utilities.QR_IMAGE_NAME_4_TEMP_CODE);


            _status = "New activation code generated";
            OnPropertyChanged(nameof(RegisterationKey));
            OnPropertyChanged(nameof(QRImageButton));
            OnPropertyChanged(nameof(Status));

            // Navigate to the specified URL in the system browser.
            // await Launcher.Default.OpenAsync(Constants.HANDSHAKE_URL);            
        }


        //Ref https://github.com/JPlenert/QRCoder-ImageSharp
        private void createQrCrCodeImage(string content)
        {
            var image = generateImage(content, (qr) => qr.GetGraphic(11) as Image<Rgba32>);
            //saveImageToFile(Utilities.MEDIA_DIRECTORY_PATH, Utilities.QR_IMAGE_NAME_4_TEMP_CODE, image);
            saveImageToFile(FileSystem.CacheDirectory, Utilities.QR_IMAGE_NAME_4_TEMP_CODE, image);

        }
        private Image<Rgba32> generateImage(string content, Func<QRCode, Image<Rgba32>> getGraphic)
        {
            QRCodeGenerator gen = new QRCodeGenerator();
            QRCodeData data = gen.CreateQrCode(content, QRCodeGenerator.ECCLevel.H);
            return getGraphic(new QRCode(data));
        }

        private void saveImageToFile(string path, string imageName, Image<Rgba32> image)
        {
            if (String.IsNullOrEmpty(path))
                return;

            image.Save(Path.Combine(path, imageName));
        }

        // SVG you may try 
        //https://github.com/JPlenert/QRCoder-ImageSharp/blob/master/QRCoderTests/Helpers/HelperFunctions.cs
        // public static void TestImageToFile(string path, string testName, string svg)
        //public static string GenerateSvg(string content, Func<SvgQRCode, string> getGraphic)
    }
}
