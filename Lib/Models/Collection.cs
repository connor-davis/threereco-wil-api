using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace three_api.Lib.Models;


public record Collection(
    Guid Id,
    Guid BusinessId,
    Guid CollectorId,
    Guid ProductId,
    string Weight
)
{
    [NotMapped]
    public Business? Business { get; set; }

    [NotMapped]
    public Collector? Collector { get; set; }

    [NotMapped]
    public Product? Product { get; set; }
}