using Authentication.Services;
using Data;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RequestValidationInMinimalAPIs.Filters;

namespace Authentication.Endpoints;


    public class Login : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapPost("/login", Handle)
            .WithSummary("Logs in a user")
            .WithRequestValidation<Request>();

        public record Request(string Username, string Password);
        public record Response(string Token);

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Username).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(
            Request request, 
            [FromServices] UserRepository userRepository,  // Use UserRepository instead of AppDbContext
            Jwt jwt, 
            CancellationToken cancellationToken)
        {
            var user = userRepository.GetAll()
                .FirstOrDefault(x => x.Username == request.Username && x.Password == request.Password);

            if (user is null || user.Password != request.Password)
            {
                return TypedResults.Unauthorized();
            }

            var token = jwt.GenerateToken(user);
            var response = new Response(token);
            return TypedResults.Ok(response);
        }
    }