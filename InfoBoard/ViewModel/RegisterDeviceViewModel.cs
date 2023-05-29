

using InfoBoard.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using QRCoder;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using Microsoft.Win32;
using System.Windows.Input;
using System.Diagnostics.Metrics;

namespace InfoBoard.ViewModel
{
    internal class RegisterDeviceViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        System.Timers.Timer aTimer = new System.Timers.Timer();

        private string _registerKeyLabel;
        private string _qrImageButton;
        private string _status;
        private int counter;

        public Command OnQRImageButtonClickedCommand { get; set; }
        public RegisterDeviceViewModel() 
        {
            counter = 1;          
            //Initial Code Generation
            generateQrCode();
            //First Register Attempt
            Task.Run(() => registerDevice()).Wait();
            //Set timer to call to register with new code
            startTimedRegisterationEvent();


            OnQRImageButtonClickedCommand = new Command(
                execute: () =>
                {
                    generateQrCode();                    
                });
            
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

        private void startTimedRegisterationEvent()
        {            
           
            //aTimer.Elapsed += updateQrCodeImageAndRegisterDevice;
            aTimer.Elapsed += async (sender, e) => await registerDevice();
         
            aTimer.Start();
            aTimer.Interval = 5 * 1000;      // This should be like 15 seconds or more      
          
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            _status = "Timed Registration Event Created";
            OnPropertyChanged();

        }
        private void generateQrCode() 
        { 
            Constants.TEMPORARY_CODE = Constants.updateTemporaryCode();
            _registerKeyLabel = "Temporary Code:" + Constants.TEMPORARY_CODE;
            createQrCrCodeImage(Constants.TEMPORARY_CODE);
            _qrImageButton = Path.Combine(Constants.MEDIA_DIRECTORY_PATH, Constants.QR_IMAGE_NAME_4_TEMP_CODE);

            _status = "New QR Code Generated";
            OnPropertyChanged(nameof(RegisterationKey));
            OnPropertyChanged(nameof(QRImageButton));            
            OnPropertyChanged(nameof(Status));
        }

        private async Task<string> registerDevice()
        {
            _status = "registerDevice";
           
            //Register Device
            RegisterDevice register = new RegisterDevice();
            //_status = register.registerDevice();
            string result  = await (register.registerDevice());
            //_status = status;
            _status = $"{result} Attempt {counter++}";

            // TODO if succesfully registered  => check result text or make it boolean
            // aTimer event should stop
            // aTimer.Stop();

            OnPropertyChanged(nameof(Status));
            return _status;
        }


        private void createQrCrCodeImage(string content)
        {
            var image = generateImage(content, (qr) => qr.GetGraphic(10) as Image<Rgba32>);
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
    }
}
