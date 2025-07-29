using JoelComponents.Components;
using JoelComponents.Services;
using JoelComponents.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient();
//AddJoelComponents. This is needed to inject dependencies :)
builder.Services.AddJoelComponents();
builder.Services.AddSingleton<CatApiService>();
builder.Services.AddSingleton<PicSumService>();
builder.Services.AddSingleton<PersonNotExistService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();