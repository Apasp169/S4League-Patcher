using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace S4LeaguePatcher.settings;

public class Settings
{
    private static readonly string SettingsFilePath = Path.Combine(
        Directory.GetCurrentDirectory(), "settings.json");

    [JsonIgnore] public static Settings Instance { get; private set; } = new();

    [JsonIgnore] public string Username { get; set; } = string.Empty;

    [JsonIgnore] public string Password { get; set; } = string.Empty;

    [JsonIgnore] public string AuthToken { get; set; } = string.Empty;

    public string GameInstallPath { get; set; } = string.Empty;

    public long AuthTokenExpiry { get; set; }

    public string EncryptedUsername { get; set; } = string.Empty;
    public string EncryptedPassword { get; set; } = string.Empty;
    public string EncryptedAuthToken { get; set; } = string.Empty;

    public async Task LoadAsync()
    {
        if (!File.Exists(SettingsFilePath))
        {
            Instance = new Settings();
            return;
        }

        var json = await File.ReadAllTextAsync(SettingsFilePath);
        var settings = JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();

        settings.Username = Decrypt(settings.EncryptedUsername);
        settings.Password = Decrypt(settings.EncryptedPassword);
        settings.AuthToken = Decrypt(settings.EncryptedAuthToken);

        Instance = settings;

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (settings.AuthTokenExpiry > 0 && settings.AuthTokenExpiry <= now)
        {
            settings.AuthToken = string.Empty;
            settings.AuthTokenExpiry = 0;
            settings.EncryptedAuthToken = string.Empty;
            await settings.SaveAsync();
        }
    }

    public async Task SaveAsync()
    {
        EncryptedUsername = Encrypt(Username);
        EncryptedPassword = Encrypt(Password);
        EncryptedAuthToken = Encrypt(AuthToken);

        var directory = Path.GetDirectoryName(SettingsFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        await File.WriteAllTextAsync(SettingsFilePath, json);
    }

    private static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return string.Empty;

        using var aes = Aes.Create();
        aes.Key = GenerateMachineKey();
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    private static string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText)) return string.Empty;

        var buffer = Convert.FromBase64String(encryptedText);

        using var aes = Aes.Create();
        aes.Key = GenerateMachineKey();
        var iv = new byte[aes.BlockSize / 8];
        Array.Copy(buffer, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }

    private static byte[] GenerateMachineKey()
    {
        var machineId = $"{Environment.MachineName}_{Environment.UserName}_{GetDriveSerial()}";
        using var sha = SHA256.Create();
        return sha.ComputeHash(Encoding.UTF8.GetBytes(machineId));
    }

    private static string GetDriveSerial()
    {
        try
        {
            var drive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)!);
            return drive.VolumeLabel + drive.TotalSize;
        }
        catch
        {
            return "fallback";
        }
    }
}