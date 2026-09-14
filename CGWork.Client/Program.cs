using Blazored.LocalStorage;
using CGWork.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Всичко, което прави фронтендът, е да комуникира с бекенд API-то през HttpClient:
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) // Или URL адреса на бекенда в Render
});

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();