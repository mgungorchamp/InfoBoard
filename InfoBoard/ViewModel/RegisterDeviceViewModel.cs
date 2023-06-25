using InfoBoard.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using QRCoder;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using InfoBoard.Models;
using CommunityToolkit.Maui.Alerts;

namespace InfoBoard.ViewModel
{
    public sealed class RegisterDeviceViewModel : INotifyPropertyChanged
    {          
        public event PropertyChangedEventHandler PropertyChanged;
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
                     await Launcher.Default.OpenAsync($"https://guzelboard.com/index.php?action=devices&temporary_code={Constants.TEMPORARY_CODE}");

                 });

            timer4Registration = Application.Current.Dispatcher.CreateTimer();
            //Set timer to call to register with new code
            //startTimedRegisterationEvent();
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

            _status = "Registering device...";
            OnPropertyChanged();
        }

        public void StopTimed4DeviceRegisterationEvent()
        {
         
            timer4Registration.IsRepeating = false;
            timer4Registration.Stop();
            _status = "Timer stopped...";
            OnPropertyChanged();
        }

        private async Task StartTimer4DeviceRegistration()
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
            DeviceSettingsService deviceSettingsService = DeviceSettingsService.Instance;
            RegisterationResult registrationResult = await deviceSettingsService.RegisterDeviceViaServer();
            
            //We got response from server
            if (registrationResult != null)
            {
                //Registeration succesful - no error
                if (registrationResult.error == null)
                {
                   // timer4Registration.IsRepeating = false;
                    //timer4Registration.Stop();

                    DeviceSettings deviceSettings  = await deviceSettingsService.loadDeviceSettings();
                    _status = $"Device registered succesfully. \nDevice ID: {deviceSettings.device_key}";
                    _status += "\nUpdating Media Files... going back to front page!";

                    //Ref: https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/alerts/toast?tabs=android
                    var toast = Toast.Make("Updating Media Files... going back to front page!");
                    await toast.Show();

                    OnPropertyChanged(nameof(Status));

                    //await Task.Delay(TimeSpan.FromSeconds(7));
                    //change to ImageDisplayView               
                    //await imageViewModel.GoTimeNow();
                    await Shell.Current.GoToAsync("imagedisplay");
                    counter = 1;
                    _status = "Welcome!";

                }
                else // Registration failed - error returned
                {
                    _status = $"Attempting... {counter} \nServer says: {registrationResult.error.message}";
                    OnPropertyChanged(nameof(Status));
                }               
            }
            else
            {                
                _status = $"Something strange ocurrred... Please restart your device{counter}";  
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
