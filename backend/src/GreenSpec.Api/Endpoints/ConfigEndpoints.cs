using GreenSpec.Application.Commands;
using GreenSpec.Application.DTOs;
using GreenSpec.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GreenSpec.Api.Endpoints;

public static class ConfigEndpoints
{
    public static RouteGroupBuilder MapConfigEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetConfigQuery();
            var result = await sender.Send(query, cancellationToken);

            return Results.Ok(result);
        })
        .WithName("GetConfig")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get current configuration";
            operation.Description = "Retrieve current sensor threshold configuration";
            return operation;
        })
        .Produces<ConfigDto>()
        .RequireAuthorization();

        group.MapPut("/", async (
            [FromBody] UpdateConfigRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateConfigCommand(request.TempMax, request.HumidityMax);
            var result = await sender.Send(command, cancellationToken);

            return Results.Ok(result);
        })
        .WithName("UpdateConfig")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Update configuration";
            operation.Description = "Update sensor threshold configuration";
            return operation;
        })
        .Produces<ConfigDto>()
        .ProducesValidationProblem()
        .RequireAuthorization();

        return group;
    }
}

public record UpdateConfigRequest(decimal TempMax, decimal HumidityMax);
