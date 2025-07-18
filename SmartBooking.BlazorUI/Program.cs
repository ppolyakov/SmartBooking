using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor.Services;
using SmartBooking.BlazorUI.Services;
using SmartBooking.BlazorUI.Services.Interfaces;
using SmartBooking.BlazorUI.Services.Provider;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthStateProvider>());

builder.Services.AddHttpClient<ITimeSlotsService, TimeSlotService>(client =>
  {
      client.BaseAddress = new Uri("https://localhost:7125/api/");
  });
builder.Services.AddHttpClient<IClientService, ClientService>(client =>
  {
      client.BaseAddress = new Uri("https://localhost:7125/api/");
  });
builder.Services.AddHttpClient<IServiceService, ServiceService>(client =>
  {
      client.BaseAddress = new Uri("https://localhost:7125/api/");
  });
builder.Services.AddHttpClient<IBookingService, BookingService>(client =>
  {
      client.BaseAddress = new Uri("https://localhost:7125/api/");
  });
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
  {
      client.BaseAddress = new Uri("https://localhost:7125/api/");
  });
builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7125/api/");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();