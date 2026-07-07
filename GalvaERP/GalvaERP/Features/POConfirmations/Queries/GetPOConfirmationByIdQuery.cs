using GalvaERP.Features.POConfirmations.DTOs;
using MediatR;

namespace GalvaERP.Features.POConfirmations.Queries;

public record GetPOConfirmationByIdQuery(string Doku) : IRequest<POConfirmationDetailDto?>;
