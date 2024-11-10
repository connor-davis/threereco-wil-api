using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace three_api.Lib.Models;

public record User(Guid Id, string Email)
{
    [JsonIgnore]
    public string Password { get; set; } = string.Empty;

    [JsonIgnore]
    public Roles[] Roles { get; set; } = [];

    [NotMapped]
    public string[] AvailableRoles => Roles.Select(role => role.ToString()).ToArray();

    [NotMapped]
    [JsonIgnore]
    public List<Business> Businesses { get; set; } = [];

    [NotMapped]
    [JsonIgnore]
    public List<Collector> Collectors { get; set; } = [];
}

public record UserAuthentication(string Email, string Password);