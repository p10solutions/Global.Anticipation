using System.ComponentModel.DataAnnotations;

namespace Global.Anticipation.API.Transport
{
    public enum StatusRequest
    {
        Approved = 2,
        Rejected
    }

    public record UpdateStatusRequest([Required] StatusRequest Status);
}
