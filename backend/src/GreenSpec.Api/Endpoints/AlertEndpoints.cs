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
            ISender sender,
            CancellationToken cancellationToken,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10) =>
        {
            var query = new GetPagedAlertsQuery(status, from, to, pageNumber, pageSize);
            var result = await sender.Send(query, cancellationToken);

            return Results.Ok(result);
        })
        .WithName("GetAlerts")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get alerts";
            operation.Description = "Retrieve alerts with pagination and optional filters (status, date range)";
            return operation;
        })
        .Produces<PagedResultDto<AlertDto>>()
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
