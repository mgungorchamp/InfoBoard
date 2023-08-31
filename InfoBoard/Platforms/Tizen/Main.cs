
/* Unmerged change from project 'InfoBoard (net8.0-ios)'
Before:
using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
After:
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using System;
*/

/* Unmerged change from project 'InfoBoard (net8.0-android)'
Before:
using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
After:
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using System;
*/
namespace InfoBoard;

class Program : MauiApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    static void Main(string[] args)
    {
        var app = new Program();
        app.Run(args);
    }
}
