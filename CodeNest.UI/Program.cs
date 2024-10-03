using CodeNest.BLL.AutoMapper;
using CodeNest.BLL.Repositories;
using CodeNest.BLL.Service;
using CodeNest.DAL.Context;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<MangoDbService>();
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IFormatterServices, FormatterServices>();
builder.Services.AddTransient<IJsonService, JsonService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("", _client =>
{
    _client.Timeout=TimeSpan.FromMinutes(5);
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout period
    options.Cookie.HttpOnly = true; // Cookie settings
    options.Cookie.IsEssential = true;
});
WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=LoginBasic}/{id?}");

app.Run();
