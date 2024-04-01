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
        public bool GetMail { get; set; } = true;
        
        //Bool that tells if the player gets punished for missing Birthdays.
        public bool EnableMissedBirthdays { get; set; } = true;
        
        //Bool that tells the mod if numbers need to be rounded
        public bool EnableRounded { get; set; } = false;
        
        //Amount of Gifts the npc's require to keep them happy for a whole week.
        public int AmtOfGiftsToKeepNpcHappy { get; set; } = 1;
        
        //Internal Basic Amount. Part of the calculations
        public int BasicAmount { get; set; } = 10;
        //Multipliers
        public Multipliers HeartMultipliers { get; set; } = new();
       
        
    }
}
