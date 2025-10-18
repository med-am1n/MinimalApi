using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

public static class CustomersEndPoints
{

    public static void AddCustomersEndPoints(this IEndpointRouteBuilder app)
    {

        app.MapGet("/customers", ([FromServices] CustomerRepository repo) =>
        {
            return repo.GetAll();
        });


        app.MapGet("/customers/{id}", ([FromServices] CustomerRepository repo, Guid id) =>
        {

            var customer = repo.GetById(id);
            return customer is not null ? Results.Ok(customer) : Results.NotFound();
        });


        app.MapPut("/customers/{id}", ([FromServices] CustomerRepository repo, Customer updatedCustomer) =>
        {

            var customer = repo.GetById(updatedCustomer.Id);
            if (customer is null)
            {
                return Results.NotFound();
            }
            repo.Update(updatedCustomer);
            return Results.Ok(updatedCustomer);
        });


        app.MapDelete("/customers/{id}", ([FromServices] CustomerRepository repo, Guid id) =>
        {
            repo.Delete(id);
            return Results.Ok();
        });

        app.MapPost("/customers", ([FromServices] CustomerRepository repo, string fullName) =>
        {
            if (string.IsNullOrEmpty(fullName))
            {
                // Instantiation of ProblemDetails
                // return Results.Problem(
                //     type: "Bad Request",
                //     title: "Empty Field FullName",
                //     detail: "FullName can not be empty!",
                //     statusCode: StatusCodes.Status400BadRequest);

                throw new ProblemException("Empty Field FullName", "FullName can not be empty!");

            }
            Customer customer = new Customer(Guid.NewGuid(), fullName); 
            repo.Create(customer);
            return Results.Created($"/customers/{customer.Id}", customer.Id);
        });

    }
}