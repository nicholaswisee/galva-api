using System;
using System.Collections.Generic;
using GalvaERP.Features.POConfirmations.DTOs;
using MediatR;

namespace GalvaERP.Features.POConfirmations.Commands;

public record UpdatePOConfirmationCommand(
    string Doku,
    string Doku_PO,
    DateTime Tgl,
    string? ContactPr,
    DateTime? Psd,
    DateTime? Etd,
    string? Memo,
    List<POConfirmationLineDto> LineItems,
    byte[] IfMatchRowVersion) : IRequest<POConfirmationDetailDto>;
