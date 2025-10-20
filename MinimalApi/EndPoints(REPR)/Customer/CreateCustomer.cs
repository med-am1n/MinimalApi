using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Data;
using FluentValidation;
using RequestValidationInMinimalAPIs.Filters;



// An endpoint is self contained unit of work, broken down into three main things,
public class CreateCustomer : IEndpoint
{
    // 1- EndPoint Mapping
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/customers", HandleAsync)
        .WithName("CreateCustomer")
        .WithSummary("Creates a new customer")
        .WithRequestValidation<Request>();

    // 2- Request Response Contract
    public record Request(string FullName);
    public record Response(Guid Id, string FullName);

    // 3- Handler
    private static async Task<Results<Created<Response>, BadRequest>> HandleAsync(Request request, [FromServices] CustomerRepository repo)
    {
        var customer = new Customer(Guid.NewGuid(), request.FullName);
        repo.Create(customer);

        var response = new Response(customer.Id, customer.FullName);

        return TypedResults.Created($"/customers/{customer.Id}", response);
    }

    // When using  IValidator<Request> as a parameter in the handler method. (No use of Global Validation Using Filters)

    // private static async Task<Results<Created<Response>, BadRequest>> HandleAsync(Request request, IValidator<Request> validator, [FromServices] CustomerRepository repo)
    // {

    //     var context = new ValidationContext<CreateCustomer.Request>(request);
    //     var result = validator.Validate(context);
    //     if (!result.IsValid)
    //     {
    //         var validationErrorMessage = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));

    //         throw new ProblemException("Validation Failed", validationErrorMessage);
    //         // return Results.ValidationProblem(result.ToDictionary());
    //     }

    //     var customer = new Customer(Guid.NewGuid(), request.FullName);
    //     repo.Create(customer);

    //     var response = new Response(customer.Id, customer.FullName);

    //     return TypedResults.Created($"/customers/{customer.Id}", response);
    // }



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
