using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace three_api.Lib.Models;


public record Product(
    Guid Id,
    Guid BusinessId,
    string Name,
    string Price,
    string GwCode,
    string CarbonFactor
)
{
    [NotMapped]
    public Business? Business { get; set; }

    [NotMapped]
    [JsonIgnore]
    public List<Collection> Collections { get; set; } = [];
}