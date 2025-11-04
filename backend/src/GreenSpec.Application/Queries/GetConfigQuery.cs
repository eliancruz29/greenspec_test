using GreenSpec.Application.DTOs;
using MediatR;

namespace GreenSpec.Application.Queries;

public record GetConfigQuery() : IRequest<ConfigDto>;
