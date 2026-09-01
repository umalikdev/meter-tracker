using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MeterTracker;
using MeterTracker.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Plain HttpClient used to call Supabase's REST API directly from the browser.
builder.Services.AddScoped(sp => new HttpClient());
builder.Services.AddScoped<SupabaseService>();

await builder.Build().RunAsync();
