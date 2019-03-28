using System.Collections.Generic;
using UltimateTool.Framework.Configuration;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class PickaxeTool : BaseTool
    {
        private readonly PickaxeConfig Config;

        private readonly IDictionary<int, int> ResourceUpgradeNeeded = new Dictionary<int, int>
        {
            [ResourceClump.meteoriteIndex] = Tool.gold,
            [ResourceClump.boulderIndex] = Tool.steel
        };

        public PickaxeTool(PickaxeConfig config)
        {
            this.Config = config;
        }
     
        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return tool is Pickaxe;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(this.Config.CutDebris && tileObj?.Name == "Stone")
            {
                return this.UseToolOnTile(tool, tile);
            }
            
            if(tileFeature is HoeDirt dirt)
            {
                if(this.Config.ClearDirt && dirt.crop == null)
                {
                    return this.UseToolOnTile(tool, tile);
                }

                if (this.Config.CutDeadCrops && dirt.crop.dead)
                {
                    return this.UseToolOnTile(tool, tile);
                }
            }

            if (this.Config.ClearBouldersAndMeteor)
            {
                ResourceClump rc = this.ResourceClumpCoveringTile(location, tile);
                if(rc != null && this.ResourceUpgradeNeeded.ContainsKey(rc.parentSheetIndex) && tool.upgradeLevel >= this.ResourceUpgradeNeeded[rc.parentSheetIndex])
                {
                    this.UseToolOnTile(tool, tile);
                }
            }
            return false;
        }
    }
}
