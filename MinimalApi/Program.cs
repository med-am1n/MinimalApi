using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<CustomerRepository>();


builder.Services.AddEndpointsApiExplorer(); // Required for minimal APIs
builder.Services.AddSwaggerGen();


// Extension methods used to add custom information whenever a problemDetails is instantiated in the application.
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requesrId", context.HttpContext.TraceIdentifier);
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});



builder.Services.AddExceptionHandler<ProblemExceptionHandler>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}


app.UseExceptionHandler();


app.MapGet("/customers", ([FromServices] CustomerRepository repo) =>
{
    return repo.GetAll();
});


app.MapGet("/customers/{id}", ([FromServices] CustomerRepository repo, Guid id) =>
{

    var customer = repo.GetById(id);
    return customer is not null ? Results.Ok(customer) : Results.NotFound();
});


app.MapPut("/customers/{id}", ([FromServices] CustomerRepository repo, Guid id, Customer updatedCustomer) =>
{

    var customer = repo.GetById(id);
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



app.MapPost("/customers", ([FromServices] CustomerRepository repo, Customer customer) =>
{
    if (string.IsNullOrEmpty(customer.FullName))
    {
        // Instantiation of ProblemDetails
        // return Results.Problem(
        //     type: "Bad Request",
        //     title: "Empty Field FullName",
        //     detail: "FullName can not be empty!",
        //     statusCode: StatusCodes.Status400BadRequest);

        throw new ProblemException("Empty Field FullName", "FullName can not be empty!"); 
        
    }
    repo.Create(customer);
    return Results.Created($"/customers/{customer.Id}", customer.Id);
});

app.Run();



record Customer(Guid Id, string FullName);

class CustomerRepository
{
    private readonly Dictionary<Guid, Customer> _customers = new();

    public void Create(Customer customer)
    {
        if (customer is null)
        {
            return;
        }
        _customers[customer.Id] = customer;
    }

    public Customer GetById(Guid id)
    {
        return _customers[id];
    }

    public List<Customer> GetAll()
    {
        return _customers.Values.ToList();
    }


    public void Update(Customer customer)
    {
        var existingCustomer = GetById(customer.Id);
        if (existingCustomer is null)
        {
            return;
        }

        _customers[customer.Id] = customer;
    }


    public void Delete(Guid id)
    {
        _customers.Remove(id);
    }
}