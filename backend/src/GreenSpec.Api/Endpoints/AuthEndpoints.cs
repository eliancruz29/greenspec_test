using GreenSpec.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GreenSpec.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/login", async (
            [FromBody] LoginRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginCommand(request.Username, request.Password);
            var result = await sender.Send(command, cancellationToken);

            return Results.Ok(result);
        })
        .WithName("Login")
        .WithOpenApi(operation =>
        {
            operation.Summary = "User login";
            operation.Description = "Authenticate user and return JWT token";
            return operation;
        })
        .Produces<Application.DTOs.LoginResponse>()
        .ProducesProblem(401);

        return group;
    }
}

public record LoginRequest(string Username, string Password);
