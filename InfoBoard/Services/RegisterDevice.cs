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

        public async Task<string> registerDevice()
        {
            DeviceSettingsService settingsService = new DeviceSettingsService();
            DeviceSettings tempDeviceSettings = settingsService.loadDeviceSettings();
           
            //Not registered yet
            if (tempDeviceSettings.deviceId == "NOTSET")
            {
                ErrorInfo errorInfo = new ErrorInfo();
                //Get Device settings
                //Task.Run(() => errorInfo = requestDeviceRegisterFromServer()).Wait();
                errorInfo = await requestDeviceRegisterFromServer();

                if (errorInfo.message == "unrecognized_device")
                {
                    return "Unrecognized Device";//call itself after 10 minutes with a new temp id. 
                                                 //registerDevice();
                }
                else 
                {
                    //This should return SUCCESS or something like that 
                    tempDeviceSettings = settingsService.loadDeviceSettings();
                    return $"Device ID is: {tempDeviceSettings.deviceId}";
                }

            }    
            return $"This device has already been registered with Device ID is: {tempDeviceSettings.deviceId}";
        }

        // HANDSHAKE - Register device and get settings
        // If already registered then just return device id
        private async Task<ErrorInfo> requestDeviceRegisterFromServer()
        {
            RestService restService = new RestService();
            ErrorInfo registerResult = (await restService.registerDevice());
          
            //ErrorInfo registerResult = task.Result;
            //ToDo: 
            //Save the device id into settings via DeviceSettings class 
            if (registerResult.message != "unrecognized_device")
            { 
                DeviceSettingsService settingsService = new DeviceSettingsService();
                DeviceSettings deviceSettings = new DeviceSettings();
                deviceSettings.deviceId = registerResult.message; // This should be the device ID 
                settingsService.saveSettingsToLocalAsJSON(deviceSettings);
            }
            return registerResult;
        }

    }
}
