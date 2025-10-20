using FluentValidation;

namespace RequestValidationInMinimalAPIs.Filters;

public class ValidationFilter<TRequest> : IEndpointFilter
{
    private readonly IValidator<TRequest> validator;

    public ValidationFilter(IValidator<TRequest> validator)
    {
        this.validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().First();

        var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        if (!result.IsValid)
        {
            var validationErrorMessage = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));

            // throw new ProblemException("Validation Failed", validationErrorMessage);
            return Results.ValidationProblem(result.ToDictionary());
        }

        return await next(context);
    }
}

public static class ValidationExtensions
{
    // This method automatically hooks into the request pipeline 
    // and performs the validation before the Handle method is invoked.
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<ValidationFilter<TRequest>>()
            .ProducesValidationProblem();
    }
}