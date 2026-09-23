using ASP_Lesson_14.Models.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
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
builder.Services.AddAuthentication().AddGoogle(options =>
{
    var googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleAuthSection["ClientId"];
    options.ClientSecret = googleAuthSection["ClientSecret"];
}).AddGitHub(gitHubOptions=>
{
    gitHubOptions.ClientId = "Ov23li5O7KbBrO6vjXar";
    gitHubOptions.ClientSecret = "a72fccebe8a5dfcf59f2f3081822fa6006bf24a9";


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
