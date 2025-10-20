using Microsoft.AspNetCore.Http.Features;
using Serilog;
using Data;
using FluentValidation;

namespace Config;


public static class ConfigureServices
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.AddSerilog();
        builder.AddSwagger();
        builder.AddDatabase();
        builder.AddJwtAuthentication();
        builder.AddProblemDetails();
        builder.AddExceptionHandling();
        builder.Services.AddValidatorsFromAssembly(typeof(ConfigureServices).Assembly);
        
        // When using  IValidator<Request> as a parameter in the handler method. (No use of Global Validation Using Filters)
        //builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomer.RequestValidator>();

    }

    private static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));
            options.InferSecuritySchemes();
        });
    }

    private static void AddProblemDetails(this WebApplicationBuilder builder)
    {
        // Extension methods used to add custom information whenever a problemDetails is instantiated in the application.
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requesrId", context.HttpContext.TraceIdentifier);
                var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            };
        });
    }

    private static void AddExceptionHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<ProblemExceptionHandler>();

    }

    private static void AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });
    }

    private static void AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<CustomerRepository>();
        // builder.Services.AddDbContext<AppDbContext>(options =>
        // {
        //     options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
        // });
    }

    private static void AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        // builder.Services.AddAuthentication().AddJwtBearer(options =>
        // {
        //     options.TokenValidationParameters = new TokenValidationParameters
        //     {
        //         IssuerSigningKey = Jwt.SecurityKey(builder.Configuration["Jwt:Key"]!),
        //         ValidateIssuer = false,
        //         ValidateAudience = false,
        //         ValidateLifetime = true,
        //         ValidateIssuerSigningKey = true,
        //         ClockSkew = TimeSpan.Zero
        //     };
        // });
        // builder.Services.AddAuthorization();

        // builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
        // builder.Services.AddTransient<Jwt>();
    }
}
