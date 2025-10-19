namespace Data;
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