
public static class EndpointsExtention
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("");

        endpoints.MapCustomerEndpoints();
    }


    private static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/customers")
            .WithTags("Customers");

        endpoints.MapPublicGroup()
            .MapEndpoint<CreateCustomer>()
            .MapEndpoint<GetAllCustomers>()
            .MapEndpoint<GetCustomerById>()
            .MapEndpoint<UpdateCustomer>();

        // endpoints.MapAuthorizedGroup()
        //     .MapEndpoint<UpdateCustomer>()
        //     .MapEndpoint<DeleteCustomer>();
    }

    // private static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    // {
    //     var endpoints = app.MapGroup("/auth")
    //         .WithTags("Authentication");

    //     endpoints.MapPublicGroup()
    //         .MapEndpoint<Signup>()
    //         .MapEndpoint<Login>();
    // }

    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .AllowAnonymous();
    }


    private static RouteGroupBuilder MapAuthorizedGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .RequireAuthorization();
    }
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }

}
