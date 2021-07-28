/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

namespace SpaceCore.Framework
{
    internal class Configuration
    {
        public bool CustomSkillPage { get; set; } = true;
        public bool WalletLegacyStyle { get; set; }
        public bool WalletOnRightOfSkillPage { get; set; }

        /// <summary>Whether to dispose extended tilesheet textures when they're no longer used by SpaceCore.</summary>
        /// <remarks>This can reduce memory usage, but can cause crashes if other mods don't handle disposal correctly.</remarks>
        public bool DisposeOldTextures { get; set; }
    }
}
