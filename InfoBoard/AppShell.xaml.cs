using InfoBoard.Services;

namespace InfoBoard;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(Views.NotePage), typeof(Views.NotePage));
        SaveFilesToLocalDirectory saveFilesToLocalDirectory = new SaveFilesToLocalDirectory();
        saveFilesToLocalDirectory.fetchAndSave();
    }
}
