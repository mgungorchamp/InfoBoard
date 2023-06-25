using CommunityToolkit.Maui.Alerts;
using InfoBoard.Views;
using Microsoft.Maui;
using System.Diagnostics;

namespace InfoBoard;

public partial class App : Application
{
    //ImageDisplay imageDisplayView;
    //public App(ImageDisplay imageDisplayView)
    //{
    //   InitializeComponent();
    //   this.imageDisplayView = imageDisplayView;

    ////    //MainPage = new AppShell(); // SEEMS not NEEDED
    ////    // Following the article:  https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pages/navigationpage#perform-modeless-navigation

    //   Debug.WriteLine($" DIP *************** ------ > App  CONSTRUCTOR! \n{App.Current.Id}");
    //   MainPage = new NavigationPage(root: imageDisplayView);

    ////    //imageDisplayView.showMustStart();
    //}

    public App()
    {
        InitializeComponent();

        MainPage = new AppShell(); // SEEMS not NEDED

        // Following the article:  https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pages/navigationpage#perform-modeless-navigation
        Debug.WriteLine($" +++++++++++++++++ > App  CONSTRUCTOR! \n{App.Current.Id}");         
        //MainPage = new NavigationPage(root: new ImageDisplay());
    }

    ~App() 
    {
        Debug.WriteLine($"  ---------------- > App  DESTRUCTOR!//////// \n{App.Current.Id}");
    }
    protected override void OnStart()
    {
        //var task = InitAsync();

       // task.ContinueWith((task) =>
        //{
         //   MainThread.BeginInvokeOnMainThread(() =>
          //  {
                Debug.WriteLine($" +++++++++++++++++ > App  OnStart!");
               // MainPage = new AppShell();

                // Choose navigation depending on init
                //Shell.Current.GoToAsync("imagedisplay");
           // });
       // });

        base.OnStart();
    }

    //private async Task InitAsync()
    //{
    //    //await m_configProvider.InitAsync();
    //}


    //protected override Window CreateWindow(IActivationState activationState)
    //{
    //    Window window = this.Windows.FirstOrDefault();
    //    // Window window = Application.Current.Windows.FirstOrDefault();   
    //    if (window != null)
    //        return window;
        
    //    window = base.CreateWindow(activationState);

    //    window.Created += (s, e) =>
    //    {
    //        // Custom logic
    //        var toast = Toast.Make($"Created!");
    //        toast.Show();

            
    //        Debug.WriteLine($"\n\t**window.Page.Id: {window.Page.Id}");
    //        Debug.WriteLine($"\n\t**MainPage.Id: {MainPage.Id}");
    //        Debug.WriteLine($"Creating....");
    //        //MainPage = new NavigationPage(root: new ImageDisplay());
    //        Debug.WriteLine($"Created:\n\twindow.Id: {window.Id} \n\t App.Current.Id:{App.Current.Id} \n\tMainPage.Id: {MainPage.Id}");
            
    //        //MainPage = new NavigationPage(root: imageDisplayView); // when it was singleton
    //    };
        
    //    window.Resumed += (s, e) =>
    //    {
    //        // Custom logic
    //        //var toast = Toast.Make($"OnResumed!");
    //        //toast.Show();            
    //        Debug.WriteLine($"OnResumed:\n\twindow.Id: {window.Id} \n\t App.Current.Id:{App.Current.Id} \n\tMainPage.Id: {MainPage.Id}");
    //    };
        
    //    window.Activated += (s, e) =>
    //    {
    //        // Custom logic
    //        //var toast = Toast.Make($"Activated!");
    //        //toast.Show();

    //        Debug.WriteLine($"Activated:\n\twindow.Id: {window.Id} \n\t App.Current.Id:{App.Current.Id} \n\tMainPage.Id: {MainPage.Id}");

    //        //MainPage = new NavigationPage(root: imageDisplayView);
    //        //imageDisplayView.showMustStart();

    //    };
        
    //    window.Destroying += (s, e) =>
    //    {
    //        // Custom logic
    //        var toast = Toast.Make($"Destroying!");
    //        toast.Show();
            

    //        Debug.WriteLine($"Destroying:\n\twindow.Id: {window.Id} \n\t App.Current.Id:{App.Current.Id} \n\tMainPage.Id: {MainPage.Id}");
    //    };

        
      
    //    return window;
    //}


}
