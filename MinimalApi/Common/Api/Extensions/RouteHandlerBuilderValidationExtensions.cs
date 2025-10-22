using Common.Api.Filters;
using Data;
using Data.Types;

namespace Common.Api.Extensions;
public static class RouteHandlerBuilderValidationExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder
            .AddEndpointFilter<RequestValidationFilter<TRequest>>()
            .ProducesValidationProblem();
    }

    public static RouteHandlerBuilder WithEnsureEntityExists<TEntity, TRequest>(this RouteHandlerBuilder builder, Func<TRequest, int?> idSelector) 
        where TEntity : class, IEntity
    {
        return builder
            .AddEndpointFilterFactory((endpointFilterFactoryContext, next) => async context =>
            {
                var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var filter = new EnsureEntityExistsFilter<TRequest, TEntity>(db, idSelector);
                return await filter.InvokeAsync(context, next);
            })
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public static RouteHandlerBuilder WithEnsureUserOwnsEntity<TEntity, TRequest>(this RouteHandlerBuilder builder, Func<TRequest, int> idSelector) 
        where TEntity : class, IEntity, IOwnedEntity
    {
        return builder
            .AddEndpointFilterFactory((endpointFilterFactoryContext, next) => async context =>
            {
                var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var filter = new EnsureUserOwnsEntityFilter<TRequest, TEntity>(db, idSelector);
                return await filter.InvokeAsync(context, next);
            })
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden);
    }
}
