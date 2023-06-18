using CommunityToolkit.Maui.Views;
using InfoBoard.ViewModel;
using System.Diagnostics;

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
        //mediaElement.Source = FileMediaSource.FromFile("C:\\Users\\mgungor\\AppData\\Local\\Packages\\1865f6a0-a2e2-499e-b742-d4d1137346cd_9zz4h110yvjzm\\LocalState\\Media\\648e0b18dddd2-cat.jpg");
        //mediaElement.Source = FileMediaSource.FromUri("https://sec.ch9.ms/ch9/5d93/a1eab4bf-3288-4faf-81c4-294402a85d93/XamarinShow_mid.mp4");
        
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Debug.WriteLine($"OnAppearing: {Title}");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        Debug.WriteLine($"OnDisappearing: {Title}");
    }
}