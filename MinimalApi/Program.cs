using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<CustomerRepository>();


builder.Services.AddEndpointsApiExplorer(); // Required for minimal APIs
builder.Services.AddSwaggerGen();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

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
        return Results.BadRequest();
        
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