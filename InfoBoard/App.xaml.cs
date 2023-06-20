using CommunityToolkit.Maui.Alerts;
using InfoBoard.Views;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace InfoBoard;

public partial class App : Application
{
    ImageDisplay imageDisplayView;
    public App(ImageDisplay imageDisplayView)
    {
        InitializeComponent();
        this.imageDisplayView = imageDisplayView;

        //MainPage = new AppShell(); // SEEMS not NEDED

        // Following the article:  https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pages/navigationpage#perform-modeless-navigation
 
        Debug.WriteLine($" *************** ------ > App  CONSTRUCTOR! \n{App.Current.Id}");

        MainPage = new NavigationPage(root: imageDisplayView);
        
        //imageDisplayView.showMustStart();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.Destroying += (s, e) =>
        {
            // Custom logic
            var toast = Toast.Make($"Destroying!");
            toast.Show();
            

            Debug.WriteLine($"Destroying:\n{window.Id} \n{App.Current.Id}");
        };

        window.Resumed += (s, e) =>
        {            
            // Custom logic
            var toast = Toast.Make($"OnResumed!");
            toast.Show();
            
            Debug.WriteLine($"OnResumed:\n{window.Id} \n{App.Current.Id}");
        };
        window.Created += (s, e) =>
        {
            // Custom logic
            var toast = Toast.Make($"Created!");
            toast.Show();

            Debug.WriteLine($"Created:\n{window.Id} \n{App.Current.Id}");            

        };
        
        window.Activated += (s, e) =>
        {
            // Custom logic
            var toast = Toast.Make($"Activated!");
            toast.Show();

            Debug.WriteLine($"Activated:\n{window.Id} \n{this.Id}");
           
            //MainPage = new NavigationPage(root: imageDisplayView);

            //imageDisplayView.showMustStart();

        };
        return window;
    }


}
