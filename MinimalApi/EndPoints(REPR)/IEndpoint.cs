// IEndpoint.cs - The interface all endpoints must implement
public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}