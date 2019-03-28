using UltimateTool.Framework.Configuration;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace UltimateTool.Framework.Tools
{
    internal class ScytheTool : BaseTool
    {
        private readonly ScytheConfig Config;

        public ScytheTool(ScytheConfig config)
        {
            this.Config = config;
        }

        public override bool IsEnabled(SFarmer who, Tool tool, Item item, GameLocation location)
        {
            return tool is MeleeWeapon && tool.name.ToLower().Contains("scythe");
        }

        public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, SFarmer who, Tool tool, Item item, GameLocation location)
        {
            if(this.Config.HarvestForage && tileObj?.isSpawnedObject == true)
            {
                this.TileAction(location, tile, who);
                return true;
            }
            //Bushes
            if(tileFeature is Bush bush)
            {                
                if (bush.inBloom(Game1.currentSeason, Game1.dayOfMonth))
                {
                    UBush b = (UBush)bush;
                    b.shake(tile, false);
                }
            }
            //End Bushes
            if (tileFeature is HoeDirt dirt)
            {
                if (dirt.crop == null)
                {
                    return false;
                }

                if (this.Config.CutDeadCrops && dirt.crop.dead)
                {
                    this.UseToolOnTile(new Pickaxe(), tile);
                    return true;
                }

                if (this.Config.HarvestCrops)
                {
                    if (dirt.crop.harvestMethod == Crop.sickleHarvest)
                    {
                        return dirt.performToolAction(tool, 0, tile, location);
                    }
                    else
                    {
                        this.TileAction(location, tile, who);
                    }
                }
            }

                if(this.Config.HarvestFruit && tileFeature is FruitTree fTree)
                {
                    fTree.performUseAction(tile);
                    return true;
                }

                if (this.Config.HarvestGrass && tileFeature is Grass)
                {
                    location.terrainFeatures.Remove(tile);
                    if(Game1.getFarm().tryToAddHay(1) == 0)
                    {
                        Game1.addHUDMessage(new HUDMessage("Hay", HUDMessage.achievement_type, true, Color.LightGoldenrodYellow, new SObject(178, 1)));                        
                    }                    
                    return true;
                }

                if(this.Config.CutWeeds && tileObj?.Name.ToLower().Contains("weed") == true)
                {
                    this.UseToolOnTile(tool, tile);
                    tileObj.performToolAction(tool);
                    location.removeObject(tile, false);
                    return true;
                }
            return false;
        }
    }
}
