/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/phrasefable/StardewMods
**
*************************************************/

using JetBrains.Annotations;

namespace NoOre
{
    public interface IModConfig
    {
        bool ReplaceOres { get; }
        bool ReplaceGemNodes { get; }
        bool ReplaceMysticStone { get; }
        bool ReplaceGeodeNodes { get; }
    }


    public class ModConfig : IModConfig
    {
        public bool ReplaceOres { get; [UsedImplicitly] set; }
        public bool ReplaceGemNodes { get; [UsedImplicitly] set; }
        public bool ReplaceMysticStone { get; [UsedImplicitly] set; }
        public bool ReplaceGeodeNodes { get; [UsedImplicitly] set; }
    }
}