using System.Text.Json.Serialization;
using GalvaERP.Features.PurchaseReturns.DTOs;
using MediatR;

namespace GalvaERP.Features.PurchaseReturns.Commands;

public record UpdateReturnCommand(
    string Doku,
    string STS,
    string? Memo,
    bool Validasi,
    string? StatusGL) : IRequest<ReturnDetailDto>
{
    [JsonIgnore]
    public byte[] IfMatchRowVersion { get; init; } = Array.Empty<byte>();
}
