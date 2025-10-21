using Authentication.Services;
using Data;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RequestValidationInMinimalAPIs.Filters;

namespace Authentication.Endpoints;

public class Signup : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/signup", HandleAsync)
        .WithSummary("Creates a new user account")
        .WithRequestValidation<Request>();

    public record Request(string Username, string Password, string Name);
    public record Response(string Token);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    private static async Task<Results<Ok<Response>, BadRequest>> HandleAsync(
        Request request,
        [FromServices] UserRepository userRepository,  // Use UserRepository instead of AppDbContext
        Jwt jwt,
        CancellationToken cancellationToken)
    {
        var isUsernameTaken = userRepository.GetAll()
            .Any(x => x.Username == request.Username);

        if (isUsernameTaken)
        {
            return TypedResults.BadRequest();
        }


        var nextId = userRepository.GetAll().Any()
            ? userRepository.GetAll().Max(u => u.Id) + 1
            : 1;

        var user = new User(
            Id: nextId,
            ReferenceId: Guid.NewGuid(),
            Username: request.Username,
            Password: request.Password,
            DisplayName: request.Name
        );

        userRepository.Create(user);

        var token = jwt.GenerateToken(user);
        var response = new Response(token);
        return TypedResults.Ok(response);
    }
}

