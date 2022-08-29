/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace EnhancedRelationships
{
   internal class ModConfig
    {
        //If the player should receive mail on the day before the npc's birthday
        public bool GetMail => true;

        //Amount of Gifts the npc's require to keep them happy for a whole week.
        public int AmtOfGiftsToKeepNpcHappy => 1;

        //Internal Basic Amount. Part of the calculations
        public int BasicAmount => 10;

        //Bool that tells if the player gets punished for missing Birthdays.
        public bool EnableMissedBirthdays => true;

        //Bool that tells the mod if numbers need to be rounded
        public bool EnableRounded => false;

        //Heart Multiplier
        public float[] HeartMultiplier { get; } = {
            1f,
            0.9f,
            0.8f,
            0.7f,
            0.6f,
            0.5f,
            0.4f,
            0.3f,
            0.2f,
            0.15f,
            0.1f
        };
        //Heart Multiplier for Birthdays
        public float[] BirthdayHeartMultiplier { get; } = {
            0.1f,
            0.15f,
            0.2f,
            0.3f,
            0.4f,
            0.5f,
            0.6f,
            0.7f,
            0.8f,
            0.9f,
            1f
        };
        //Birthday Multiplier
        public float BirthdayMultiplier => 5f;
    }
}
