using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace three_api.Lib.Models;

public record User(Guid Id, string Email, Roles[] Roles)
{
    [JsonIgnore]
    public string Password { get; set; } = string.Empty;

    [NotMapped]
    [JsonIgnore]
    public List<Business> Businesses { get; set; } = [];

    [NotMapped]
    [JsonIgnore]
    public List<Collector> Collectors { get; set; } = [];
}

public record UserAuthentication(string Email, string Password);