using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoBoard.Platforms.Android
{
    //https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/webview?pivots=devices-android#reload-content
    internal class MyWebChromeClient : MauiWebChromeClient
    {
        public MyWebChromeClient(IWebViewHandler handler) : base(handler)
        {

        }


   

    }
}
