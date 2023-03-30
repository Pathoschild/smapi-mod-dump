/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using SObject = StardewValley.Object;

using StardewValley.TerrainFeatures;

#nullable enable

namespace MoreGrassStarters;
public interface IMoreGrassStartersAPI
{
    public Grass? GetGrass(int which, int numberOfWeeds = 4);

    public SObject? GetGrassStarter(int which);

    public Grass? GetMatchingGrass(SObject starter, int numberOfWeeds = 4);

    public SObject? GetMatchingGrassStarter(Grass grass);
}
