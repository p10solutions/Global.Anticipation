using System.ComponentModel.DataAnnotations;

namespace Global.Anticipation.API.Transport
{
    public record CreateAnticipationRequest(
        [Required] Guid CreatorId,
        [Required] decimal RequestedAmount,
        [Required] DateTime RequestDate
    );
}
