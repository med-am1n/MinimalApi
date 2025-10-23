
using Authentication.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Endpoints.Posts;
using Posts.Endpoints;
using Common.Api.Filters;
public static class EndpointsExtention
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("")
        .AddEndpointFilter<RequestLoggingFilter>()
        .WithOpenApi();

        endpoints.MapAuthenticationEndpoints();
        endpoints.MapPostEndpoints();

        app.MapGet("/globalexception-test", () =>
            {
                throw new Exception("This is a test exception for global exception handling.");
            });

    }


    private static void MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/Posts")
            .WithTags("Posts");

        endpoints.MapPublicGroup()
            .MapEndpoint<GetPostById>()
            .MapEndpoint<GetPosts>();

        endpoints.MapAuthorizedGroup()
            .MapEndpoint<CreatePost>()
            .MapEndpoint<UpdatePost>()
            .MapEndpoint<DeletePost>();
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
