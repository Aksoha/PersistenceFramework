using BlazorExample;
using BlazorExample.Settings;
using Holize.PersistenceFramework;
using Holize.PersistenceFramework.Extensions.Microsoft;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

#if DEBUG
var persistenceOption = new PersistenceOptions
{
    LocalFilesDirectory = ApplicationInfo.ExecutingDirectory
};
builder.Services.AddSingleton(persistenceOption);
#endif

builder.Services.RegisterSettings(new CounterSettings
{
    Current = 18,
    Increment = 2
});


var app = builder.Build();

app.InitializeSettings();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.InitializeSettings();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();