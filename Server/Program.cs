using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=temperature.db"));

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactNative",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddSignalR();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ** Đặt UseCors trước MapControllers để áp dụng CORS cho API **
app.UseCors("AllowReactNative");

// Nếu có xác thực, đặt app.UseAuthentication(); và app.UseAuthorization(); ở đây

// Map các route
app.MapControllers();              // API controller
app.MapRazorPages();               // Razor pages
app.MapBlazorHub();                // Blazor SignalR hub
app.MapHub<MessHub>("/MessHub");  // SignalR hub riêng (messaging)
app.MapFallbackToPage("/_Host");  // fallback cho Blazor

app.Run();
