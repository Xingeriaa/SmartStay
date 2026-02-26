using do_an_tot_nghiep.Models;
using do_an_tot_nghiep.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

#region SERVICES

// 1. Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection configuration.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// 2. Authentication (Cookie)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

// 3. HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// 4. Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 5. MVC + API + SignalR
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

// 6. Swagger (CHỈ CHẠY DEVELOPMENT – CHUẨN BẢO MẬT)
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartStay API",
        Version = "v1",
        Description = "API quản lý nhà trọ SmartStay"
    });

    // XML Documentation (đã fix crash)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

// 7. HttpClient
builder.Services.AddHttpClient("ApiClient", (sp, client) =>
{
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    if (httpContext != null)
    {
        // Tự động detect port/host của server đang chạy (chống timeout 2s)
        client.BaseAddress = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host.Value}");
    }
    else
    {
        var baseUrl = builder.Configuration["ApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(baseUrl))
        {
            client.BaseAddress = new Uri(baseUrl);
        }
    }
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    UseProxy = false, // Vô hiệu hóa proxy tự động (Fix triệt để delay 2s do WPAD)
    PooledConnectionLifetime = TimeSpan.FromMinutes(2)
});

// 8. Dependency Injection
builder.Services.AddScoped<IPhongTroService, PhongTroService>();
builder.Services.AddScoped<IKhachThueService, KhachThueService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IDichVuService, DichVuService>();
builder.Services.AddScoped<IQuanLyNhaService, QuanLyNhaService>();
builder.Services.AddScoped<IRoomAssetsService, RoomAssetsService>();

#endregion

var app = builder.Build();

#region MIDDLEWARE

// Swagger – Enabled for all environments
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartStay API v1");
    options.RoutePrefix = "swagger";
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

#endregion

#region ROUTES

// API Controllers
app.MapControllers();

// MVC Controllers (View)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Landing}/{id?}"
);


#endregion

app.Run();
