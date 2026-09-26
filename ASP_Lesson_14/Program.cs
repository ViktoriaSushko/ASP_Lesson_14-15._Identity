using ASP_Lesson_14.Models.Data;
using ASP_Lesson_14.Models.DTO.Claims.CustomerPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IAuthorizationRequirement, AllowedAgeRequirement>();
builder.Services.AddTransient<IAuthorizationHandler, AllowedAgeHandler>();
//IdentityDbContext
string connStr = builder.Configuration.GetConnectionString("DefaultConnection")??throw new InvalidOperationException("Connection string wasn't provided!");
builder.Services.AddDbContext<ShopDbContext>(options => options.UseSqlServer(connStr));
builder.Services.AddIdentity<ShopUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

}).AddEntityFrameworkStores<ShopDbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyz" +
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        "абвгдеёжзийклмнопрстуфхцчшщъыьэюя" +
        "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ" +
        "іІїЇєЄґҐ" +
        "0123456789" +
        "-._@+ ";
}); ;
builder.Services.AddAuthentication().AddGoogle(options =>
{
    var googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleAuthSection["ClientId"];
    options.ClientSecret = googleAuthSection["ClientSecret"];
}).AddGitHub(gitHubOptions=>
{
    gitHubOptions.ClientId = "Ov23li3DwHFVGT4hsUXx";
    gitHubOptions.ClientSecret = "06885b070a7816e910d69f505f4f64552dd0a734";
    gitHubOptions.CallbackPath = "/signin-github";
    gitHubOptions.Scope.Add("user:email"); 
});

builder.Services.AddAuthorization(configure =>
{
    configure.AddPolicy("dotnetUsersOnly", configurePolicy =>
    {
        configurePolicy.RequireClaim("PrefferedFramework", "ASP.Net Core", ".Net MAUI");
    });
    configure.AddPolicy("minAgePolicy", configurePolicy =>
    {
        configurePolicy.Requirements.Add(new AllowedAgeRequirement { MinAge=18});
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
//Authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapControllerRoute(
    name: "childCategory",
    pattern: "category/GetChildCategories/{parentId:int?}",
    defaults: new { controller = "Category", action = "GetChildCategories" }
    );

app.Run();
