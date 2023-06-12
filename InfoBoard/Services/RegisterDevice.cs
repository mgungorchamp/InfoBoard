 using InfoBoard.Models;

namespace InfoBoard.Services
{
    internal class RegisterDevice
    {
        public RegisterationResult attemptToRegister()
        {
            DeviceSettingsService settingsService =  DeviceSettingsService.Instance;
            DeviceSettings localDeviceSettings = settingsService.readSettingsFromLocalJSON();

            //Not registered yet
            if (localDeviceSettings == null)
            {
                RegisterationResult registrationResult;
                //Get Device settings
                //Task.Run(() => errorInfo = requestDeviceRegisterFromServer()).Wait();
                registrationResult = requestDeviceRegistrationFromServer();

                //Registeration succesful
                if (registrationResult != null && registrationResult.error == null)
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
        private RegisterationResult requestDeviceRegistrationFromServer()
        {
            RestService restService = new RestService();

            var task = Task.Run(() => restService.registerDevice());
            task.Wait();
            return task.Result;

            //Task<RegisterationResult> result = restService.registerDevice();
            //RegisterationResult registerResult = await result;
            //var registerResult = restService.registerDevice();
            //registerResult.Start();
            //registerResult.Wait();
            //fileListFromServer = task.Result;
            //return registerResult.Result;


            //return registerResult;
        }
    }
}
