using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.Persistence;

public class ChatAppDbInitializer
{
    private readonly ILogger<ChatAppDbInitializer> _logger;
    private readonly ChatAppDbContext _context;
    private readonly IHostEnvironment _environment;

    public ChatAppDbInitializer(
        ILogger<ChatAppDbInitializer> logger,
        ChatAppDbContext context,
        IHostEnvironment environment)
    {
        _logger = logger;
        _context = context;
        _environment = environment;
    }

    public async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Starting database initialization...");
            
            if (_context.Database.IsNpgsql())
            {
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Database migration completed successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }
}

public static class ChatAppDbInitializerExtensions
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<ChatAppDbInitializer>();
        await initializer.InitializeAsync();
    }
}