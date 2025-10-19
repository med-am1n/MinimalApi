using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Data;
public class GetAllCustomers : IEndpoint
{
    // 1- EndPoint Mapping
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/customers", HandleAsync)
        .WithSummary("Gets all customers");

    // 2- Request Response Contract
    public record ResponseItem(Guid Id, string FullName);
    public record Response(
        int NumberOfCustomers,
        IReadOnlyCollection<ResponseItem> Data
    );

    // 3- Handler
    private static async Task<Results<Ok<Response>, NotFound>> HandleAsync([FromServices] CustomerRepository repo, CancellationToken cancellationToken)
    {
        var customers = repo.GetAll();

        if (customers == null || !customers.Any())
        {
            return TypedResults.NotFound();
        }

        // Map each customer to ResponseItem
        var responseItems = customers
            .Select(customer => new ResponseItem(customer.Id, customer.FullName))
            .ToList();

        // Create the Response with count and data
        var response = new Response(
            NumberOfCustomers: responseItems.Count,
            Data: responseItems
        );

        return TypedResults.Ok(response);
    }
}
