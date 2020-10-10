/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using UltimateTool.Framework.Configuration;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class ScytheTool : BaseTool
    {
        private readonly ScytheConfig _config;

        public ScytheTool(ScytheConfig config)
        {
            _config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return tool is MeleeWeapon && tool.Name.ToLower().Contains("scythe");
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(_config.HarvestForage && tileObj?.IsSpawnedObject == true)
            {
                TileAction(location, tile, who);
                return true;
            }
            //Bushes
            if(tileFeature is Bush bush)
            {                
                if (bush.inBloom(Game1.currentSeason, Game1.dayOfMonth))
                {
                    UBush b = (UBush)bush;
                    b.Shake(tile, false);
                }
            }
            //End Bushes
            if (tileFeature is HoeDirt dirt)
            {
                if (dirt.crop == null)
                {
                    return false;
                }

                if (_config.CutDeadCrops && dirt.crop.dead.Value)
                {
                    UseToolOnTile(new Pickaxe(), tile);
                    return true;
                }

                if (_config.HarvestCrops)
                {
                    if (dirt.crop.harvestMethod.Value == Crop.sickleHarvest)
                    {
                        return dirt.performToolAction(tool, 0, tile, location);
                    }
                    else
                    {
                        TileAction(location, tile, who);
                    }
                }
            }

                if(_config.HarvestFruit && tileFeature is FruitTree fTree)
                {
                    fTree.performUseAction(tile, location);
                    return true;
                }

                if (_config.HarvestGrass && tileFeature is Grass)
                {
                    location.terrainFeatures.Remove(tile);
                    if(Game1.getFarm().tryToAddHay(1) == 0)
                    {
                        Game1.addHUDMessage(new HUDMessage("Hay", HUDMessage.achievement_type, true, Color.LightGoldenrodYellow, new SObject(178, 1)));                        
                    }                    
                    return true;
                }

                if(_config.CutWeeds && tileObj?.Name.ToLower().Contains("weed") == true)
                {
                    UseToolOnTile(tool, tile);
                    tileObj.performToolAction(tool, location);
                    location.removeObject(tile, false);
                    return true;
                }
            return false;
        }
    }
}
