using GreenSpec.Application.Commands;
using GreenSpec.Application.DTOs;
using GreenSpec.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GreenSpec.Api.Endpoints;

public static class AlertEndpoints
{
    public static RouteGroupBuilder MapAlertEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            [FromQuery] string? status,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAlertsQuery(status, from, to);
            var result = await sender.Send(query, cancellationToken);

            return Results.Ok(result);
        })
        .WithName("GetAlerts")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get alerts";
            operation.Description = "Retrieve alerts with optional filters (status, date range)";
            return operation;
        })
        .Produces<IEnumerable<AlertDto>>()
        .RequireAuthorization();

        group.MapPost("/{id:int}/ack", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new AcknowledgeAlertCommand(id);
            var result = await sender.Send(command, cancellationToken);

            return Results.Ok(result);
        })
        .WithName("AcknowledgeAlert")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Acknowledge alert";
            operation.Description = "Mark an alert as acknowledged";
            return operation;
        })
        .Produces<AlertDto>()
        .ProducesProblem(404)
        .RequireAuthorization();

        return group;
    }
}
