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
