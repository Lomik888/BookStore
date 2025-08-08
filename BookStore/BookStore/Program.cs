using BookStore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<MigrationsRunner>();

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
var migrationRunner = scope.ServiceProvider.GetRequiredService<MigrationsRunner>();
await migrationRunner.RunAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();