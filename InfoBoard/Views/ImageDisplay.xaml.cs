using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using InfoBoard.ViewModel;
using System.ComponentModel;
using System.Diagnostics;

namespace InfoBoard.Views;

public partial class ImageDisplay : ContentPage
{
    //ImageViewModel _imageViewModel;
    //public ImageDisplay(ImageViewModel imageViewModel)
    //{
    //    InitializeComponent();

    //    NavigationPage.SetHasNavigationBar(this, false);

    //    imageViewModel.NNavigation = this.Navigation;
    //    BindingContext = imageViewModel;// new ImageViewModel(Navigation);

    //    _imageViewModel = imageViewModel;

    //    //mainPageImage.Source = ImageSource.FromFile(_imageViewModel.ImageSource);
    //    //DisplayAlert("Downloading Files", "Fetching files and storing them to local folder", "OK");
    //    //mainPageImage.Aspect = .
    //    //mediaElement.Source = FileMediaSource.FromFile("C:\\Users\\mgungor\\AppData\\Local\\Packages\\1865f6a0-a2e2-499e-b742-d4d1137346cd_9zz4h110yvjzm\\LocalState\\Media\\648e0b18dddd2-cat.jpg");
    //    //mediaElement.Source = FileMediaSource.FromUri("https://sec.ch9.ms/ch9/5d93/a1eab4bf-3288-4faf-81c4-294402a85d93/XamarinShow_mid.mp4");        
    //}

    //ImageViewModel _imageViewModel;
    public ImageDisplay()
    {
        InitializeComponent();
        //NavigationPage.SetHasNavigationBar(this, false);
        
        BindingContext = new ImageViewModel(); 
        //_imageViewModel.NavigationSet = this.Navigation;
        Debug.WriteLine("\n\n++++++++++++++ ImageDisplay Constructor\n\n");
    }
    ~ImageDisplay() 
    {
        Debug.WriteLine("\n\n------------- ImageDisplay Destructor\n\n");
    } 
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        
        //await mainPageImage.FadeTo(0, 500);
        //await mainPageImage.FadeTo(1, 500);

        var toast = Toast.Make($"Appearing! ImageDisplay");
        await toast.Show();
        
        Debug.WriteLine($"OnAppearing:\n{mainPageImage.Source} \nApp.Current.Id{App.Current.Id}\nPage ID:{this.Id}");

        //mainPageImage.Source = ImageSource.FromFile(_imageViewModel.ImageSource);
        await ((ImageViewModel)BindingContext).GoTimeNow();
    }

     
    protected async override void OnDisappearing()
    {
        base.OnDisappearing();

        ((ImageViewModel)BindingContext).StopTimerNow();

        var toast = Toast.Make("Disappearing! ImageDisplay");
        await toast.Show();

        Debug.WriteLine($"OnDisappearing:\n{mainPageImage.Source} \nApp.Current.Id{App.Current.Id}\nPage ID:{this.Id}");

    }



    //public async void showMustStart()
    //{
    //    //await _imageViewModel.GoTimeNow();

    //    //mainPageImage.PropertyChanged += async (object sender, PropertyChangedEventArgs eargs) =>
    //    //{

    //    //    if (eargs.PropertyName == "IsLoading")
    //    //    {

    //    //        if (!mainPageImage.IsLoading)
    //    //        {

    //    //            var mauiContext = this.Handler.MauiContext;/* get ContentPage.Handler.MauiContext */;

    //    //            try
    //    //            {

    //    //                var res = await mainPageImage.Source.GetPlatformImageAsync(mauiContext);
    //    //                //mainPageImage.Source = res.Value;
    //    //            }
    //    //            catch (Exception e)
    //    //            {

    //    //                Debug.WriteLine($"EXCEPTION CAUGHT {e}");

    //    //                mainPageImage.Source = "https://upload.wikimedia.org/wikipedia/commons/d/dd/Achtung.svg";
    //    //            }
    //    //        }
    //    //    }
    //    //};

    //}
}