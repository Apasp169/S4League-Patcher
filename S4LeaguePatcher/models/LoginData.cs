using Newtonsoft.Json;

namespace S4LeaguePatcher.models;

public class LoginData
{
    [JsonProperty("service_code")] public string ServiceCode { get; set; } = string.Empty;

    [JsonProperty("sso_info_new")] public string SsoInfoNew { get; set; } = string.Empty;

    [JsonProperty("sso_info")] public string SsoInfo { get; set; } = string.Empty;

    [JsonProperty("email")] public string Email { get; set; } = string.Empty;

    [JsonProperty("user_id")] public string UserId { get; set; } = string.Empty;

    [JsonProperty("gender")] public string Gender { get; set; } = string.Empty;

    [JsonProperty("birthday")] public string Birthday { get; set; } = string.Empty;

    [JsonProperty("country")] public string Country { get; set; } = string.Empty;

    [JsonProperty("auth")] public string Auth { get; set; } = string.Empty;

    [JsonProperty("SSOKey")] public string SsoKey { get; set; } = string.Empty;

    [JsonProperty("check_email")] public string CheckEmail { get; set; } = string.Empty;

    [JsonProperty("member_type")] public string MemberType { get; set; } = string.Empty;

    [JsonProperty("user_serial")] public string UserSerial { get; set; } = string.Empty;

    [JsonProperty("user_profile_key")] public string UserProfileKey { get; set; } = string.Empty;

    [JsonProperty("user_ip")] public string UserIp { get; set; } = string.Empty;

    [JsonProperty("network_country")] public string NetworkCountry { get; set; } = string.Empty;

    [JsonProperty("valofe_web_token")] public string ValofeWebToken { get; set; } = string.Empty;

    [JsonProperty("nickName")] public string NickName { get; set; } = string.Empty;

    [JsonProperty("server_time")] public long ServerTime { get; set; }

    [JsonProperty("expired_at")] public long ExpiredAt { get; set; }

    [JsonProperty("vfun_use_otp")] public int VfunUseOtp { get; set; }
}