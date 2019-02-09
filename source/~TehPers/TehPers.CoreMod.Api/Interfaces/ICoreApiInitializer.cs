using StardewModdingAPI;
using TehPers.CoreMod.Api.ContentPacks.Tokens;

namespace TehPers.CoreMod.Api {
    public interface ICoreApiInitializer {
        /// <summary>The mod that owns the <see cref="CoreApi"/>.</summary>
        IMod Owner { get; }

        /// <summary>The core API being initialized.</summary>
        ICoreApi CoreApi { get; }

        /// <summary>Registers a token which can be used by the content pack API.</summary>
        /// <param name="name"></param>
        /// <param name="token"></param>
        void RegisterToken(string name, IToken token);
    }
}