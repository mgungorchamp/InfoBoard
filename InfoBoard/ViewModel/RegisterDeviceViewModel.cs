using InfoBoard.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using QRCoder;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using InfoBoard.Models;

namespace InfoBoard.ViewModel
{
    public sealed class RegisterDeviceViewModel : INotifyPropertyChanged
    {
        private static readonly RegisterDeviceViewModel instance = new();
        static RegisterDeviceViewModel()
        {
        }
       
        public static RegisterDeviceViewModel Instance {
            get {
                return instance;
            }
        }   
        public event PropertyChangedEventHandler PropertyChanged;
        //System.Timers.Timer aRegistrationTimer = new System.Timers.Timer();
        IDispatcherTimer timer4Registration;

        private string _registerKeyLabel;
        private string _qrImageButton;
        private string _status;
        private int counter;

        public Command OnRegenerateQrCodeCommand { get; set; }
        public Command OnOpenRegisterDeviveWebPageCommand { get; set; }
        private RegisterDeviceViewModel() 
        {
            counter = 1;
            //Initial Code Generation t
            generateQrCode();

            OnRegenerateQrCodeCommand = new Command(
               execute: () =>
               {
                   generateQrCode();
                   //startRegistration();
               });

            OnOpenRegisterDeviveWebPageCommand = new Command(
                 execute: async () =>
                 {
                    // Navigate to the specified URL in the system browser.
                     await Launcher.Default.OpenAsync($"https://guzelboard.com/index.php?action=devices&temporary_code={Constants.TEMPORARY_CODE}");

                 });

            timer4Registration = Application.Current.Dispatcher.CreateTimer();
            //Set timer to call to register with new code
            //startTimedRegisterationEvent();
        }

        public void startRegistration()
        {
            counter++;
            //Task.Run(() => registerDeviceViaServer()).Wait();
            registerDeviceViaServer();
            //await Navigation.PushAsync(new RegisterView());
            //TODO 
            //Change the view to RegisterView 

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
            }
        }
        private ImageViewModel imageViewModel;

        public void startTimedRegisterationEvent(ImageViewModel imageViewModel)
        {
            generateQrCode();
            this.imageViewModel = imageViewModel;
            timer4Registration.Interval = TimeSpan.FromSeconds(10);
            timer4Registration.Tick += (sender, e) => registerDeviceViaServer();
            timer4Registration.Start();

            _status = "Registering device...";
            OnPropertyChanged();
        }      

        private async void registerDeviceViaServer()
        {
            counter++;
            //aRegistrationTimer.Interval = counter * 10 * 1000;

            if (!UtilityServices.isInternetAvailable())
            {             
                _status = "No Internet Connection";
                OnPropertyChanged(nameof(Status));
                return;
            }

            _status = $"Registering Device: Attempt {counter}"; 
            OnPropertyChanged(nameof(Status));

            //Register Device
            RegisterDevice register = new RegisterDevice();            
            RegisterationResult registrationResult = await register.attemptToRegister();
            
            //Success - no error
            if (registrationResult != null && registrationResult.error == null)
            {           
                timer4Registration.Stop();                

                DeviceSettingsService service = DeviceSettingsService.Instance;
                DeviceSettings deviceSettings = service.readSettingsFromLocalJSON();
                _status = $"Device registered succesfully. \nDevice ID: {deviceSettings.device_key}";
                _status += "\nUpdating Media Files... going back to image page!";

                Constants.updateMediaFilesUrl(deviceSettings.device_key);

                //*aRegistrationTimer.Stop(); // Timer needs to be stopped after successful registration
                //*aRegistrationTimer.Dispose();
                //imageViewModel.Navigation.PopAsync();
               
                OnPropertyChanged(nameof(Status));
                await Task.Delay(TimeSpan.FromSeconds(5));        
               

                //change to ImageDisplayView               
                imageViewModel.NavigateToMainViewAndStartTimer4ImageDisplayAnd4FileSync();
                counter = 1;
                _status = "Welcome!";
            }
            else if (registrationResult != null && registrationResult.error != null)
            {                
                _status = $"Registration Failed. \nError: {registrationResult?.error.message} \nAttempt {counter}";  
                OnPropertyChanged(nameof(Status));
            }          
     
        }

        private void generateQrCode()
        {
            //Reset the temporary code and handshake URL
            Constants.resetTemporaryCodeAndHandshakeURL();

            _registerKeyLabel = "Temporary Code:" + Constants.TEMPORARY_CODE;

            //Give full path to API with QR Code 
            //string qrCodeContent = Constants.HANDSHAKE_URL + Constants.TEMPORARY_CODE;
            createQrCrCodeImage(Constants.HANDSHAKE_URL);
            _qrImageButton = Path.Combine(Constants.MEDIA_DIRECTORY_PATH, Constants.QR_IMAGE_NAME_4_TEMP_CODE);

            _status = "New QR Code Generated";
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
            saveImageToFile(Constants.MEDIA_DIRECTORY_PATH, Constants.QR_IMAGE_NAME_4_TEMP_CODE, image);

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
