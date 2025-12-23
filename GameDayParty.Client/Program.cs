using GameDayParty.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<EventService>();
// Add the Mock Data Service here:
builder.Services.AddSingleton<MockDataService>(); // Use Singleton for mock data

await builder.Build().RunAsync();
