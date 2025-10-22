using Common.Api.Extensions;
using Common.Api.Requests;
using Data;


namespace Endpoints.Posts;

public class GetPosts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Gets all posts")
        .WithRequestValidation<Request>();

    public record Request(int? Page, int? PageSize) : IPagedRequest;
    public class RequestValidator : PagedRequestValidator<Request>;
    public record Response(
        int Id,
        string Title,
        string? Content,
        int UserId,
        string Username,
        string UserDisplayName,
        DateTime CreateAtUtc,
        DateTime? UpdatedAtUtc
    );

    private static async Task<PagedList<Response>> Handle([AsParameters] Request request, AppDbContext database, CancellationToken cancellationToken)
    {
        return await database.Posts
            .Select(x => new Response
            (
                x.Id,
                x.Title,
                x.Content,
                x.UserId,
                x.User.Username,
                x.User.DisplayName,
                x.CreatedAtUtc,
                x.UpdatedAtUtc
            ))
            .ToPagedListAsync(request, cancellationToken);
    }
}
