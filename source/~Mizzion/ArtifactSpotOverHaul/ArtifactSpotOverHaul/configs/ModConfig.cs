/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace ArtifactSpotOverHaul
{
    public class ModConfig
    {
        //Makes it so that the player only gets artifacts they don't have yet.
        //public bool UniqueArtifacts { get; set; } = true;

        //Enable Increased Artifact Luck
        public bool EnableArtifactLuck { get; set; } = true;
        public int ArtifactMultiplier { get; set; } = 1000;
        //Artifact chance.
       // public double ArtifactChance { get; set; } = 0.05;

        //Lost Book Chance
       // public double LostBookChance { get; set; } = 0.15;

        //Coal Chance
        //public double CoalChance { get; set; } = 0.05;

       // public LocationConfig lconfig { get; set; } = new LocationConfig();

      
    }
}
