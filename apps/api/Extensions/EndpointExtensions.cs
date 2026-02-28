namespace Api.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api")
            .RequireAuthorization("AuthenticatedUsers");

        group.MapGet("/user", GetUser)
            .WithName("GetUser")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }

    private static UserDto GetUser(HttpContext context) =>
        new(
            Id: context.User.FindFirst("sub")?.Value,
            Username: context.User.FindFirst("preferred_username")?.Value,
            Email: context.User.FindFirst("email")?.Value
        );
}

public record UserDto(string? Id, string? Username, string? Email);
