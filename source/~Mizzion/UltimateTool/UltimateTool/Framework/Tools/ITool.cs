using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal interface ITool
    {
        bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location);

        bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location);
    }
}
