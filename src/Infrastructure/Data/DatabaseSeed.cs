using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Domain.Constants;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Data;

public static class DatabaseSeed
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await context.Roles.AnyAsync())
            return;

        context.Roles.AddRange(
            Role.Create(RoleNames.Admin, "Full system access"),
            Role.Create(RoleNames.Member, "Standard task management access"));

        await context.SaveChangesAsync();
    }
}
