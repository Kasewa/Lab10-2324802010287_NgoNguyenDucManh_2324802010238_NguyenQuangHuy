using ASC.DataAccess;
using ASC.Web;
using ASC.Web.Configuration;
using ASC.Web.Data;
using ASC.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ASC.Web.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // Increase command timeout to 60 seconds and enable retry on failure for transient faults
        sqlOptions.CommandTimeout(60);
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
    }));
builder.Services.AddScoped<DbContext, ASC.Web.Data.ApplicationDbContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ASC.Web.Data.ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Options / Settings
builder.Services.AddOptions();
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("AppSettings"));

// MVC + Razor Pages
builder.Services.AddControllersWithViews();

// Application services
builder.Services.AddTransient<IEmailSender, AuthMessageSender>();
builder.Services.AddTransient<ISmsSender, AuthMessageSender>();
builder.Services.AddSingleton<IIdentitySeed, IdentitySeed>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Custom services: session, cache, navigation
builder.Services.AddMyDependencyGroup(builder.Configuration);

var app = builder.Build();

// HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

// Area routing must come before default routing
app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed identity data and load navigation cache
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ASC.Web.Data.ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    var storageSeed = scope.ServiceProvider.GetRequiredService<IIdentitySeed>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var options = scope.ServiceProvider.GetRequiredService<IOptions<ApplicationSettings>>();
    await storageSeed.Seed(userManager, roleManager, options);

    var navigationCache = scope.ServiceProvider.GetRequiredService<INavigationCacheOperations>();
    await navigationCache.CreateNavigationCacheAsync();
}

app.Run();
