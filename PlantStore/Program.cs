using BusinessLogic;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
    options.AppendTrailingSlash = true;
});

builder.Services.AddBusinessLogic(builder.Configuration);

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

var adminImagesPath = Path.Combine(
    Directory.GetCurrentDirectory(),
    "..", "AdminPanel", "wwwroot", "images"
);
if (Directory.Exists(adminImagesPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(adminImagesPath),
        RequestPath = "/images"  //отдельный путь
    });
}

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
