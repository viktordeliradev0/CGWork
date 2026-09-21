using Blazored.LocalStorage;
using CGWork.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Клиентът комуникира с бекенд API-то през HttpClient
        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri("https://cgwork-api.onrender.com/")
        });

        builder.Services.AddBlazoredLocalStorage();

        await builder.Build().RunAsync();
    }
}