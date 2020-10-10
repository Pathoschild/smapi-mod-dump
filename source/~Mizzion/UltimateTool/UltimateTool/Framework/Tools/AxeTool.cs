/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using UltimateTool.Framework.Configuration;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
   internal class AxeTool : BaseTool
    {
        private readonly AxeConfig _config;

        private readonly IDictionary<int, int> _resourceUpgradeNeeded = new Dictionary<int, int>
        {
            [ResourceClump.stumpIndex] = Tool.copper,
            [ResourceClump.hollowLogIndex] = Tool.steel
        };

        public AxeTool(AxeConfig config)
        {
            _config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return tool is Axe;
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(_config.CutDebris && (tileObj?.Name == "Twig" || tileObj?.Name.ToLower().Contains("weed") == true))
            {
                return UseToolOnTile(tool, tile);
            }

            switch (tileFeature)
            {
                case Tree tree:
                    if (_config.CutTrees)
                    {
                        return UseToolOnTile(tool, tile);
                    }
                    break;
                case HoeDirt dirt when dirt.crop != null:
                    if(_config.CutDeadCrops && dirt.crop.dead.Value)
                    {
                        return UseToolOnTile(tool, tile);
                    }
                    if(_config.CutLiveCrops && !dirt.crop.dead.Value)
                    {
                        return UseToolOnTile(tool, tile);
                    }
                    break;
            }


            if (_config.CutDebris)
            {
                ResourceClump rc = ResourceClumpCoveringTile(location, tile);
                if(rc != null && _resourceUpgradeNeeded.ContainsKey(rc.parentSheetIndex.Value) && tool.UpgradeLevel >= _resourceUpgradeNeeded[rc.parentSheetIndex.Value])
                {
                    UseToolOnTile(tool, tile);
                }
            }
            return false;
        }
    }
}
