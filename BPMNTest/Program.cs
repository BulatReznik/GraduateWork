using BPMN;
using BPMN.Services;
using Microsoft.Extensions.FileProviders;
using System.Drawing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ApiService>();

// Добавляем HttpClient с базовым URL
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7075/api/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.UseFileServer(new FileServerOptions()
{
    FileProvider = new PhysicalFileProvider(
                   Path.Combine(app.Environment.ContentRootPath, "node_modules")
               ),
    RequestPath = "/node_modules",
    EnableDirectoryBrowsing = true
});

app.Run();


