using System.Collections.Generic;
using UltimateTool.Framework.Configuration;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
   internal class AxeTool : BaseTool
    {
        private readonly AxeConfig Config;

        private readonly IDictionary<int, int> ResourceUpgradeNeeded = new Dictionary<int, int>
        {
            [ResourceClump.stumpIndex] = Tool.copper,
            [ResourceClump.hollowLogIndex] = Tool.steel
        };

        public AxeTool(AxeConfig config)
        {
            this.Config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return tool is Axe;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(this.Config.CutDebris && (tileObj?.Name == "Twig" || tileObj?.Name.ToLower().Contains("weed") == true))
            {
                return this.UseToolOnTile(tool, tile);
            }

            switch (tileFeature)
            {
                case Tree tree:
                    if (this.Config.CutTrees)
                    {
                        return this.UseToolOnTile(tool, tile);
                    }
                    break;
                case HoeDirt dirt when dirt.crop != null:
                    if(this.Config.CutDeadCrops && dirt.crop.dead)
                    {
                        return this.UseToolOnTile(tool, tile);
                    }
                    if(this.Config.CutLiveCrops && !dirt.crop.dead)
                    {
                        return this.UseToolOnTile(tool, tile);
                    }
                    break;
            }


            if (this.Config.CutDebris)
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
