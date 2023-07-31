/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace ShowPlayerBehind
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public float InnerTransparency { get; set; } = 0.6f;
        public float OuterTransparency { get; set; } = 0.7f;
        public float CornerTransparency { get; set; } = 0.8f;
}
}