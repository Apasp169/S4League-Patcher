using Newtonsoft.Json;

namespace S4LeaguePatcher.models;

public class LoginRequest
{
    [JsonProperty("service_code")] public string ServiceCode { get; set; } = string.Empty;

    [JsonProperty("input_user_id")] public string InputUserId { get; set; } = string.Empty;

    [JsonProperty("input_user_password")] public string InputUserPassword { get; set; } = string.Empty;
}