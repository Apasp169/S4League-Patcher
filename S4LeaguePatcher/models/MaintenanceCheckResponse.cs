using Newtonsoft.Json;

namespace S4LeaguePatcher.models;

public class MaintenanceCheckResponse
{
    [JsonProperty("result_code")] public int ResultCode { get; set; }

    [JsonProperty("isMT")] public int IsMaintenance { get; set; }

    [JsonProperty("msg")] public string Message { get; set; } = string.Empty;

    [JsonProperty("ip")] public string Ip { get; set; } = string.Empty;

    [JsonProperty("play")] public bool CanPlay { get; set; }
}