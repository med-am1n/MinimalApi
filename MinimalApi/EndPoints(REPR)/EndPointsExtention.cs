
using Authentication.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;
public static class EndpointsExtention
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("")
        .AddEndpointFilter<RequestLoggingFilter>()
        .WithOpenApi();

        endpoints.MapAuthenticationEndpoints();
        endpoints.MapCustomerEndpoints();
    }


    private static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/customers")
            .WithTags("Customers");

        endpoints.MapPublicGroup()
            .MapEndpoint<GetAllCustomers>()
            .MapEndpoint<GetCustomerById>();

        endpoints.MapAuthorizedGroup()
            .MapEndpoint<CreateCustomer>()
            .MapEndpoint<UpdateCustomer>()
            .MapEndpoint<DeleteCustomer>();
    }

    private static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/auth")
            .WithTags("Authentication");

        endpoints.MapPublicGroup()
            .MapEndpoint<Signup>()
            .MapEndpoint<Login>();
    }

    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .AllowAnonymous();
    }


    private static RouteGroupBuilder MapAuthorizedGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .RequireAuthorization()
            .WithOpenApi(x => new(x)
            {
                Security = [new() { [securityScheme] = [] }],
            });
    }

    private static readonly OpenApiSecurityScheme securityScheme = new()
    {
        Type = SecuritySchemeType.Http,
        Name = JwtBearerDefaults.AuthenticationScheme,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Reference = new()
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };


    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }

}
