using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Data;
public class CreateCustomer : IEndpoint
{
    // 1- EndPoint Mapping
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/customers", HandleAsync)
        .WithName("CreateCustomer")
        .WithSummary("Creates a new customer");

    // 2- Request Response Contract
    public record Request(string FullName);
    public record Response(Guid Id, string FullName);

    // 3- Handler
    private static async Task<Results<Created<Response>, BadRequest>> HandleAsync(Request request, [FromServices] CustomerRepository repo)
    {
        if (string.IsNullOrEmpty(request.FullName))
        {
            throw new ProblemException("Empty Field FullName", "FullName cannot be empty!");
        }
        var customer = new Customer(Guid.NewGuid(), request.FullName);
        repo.Create(customer);

        var response = new Response(customer.Id, customer.FullName);

        return TypedResults.Created($"/customers/{customer.Id}", response);
    }
}
