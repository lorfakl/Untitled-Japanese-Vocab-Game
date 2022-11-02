using PlayFab;
using PlayFab.ClientModels;
using DeviceId;

// See https://aka.ms/new-console-template for more
// information
Console.WriteLine("Hello, World!");





public class BotTesting 
{
    public async Task<PlayFabResult<LoginResult>> RandomLogin()
    {
        string deviceID = new DeviceIdBuilder().AddMachineName().ToString();
        var request = new LoginWithCustomIDRequest { CustomId = deviceID, CreateAccount = true };
        var loginTask = PlayFabClientAPI.LoginWithCustomIDAsync(request);
        await loginTask
    }
}
