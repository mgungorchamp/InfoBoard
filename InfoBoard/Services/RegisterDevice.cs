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
        public async Task<RegisterationResult> startRegistration()
        {
            DeviceSettingsService settingsService = new DeviceSettingsService();
            DeviceSettings localDeviceSettings = settingsService.readSettingsFromLocalJSON();

            //Not registered yet
            if (localDeviceSettings == null)
            {
                RegisterationResult registrationResult;
                //Get Device settings
                //Task.Run(() => errorInfo = requestDeviceRegisterFromServer()).Wait();
                registrationResult = await requestDeviceRegistrationFromServer();

                //Registeration succesful
                if (registrationResult.error == null)
                {
                    //Registeration succesful and request settings and save it to local
                    localDeviceSettings = settingsService.retrieveDeviceSettingsFromServer(registrationResult.device_key);
                    settingsService.saveSettingsToLocalAsJSON(localDeviceSettings);                 
                }    
                return registrationResult;
            }
            return null;
            //return $"This device has already been registered with Device ID is: {localDeviceSettings.device_key}";
        }

        // HANDSHAKE - Register device and get settings
        // If already registered then just return device id
        private async Task<RegisterationResult> requestDeviceRegistrationFromServer()
        {
            RestService restService = new RestService();
            RegisterationResult registerResult = (await restService.registerDevice());            
            return registerResult;
        }
    }
}
