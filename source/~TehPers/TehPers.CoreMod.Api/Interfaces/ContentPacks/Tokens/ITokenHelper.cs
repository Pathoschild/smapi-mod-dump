namespace TehPers.CoreMod.Api.ContentPacks.Tokens {
    public interface ITokenHelper {
        string GetConfigValue(string key);
        string GetLocalizedValue(string key);
    }
}