/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mikesnorth/StardewNewsFeed
**
*************************************************/

using System.Collections.Generic;

namespace StardewNewsFeed.Wrappers {
    /// <summary>
    /// Wrapper for StardewValley.GameLocation
    /// </summary>
    public interface ILocation {

        string GetDisplayName();
        string GetLocationName();
        //IList<IStardewObject> GetObjects();
        IStardewObject GetObjectAtTile(int height, int width);
        //IList<ITerrainFeature> GetTerrainFeatures<T>() where T : StardewValley.TerrainFeatures.TerrainFeature;
        bool IsGreenhouse();
        IList<NPC> GetCharacters();
        IList<NPC> GetCharactersWithBirthdays();
        int GetNumberOfHarvestableObjects();
        int GetNumberOfHarvestableTerrainFeatures<T>() where T : StardewValley.TerrainFeatures.TerrainFeature;
    }
}
