using Data;
using Data.Types;
using Microsoft.EntityFrameworkCore;

namespace Common.Api.Filters;

public class EnsureEntityExistsFilter<TRequest, TEntity>(AppDbContext database, Func<TRequest, int?> idSelector) : IEndpointFilter
    where TEntity : class, IEntity
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().Single();
        var cancellationToken = context.HttpContext.RequestAborted;
        var id = idSelector(request);

        if (!id.HasValue)
        {
            return await next(context);
        }

        var exists = await database
            .Set<TEntity>()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return exists
            ? await next(context)
            : Results.NotFound($"{typeof(TEntity).Name} with id {id} was not found.");
    }
}