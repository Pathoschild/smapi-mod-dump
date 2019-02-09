using System;
using StardewModdingAPI;

namespace TehPers.CoreMod.Api {
    public interface ICoreApiFactory {
        /// <summary>Gets the core API for the given mod.</summary>
        /// <param name="mod">The mod which owns the core API being requested.</param>
        /// <returns>The core API for the given mod.</returns>
        ICoreApi GetApi(IMod mod);

        /// <summary>Gets the core API for the given mod.</summary>
        /// <param name="mod">The mod which owns the core API being requested.</param>
        /// <param name="initialize">A callback that will be used to initialize the API.</param>
        /// <returns>The core API for the given mod.</returns>
        ICoreApi GetApi(IMod mod, Action<ICoreApiInitializer> initialize);
    }
}
