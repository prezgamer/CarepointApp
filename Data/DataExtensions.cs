using Microsoft.EntityFrameworkCore;

namespace CarePointApp.Data;

public static class DataExtensions
{
    // Allow Auto Migration when App.Start
    public static void MigrateDb(this WebApplication app)
    {
        using var scope  = app.Services.CreateScope();
        var dbContext 
            = scope.ServiceProvider.GetRequiredService<CarePointDataContext>();
        
        dbContext.Database.Migrate();
    }
}
