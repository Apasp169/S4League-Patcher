using Newtonsoft.Json;

namespace S4LeaguePatcher.models;

public class AuthCodeResponse
{
    [JsonProperty("auth_code")] public string AuthCode { get; set; } = string.Empty;

    [JsonProperty("adult_flag")] public string AdultFlag { get; set; } = string.Empty;

    [JsonProperty("game_deep_link")] public string GameDeepLink { get; set; } = string.Empty;
}