using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Data;
using FluentValidation;
using RequestValidationInMinimalAPIs.Filters;
public class UpdateCustomer :  IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
    .MapPut("/customers", HandleAsync)
    .WithName("UpdateCustomer")
    .WithSummary("Update an customer")
    .WithRequestValidation<Request>();


    // 2- Request Response Contract
    public record Request(string FullName);
    public record Response(Guid Id, string FullName);

    // 3- Handler
    private static async Task<Results<Ok<Response>, NotFound>> HandleAsync(Guid id, Request request, [FromServices] CustomerRepository repo)
    {
        if (string.IsNullOrEmpty(request.FullName))
        {
            throw new ProblemException("Empty Field FullName", "FullName cannot be empty!");
        }

        var customer = repo.GetById(id);
        if (customer is null)
        {
            return TypedResults.NotFound();
        }

        var updatedCustomer = new Customer(id, request.FullName);

        repo.Update(updatedCustomer);
        var response = new Response(updatedCustomer.Id, updatedCustomer.FullName);

        return TypedResults.Ok(response);
    }
    

    // Validator
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}