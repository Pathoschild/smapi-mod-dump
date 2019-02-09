using TehPers.CoreMod.Api.ContentPacks.Tokens;

namespace TehPers.CoreMod.Api.ContentPacks {
    public interface IContentPackValueProvider<out T> : IContextSpecific {
        T GetValue(ITokenHelper helper);
    }
}