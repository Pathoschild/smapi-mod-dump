/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

namespace weizinai.StardewValleyMod.CustomMachineExperience.Framework;

internal class ExperienceData
{
    public int FarmingExperience { get; set; }
    public int FishingExperience { get; set; }
    public int ForagingExperience { get; set; }
    public int MiningExperience { get; set; }
    public int CombatExperience { get; set; }

    public override string ToString()
    {
        return $"Farming {this.FarmingExperience} Fishing {this.FishingExperience} Foraging {this.ForagingExperience} Mining {this.MiningExperience} Combat {this.CombatExperience}";
    }
}