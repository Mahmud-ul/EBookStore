using EBookStore.Models.Database;
using EBookStore.Models.Filters;
using EBookStore.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>  // Middleware for Authorization
{
    options.Filters.Add<SimpleActionFilter>();      
}).AddRazorRuntimeCompilation();

#region Cache Memory
// ⭐ THIS IS THE KEY LINE - Register Memory Cache
builder.Services.AddMemoryCache();

// Optional: Configure memory cache limits
builder.Services.Configure<MemoryCacheOptions>(options =>
{
    //options.SizeLimit = 1024 * 1024 * 100; // 100 MB limit
    options.CompactionPercentage = 0.05; // Remove 5% when pressure detected
});
#endregion

builder.Services.AddHttpContextAccessor(); //Access the session from anywhere

builder.Services.AddDbContext<ConnectionString>(s => s.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EmailService>();

//Session-1
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(20);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

var app = builder.Build();

//Session-2
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
