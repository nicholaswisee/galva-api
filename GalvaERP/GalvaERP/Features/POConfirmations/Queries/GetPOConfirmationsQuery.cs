using System.Collections.Generic;
using GalvaERP.Features.POConfirmations.DTOs;
using MediatR;

namespace GalvaERP.Features.POConfirmations.Queries;

public record GetPOConfirmationsQuery : IRequest<List<POConfirmationListDto>>;
