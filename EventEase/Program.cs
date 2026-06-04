using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EventEase.Data;
namespace EventEase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Database: SQL Server (LocalDB locally now, Azure SQL once migrated in Part 3B).
            // EnableRetryOnFailure adds transient-fault resiliency - it transparently retries
            // brief connection failures (e.g. a LocalDB instance still spinning up, or an Azure
            // SQL transient drop), which is the Microsoft-recommended pattern for cloud SQL.
            builder.Services.AddDbContext<EventEaseContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("EventEaseContext")
                        ?? throw new InvalidOperationException("Connection string 'EventEaseContext' not found."),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(8),
                        errorNumbersToAdd: null)));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // LocalDB warm-up: open a single connection (with a short retry) before the
            // first request, so the LocalDB instance is fully started up front. This
            // serialises the auto-start and prevents the "SQL Server process failed to
            // start" race that happens when the connection pool fires several
            // first-connections at once. (Harmless against Azure SQL in Part 3B.)
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<EventEaseContext>();
                for (var attempt = 1; attempt <= 5 && !db.Database.CanConnect(); attempt++)
                {
                    System.Threading.Thread.Sleep(1500);
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
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
        }
    }
}
