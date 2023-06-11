using InfoBoard.Views;

namespace InfoBoard;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        //MainPage = new AppShell(); // SEEMS not NEDED

        // Followin the article:  https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pages/navigationpage#perform-modeless-navigation
        MainPage = new NavigationPage(root: new ImageDisplay());        
    }
}
