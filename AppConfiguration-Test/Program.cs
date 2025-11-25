using AppConfiguration_Test.Components;
using Azure.Identity;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAzureAppConfiguration();
builder.Services.AddFeatureManagement();

if (builder.Environment.IsDevelopment() is false)
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(builder.Configuration["APPCONFIG_ENDPOINT"]), new DefaultAzureCredential())
            .ConfigureRefresh(refresh =>
                refresh.Register("Counter:IncrementBy").SetCacheExpiration(TimeSpan.FromSeconds(10)));
        
        options.UseFeatureFlags(options =>
        {
            options.CacheExpirationInterval = TimeSpan.FromSeconds(10);
        });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();