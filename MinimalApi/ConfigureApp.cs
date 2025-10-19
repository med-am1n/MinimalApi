using Serilog;

namespace Config;

public static class ConfigureApp
{
    public static async Task Configure(this WebApplication app)
    {


        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                    c.RoutePrefix = string.Empty;  // This makes Swagger the default page
                });

        }
        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseExceptionHandler();

        // EndPoints
        // app.AddCustomersEndPoints();

        // EndPoints (REPR)
        app.MapEndpoints();
    }

    private static async Task EnsureDatabaseCreated(this WebApplication app)
    {
        // using var scope = app.Services.CreateScope();
        // var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // await db.Database.MigrateAsync();
    }
}