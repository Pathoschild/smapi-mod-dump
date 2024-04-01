/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

namespace UndergroundSecrets
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public float PuzzleChanceIncreaseRate { get; set; } = 0.3f;
        public float TilePuzzleBaseChance { get; set; } = 0.01f;
        public float LightPuzzleBaseChance { get; set; } = 0.01f;
        public float OfferingPuzzleBaseChance { get; set; } = 0.001f;
        public float AltarBaseChance { get; set; } = 0.01f;
        public float RiddlesBaseChance { get; set; } = 0.01f;
        public float CollapsedBaseFloorMaxPortion { get; set; } = 0.001f;
        public float TrapsBaseMaxPortion { get; set; } = 0.001f;
        public float MushroomTreesMaxPortion { get; set; } = 0.001f;
        public float DisarmTrapsBaseChanceModifier { get; set; } = 1f;
        public bool ShowTrapNotifications { get; set; } = true;
        public float AltarBuffMult { get; set; } = 0.5f;
        public bool OverrideTreasureRooms { get; set; } = true;
        public float TrapDamageMult { get; set; } = 2f;
    }
}
