using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace three_api.Lib.Models;

public enum BusinessType
{
    Recycler,
    WasteCollector,
    BuyBackCenter
}

public record Business(
    Guid Id,
    Guid? UserId,
    string Name,
    string Description,
    string PhoneNumber,
    string Address,
    string City,
    string Province,
    string ZipCode,
    BusinessType BusinessType
)
{
    [NotMapped]
    public User? User { get; set; }

    [NotMapped]
    [JsonIgnore]
    public List<Product> Products { get; set; } = [];

    [NotMapped]
    [JsonIgnore]
    public List<Collection> Collections { get; set; } = [];
}