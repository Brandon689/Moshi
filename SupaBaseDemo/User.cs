using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;
namespace SupaBaseDemo;

public class User : BaseModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }
}
