using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StardewModdingAPI;
using TehPers.Core.Drawing;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Rewrite {
    public static class ModExtensions {
        private static readonly Dictionary<IMod, TehCoreApi2> _apiRegistry = new Dictionary<IMod, TehCoreApi2>();

        /// <summary>Gets a unique instance of <see cref="ITehCoreApi"/> for this mod. It can be used to access all APIs loaded by your assembly in the <see cref="TehPers.Core"/> namespace.</summary>
        /// <param name="owner">The mod that owns the API being requested.</param>
        /// <returns>An instance of <see cref="ITehCoreApi"/> that is unique for this mod.</returns>
        public static ITehCoreApi GetCoreApi(this IMod owner) {
            DrawingDelegator.PatchIfNeeded();

            return ModExtensions._apiRegistry.GetOrAdd(owner, () => {
                TehCoreApi2 core = new TehCoreApi2(owner);
                core.Log("Core", "Core API created", LogLevel.Debug);
                return core;
            });
        }
    }
}