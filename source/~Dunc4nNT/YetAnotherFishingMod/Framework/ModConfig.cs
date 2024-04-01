/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

namespace NeverToxic.StardewMods.YetAnotherFishingMod.Framework
{
    internal class ModConfig
    {
        public ModConfigKeys Keys { get; set; } = new();

        public int CatchFishRetries { get; set; } = 0;

        public bool InstantCatchFish { get; set; } = true;

        public bool InstantCatchTreasure { get; set; } = false;

        public bool AlwaysMaxCastingPower { get; set; } = false;

        public bool AlwaysPerfect { get; set; } = false;

        public TreasureAppearanceSettings TreasureAppearence { get; set; } = TreasureAppearanceSettings.Vanilla;

        public bool AlwaysCatchDouble { get; set; } = false;

        public bool InstantBite { get; set; } = false;

        public bool AutoHook { get; set; } = false;

        public bool SpawnBaitWhenEquipped { get; set; } = false;

        public Quality FishQuality { get; set; } = Quality.Any;

        public Quality MinimumFishQuality { get; set; } = Quality.None;

        public string SpawnWhichBait { get; set; } = "(O)685";

        public bool SpawnTackleWhenEquipped { get; set; } = false;

        public string SpawnWhichTackle { get; set; } = "(O)686";

        public bool OverrideAttachmentLimit { get; set; } = false;

        public bool ResetAttachmentsWhenNotEquipped { get; set; } = true;

        public bool InfiniteBait { get; set; } = false;

        public bool InfiniteTackle { get; set; } = false;

        public bool AutoLootTreasure { get; set; } = false;

        public float DifficultyMultiplier { get; set; } = 1.0f;

        public bool DoAddEnchantments { get; set; } = false;

        public bool AddAllEnchantments { get; set; } = false;

        public bool AddAutoHookEnchantment { get; set; } = false;

        public bool AddEfficientToolEnchantment { get; set; } = false;

        public bool AddMasterEnchantment { get; set; } = false;

        public bool AddPreservingEnchantment { get; set; } = false;

        public bool ResetEnchantmentsWhenNotEquipped { get; set; } = true;
    }
}
