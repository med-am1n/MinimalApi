using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Data;

// A self Contained Unit of Work breaking down into Three main things:
public class GetCustomerById :  IEndpoint
{

    // 1- EndPoint Mapping
    public static void Map(IEndpointRouteBuilder app) => app
    .MapGet("/customers/{id}", HandleAsync)
    .WithName("GetCustomerById")
    .WithSummary("Get a Customer by id");


    // 2- Request Response Contract
    public record Response(Guid Id, string FullName);


    // 3- Handler (in MinimalApi we can provide Static methods as hundlers)
    private static async Task<Results<Ok<Response>, NotFound>> HandleAsync( Guid id, [FromServices] CustomerRepository repo, CancellationToken cancellationToken)
    {

        var customer = repo.GetById(id);

        if (customer == null)
        {
            return TypedResults.NotFound();
        }

        var response = new Response(customer.Id, customer.FullName);

        return TypedResults.Ok(response);
    }

}