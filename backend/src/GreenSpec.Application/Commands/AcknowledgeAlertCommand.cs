using GreenSpec.Application.DTOs;
using MediatR;

namespace GreenSpec.Application.Commands;

public record AcknowledgeAlertCommand(int AlertId) : IRequest<AlertDto>;
