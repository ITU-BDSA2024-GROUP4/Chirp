#nullable disable

using Chirp.Core;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChirpDBContext>(options =>
{
    options.UseSqlite(builder.Configuration
        .GetConnectionString("DefaultConnection")!);

    options.UseLoggerFactory(LoggerFactory.Create(builder => builder
        .AddFilter((category, level) =>
            !category.Equals("Microsoft.EntityFrameworkCore.Database.Command") || level < LogLevel.Information)));
});

builder.Services.AddDefaultIdentity<ChirpUser>(options =>
        options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ChirpDBContext>();


builder.Services.AddAuthentication()
    .AddCookie("Github")
    .AddCookie("Cookies")
    .AddGitHub(o =>
    {
        o.ClientId = builder.Configuration["authentication_github_clientId"];
        o.ClientSecret = builder.Configuration["authentication_github_clientSecrets"];
        o.Scope.Add("user:email");
        o.CallbackPath = "/signin-github";
        o.SaveTokens = true;
    });

// Add services to the container.
builder.Services.AddRazorPages();

// Creates repositories
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();

// Creates services
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<ICheepService, CheepService>();

//adds session
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCookiePolicy(new CookiePolicyOptions()
{
    Secure = CookieSecurePolicy.Always,
    MinimumSameSitePolicy = SameSiteMode.None
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ChirpDBContext>();
    
    await DbInitializer.SeedIdentityUsers(services);
}

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();

public partial class Program { }