using InfoBoard.ViewModel;

namespace InfoBoard.Views;

public partial class ImageDisplay : ContentPage
{
    public ImageDisplay()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        BindingContext = new ImageViewModel(Navigation);      
        //DisplayAlert("Downloading Files", "Fetching files and storing them to local folder", "OK");
        //mainPageImage.Aspect = .
    }
}