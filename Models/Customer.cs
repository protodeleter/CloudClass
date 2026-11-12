
using System;
using Newtonsoft.Json;

namespace AvaloniaApplication1.Models;

public class Customer
{
    [JsonProperty("id")]
    public string Id { get; set; } =
        Guid.NewGuid().ToString();

    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("email")]
    public string Email { get; set; } = "";

    [JsonProperty("age")]
    public int Age { get; set; }

    [JsonProperty("category")]
    public string Category { get; set; } = "general";
}
