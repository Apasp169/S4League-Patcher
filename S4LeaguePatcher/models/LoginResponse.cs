using Newtonsoft.Json;

namespace S4LeaguePatcher.models;

public class LoginResponse
{
    [JsonProperty("result")] public int Result { get; set; }
    [JsonProperty("data")] public LoginData Data { get; set; } = new();
}