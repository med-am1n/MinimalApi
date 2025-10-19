using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Data;
public class DeleteCustomer : IEndpoint
{

    // 1- EndPoint Mapping
    public static void Map(IEndpointRouteBuilder app) => app
    .MapDelete("/customers/{id}", HandleAsync)
    .WithName("DeleteCustomer")
    .WithSummary("Delete an Customer");


    // 2- Request Response Contract
    public record Response(Guid Id, string FullName);


    // 3- Handler (in MinimalApi we can provide Static methods as hundlers)
    private static async Task<Results<Ok, NotFound>> HandleAsync(Guid id, [FromServices] CustomerRepository repo, CancellationToken cancellationToken)
    {

        var customer = repo.GetById(id);

        if (customer == null)
        {
            return TypedResults.NotFound();
        }

        repo.Delete(id);


        return TypedResults.Ok();
    }
}