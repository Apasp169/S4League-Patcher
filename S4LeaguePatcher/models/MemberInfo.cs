using Newtonsoft.Json;

namespace S4LeaguePatcher.models;

public class MemberInfo
{
    [JsonProperty("user_Serial")] public string UserSerial { get; set; } = string.Empty;

    [JsonProperty("user_id")] public string UserId { get; set; } = string.Empty;

    [JsonProperty("user_birthday")] public string UserBirthday { get; set; } = string.Empty;

    [JsonProperty("nickName")] public string NickName { get; set; } = string.Empty;

    [JsonProperty("email")] public string Email { get; set; } = string.Empty;

    [JsonProperty("profile_image_url")] public string ProfileImageUrl { get; set; } = string.Empty;

    [JsonProperty("user_profile_key")] public string UserProfileKey { get; set; } = string.Empty;
}