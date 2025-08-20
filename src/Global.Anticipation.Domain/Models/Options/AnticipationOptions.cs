namespace Global.Anticipation.Domain.Models.Options
{
    public record AnticipationOptions
    {
        public decimal FeeRate { get; init; }
        public decimal MinimumRequestedAmount { get; init; }
    }
}