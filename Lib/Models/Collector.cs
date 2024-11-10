using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace three_api.Lib.Models;


public record Collector(
    Guid Id,
    Guid? UserId,
    string FirstName,
    string LastName,
    string IdNumber,
    string PhoneNumber,
    string Address,
    string City,
    string Province,
    string ZipCode,
    string BankName,
    string BankAccountHolder,
    string BankAccountNumber
)
{
    [NotMapped]
    public User? User { get; set; }

    [NotMapped]
    [JsonIgnore]
    public List<Collection> Collections { get; set; } = [];
}