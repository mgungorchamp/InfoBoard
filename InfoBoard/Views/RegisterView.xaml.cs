using InfoBoard.Services;
using QRCoder;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace InfoBoard.Views;

//https://www.c-sharpcorner.com/article/net-maui-qr-code-generator/
public partial class RegisterView : ContentPage
{
	public RegisterView()
	{
		InitializeComponent();		
        updateQrCodeImageAndRegisterDevice();
    }

    private void OnQRImageButtonClicked(object sender, EventArgs e)
    {
        updateQrCodeImageAndRegisterDevice();
       
    }

    void updateQrCodeImageAndRegisterDevice() 
    {
        Constants.TEMPORARY_CODE = Constants.updateTemporaryCode();
        RegisterKey.Text = "Temporary Code:" + Constants.TEMPORARY_CODE;
        createQrCrCodeImage(Constants.TEMPORARY_CODE);
        qrImageButton.Source = Path.Combine(Constants.MEDIA_DIRECTORY_PATH, Constants.QR_IMAGE_NAME_4_TEMP_CODE);

        //Register Device
        RegisterDevice register = new RegisterDevice();
        register.registerDevice();
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

    private  void saveImageToFile(string path, string imageName, Image<Rgba32> image)
    {
        if (String.IsNullOrEmpty(path))
            return;

        image.Save(Path.Combine(path, imageName));
    }

  
}