using InfoBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoBoard.Services
{
    internal class RegisterDevice
    {
        public void registerDevice()
        {
            DeviceSettingsService settingsService = new DeviceSettingsService();
            DeviceSettings tempDeviceSettings = settingsService.loadDeviceSettings();
           
            //Not registered yet
            if (tempDeviceSettings.deviceId == "NOTSET")
            {
                ErrorInfo errorInfo = new ErrorInfo();
                //Get Device settings
                Task.Run(() => errorInfo = requestDeviceRegisterFromServer()).Wait();

                if (errorInfo.message == "unrecognized_device")
                {
                    ;//call itself after 10 minutes with a new temp id. 
                     //registerDevice();
                }

            }           
        }

        // HANDSHAKE - Register device and get settings
        // If already registered then just get settings
        private ErrorInfo requestDeviceRegisterFromServer()
        {
            RestService restService = new RestService();
            var task = restService.registerDevice();
            task.Wait();
            ErrorInfo registerResult = task.Result;           
            return registerResult;
        }

    }
}
