/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewHitboxes
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace StardewHitboxes
{
    public class ModConfig
    {
        public bool Enabled { get; set; } = true;

        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("F2");

        public string FarmerHitboxColor { get; set; } = "#32CD32";

        public string CharacterHitboxColor { get; set; } = "#BF40BF";

        public string MonsterHitboxColor { get; set; } = "#FF0000";

        public string WeaponSwingHitboxColor { get; set; } = "#FFA500";

        public string ProjectileHitboxColor { get; set; } = "#FFA500";

        public string TerrainFeatureHitboxColor { get; set; } = "#3432A8";

        public string ObjectsHitboxColor { get; set; } = "#3432A8";

        public float HitboxOpacity { get; set; } = 0.6f;
    }
}