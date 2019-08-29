using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Locations;
using System;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

namespace AutoToolSelect
{
    public class ModConfig
    {
        public SButton ActivationKey { get; set; } = SButton.LeftControl;
        public SButton ToggleKey { get; set; } = SButton.F5;
        public bool HoeSelect { get; set; } = true;
        public bool IfNoneToolChooseWeapon { get; set; } = true;
        public bool RideHorseCursor { get; set; } = true;
        public bool PickaxeOverWareringCan { get; set; } = true;
    }

    class AutoToolSelectMod : Mod
    {
        bool togglemod = false;
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            this.Config = (ModConfig)helper.ReadConfig<ModConfig>();
            Helper.Events.Input.ButtonPressed += new EventHandler<ButtonPressedEventArgs>(this.ButtonPressed);
            Helper.Events.Input.ButtonReleased += new EventHandler<ButtonReleasedEventArgs>(this.ButtonReleased);
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == this.Config.ActivationKey && !togglemod)
            {
                Helper.Events.GameLoop.UpdateTicked += this.GameTicked;
            }
            if (e.Button == this.Config.ToggleKey)
            {
                if (togglemod)
                {
                    togglemod = false;
                    Helper.Events.GameLoop.UpdateTicked -= this.GameTicked;
                }
                else
                {
                    togglemod = true;
                    Helper.Events.GameLoop.UpdateTicked += this.GameTicked;
                }
            }
        }

        private void ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button == this.Config.ActivationKey && !togglemod)
            {
                Helper.Events.GameLoop.UpdateTicked -= this.GameTicked;
            }
        }
        
        private void GameTicked(object sender, EventArgs e)
        {
            if (Context.IsWorldReady && Context.IsPlayerFree && !Game1.player.FarmerSprite.isOnToolAnimation())
            {
                Vector2 ToolLocationVector;
                Point ToolLocationPoint;
                if (Game1.player.isRidingHorse() && this.Config.RideHorseCursor)
                {
                    ToolLocationVector = Game1.currentCursorTile;
                    ToolLocationPoint = new Point(((int)Game1.currentCursorTile.X) * Game1.tileSize + Game1.tileSize / 2, ((int)Game1.currentCursorTile.Y) * Game1.tileSize + Game1.tileSize / 2);
                }
                else
                {
                    ToolLocationVector = new Vector2((int)Game1.player.GetToolLocation(false).X / Game1.tileSize, (int)Game1.player.GetToolLocation(false).Y / Game1.tileSize);
                    ToolLocationPoint = new Point(((int)Game1.player.GetToolLocation(false).X / Game1.tileSize) * Game1.tileSize + Game1.tileSize / 2, ((int)Game1.player.GetToolLocation(false).Y / Game1.tileSize) * Game1.tileSize + Game1.tileSize / 2);
                }
                if (Game1.player.currentLocation is Farm || Game1.player.currentLocation.IsGreenhouse)
                {
                    if (this.Config.IfNoneToolChooseWeapon)
                    {
                        SetScythe();
                    }
                    if (Game1.player.currentLocation.doesTileHaveProperty((int)ToolLocationVector.X, (int)ToolLocationVector.Y, "Diggable", "Back") != null && this.Config.HoeSelect)
                    {
                        SetTool(typeof(Hoe));
                    }
                    if (Game1.player.currentLocation.doesTileHaveProperty((int)ToolLocationVector.X, (int)ToolLocationVector.Y, "Water", "Back") != null || Game1.player.currentLocation.doesTileHaveProperty((int)ToolLocationVector.X, (int)ToolLocationVector.Y, "WaterSource", "Back") != null || Game1.player.currentLocation is BuildableGameLocation && (Game1.player.currentLocation as BuildableGameLocation).getBuildingAt(ToolLocationVector) != null && ((Game1.player.currentLocation as BuildableGameLocation).getBuildingAt(ToolLocationVector).buildingType.Equals("Well") && (Game1.player.currentLocation as BuildableGameLocation).getBuildingAt(ToolLocationVector).daysOfConstructionLeft.Value <= 0))
                    {
                        SetTool(typeof(WateringCan));
                    }
                    if (Game1.player.currentLocation.objects.ContainsKey(ToolLocationVector))
                    {
                        if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Artifact Spot"))
                        {
                            SetTool(typeof(Hoe));
                        }
                        if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Stone"))
                        {
                            SetTool(typeof(Pickaxe));
                        }
                        if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Twig"))
                        {
                            SetTool(typeof(Axe));
                        }
                        if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Weeds"))
                        {
                            SetScythe();
                        }
                    }
                    if (Game1.player.currentLocation.terrainFeatures.ContainsKey(ToolLocationVector))
                    {
                        if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is HoeDirt)
                        {
                            if ((Game1.player.currentLocation.terrainFeatures[ToolLocationVector] as HoeDirt).crop != null && (((Game1.player.currentLocation.terrainFeatures[ToolLocationVector] as HoeDirt).crop.harvestMethod.Value==1 && (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] as HoeDirt).crop.fullyGrown.Value) || (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] as HoeDirt).crop.dead.Value))
                            {
                                SetScythe();
                            }
                            else
                            {
                                if (this.Config.PickaxeOverWareringCan)
                                {
                                    SetTool(typeof(Pickaxe));
                                }
                                else
                                {
                                    SetTool(typeof(WateringCan));
                                }
                            }
                        }
                        if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is GiantCrop)
                        {
                            SetTool(typeof(Axe));
                        }
                        if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is Tree)
                        {
                            SetTool(typeof(Axe));
                        }
                        if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is Grass)
                        {
                            SetScythe();
                        }
                    }
                    if(Game1.player.currentLocation is Farm)
                    {
                        for (int i = (Game1.player.currentLocation as Farm).resourceClumps.Count - 1; i >= 0; --i)
                        {
                            if ((Game1.player.currentLocation as Farm).resourceClumps[i].getBoundingBox((Game1.player.currentLocation as Farm).resourceClumps[i].tile.Value).Contains(ToolLocationPoint))
                            {
                                if ((Game1.player.currentLocation as Farm).resourceClumps[i].parentSheetIndex.Value == 622 || (Game1.player.currentLocation as Farm).resourceClumps[i].parentSheetIndex.Value == 672)
                                {
                                    SetTool(typeof(Pickaxe));
                                }
                                if ((Game1.player.currentLocation as Farm).resourceClumps[i].parentSheetIndex.Value == 600 || (Game1.player.currentLocation as Farm).resourceClumps[i].parentSheetIndex.Value == 602)
                                {
                                    SetTool(typeof(Axe));
                                }
                            }
                        }
                    }
                }

                if (!(Game1.player.currentLocation is Farm) && !Game1.player.currentLocation.IsGreenhouse)
                {
                    if (this.Config.IfNoneToolChooseWeapon)
                    {
                        SetWeapon();
                    }
                    if (Game1.player.currentLocation.doesTileHaveProperty((int)ToolLocationVector.X, (int)ToolLocationVector.Y, "Diggable", "Back") != null && this.Config.HoeSelect)
                    {
                        SetTool(typeof(Hoe));
                    }
                    if (Game1.player.currentLocation.doesTileHaveProperty((int)ToolLocationVector.X, (int)ToolLocationVector.Y, "Water", "Back") != null)
                    {
                        SetTool(typeof(FishingRod));
                    }
                    if (Game1.player.currentLocation.objects.ContainsKey(ToolLocationVector))
                    {
                        if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Artifact Spot"))
                        {
                            SetTool(typeof(Hoe));
                        }
                        if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Stone"))
                        {
                            SetTool(typeof(Pickaxe));
                        }
                        if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Twig"))
                        {
                            SetTool(typeof(Axe));
                        }
                        if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Weeds"))
                        {
                            SetWeapon();
                        }
                        if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Barrel"))
                        {
                            SetWeapon();
                        }
                    }
                    if (Game1.player.currentLocation.terrainFeatures.ContainsKey(ToolLocationVector))
                    {
                        if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is Tree)
                        {
                            SetTool(typeof(Axe));
                        }
                        if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is HoeDirt)
                        {
                            if (this.Config.PickaxeOverWareringCan)
                            {
                                SetTool(typeof(Pickaxe));
                            }
                        }
                    }
                    if (Game1.player.currentLocation is Woods)
                    {
                        for (int i = (Game1.player.currentLocation as Woods).stumps.Count - 1; i >= 0; --i)
                        {
                            if ((Game1.player.currentLocation as Woods).stumps[i].getBoundingBox((Game1.player.currentLocation as Woods).stumps[i].tile.Value).Contains(ToolLocationPoint))
                            {
                                SetTool(typeof(Axe));
                            }
                        }
                    }
                    if (Game1.player.currentLocation is Forest && (Game1.player.currentLocation as Forest).log.getBoundingBox((Game1.player.currentLocation as Forest).log.tile.Value).Contains(ToolLocationPoint))
                    {
                        SetTool(typeof(Axe));
                    }
                    if (Game1.player.currentLocation is MineShaft)
                    {
                        for (int i = (Game1.player.currentLocation as MineShaft).resourceClumps.Count - 1; i >= 0; --i)
                        {
                            if ((Game1.player.currentLocation as MineShaft).resourceClumps[i].getBoundingBox((Game1.player.currentLocation as MineShaft).resourceClumps[i].tile.Value).Contains(ToolLocationPoint))
                            {
                                SetTool(typeof(Pickaxe));
                            }
                        }
                    }
                }
            }
        }
        
        private void SetTool(Type t)
        {
            for (int i = 0; i < 12; i++)
            {
                if (Game1.player.Items[i]!=null && Game1.player.Items[i].GetType() == t)
                {
                    Game1.player.CurrentToolIndex = i;
                    return;
                }
            }
        }

        private void SetScythe()
        {
            for (int i = 0; i < 12; i++)
            {
                if (Game1.player.Items[i] != null && Game1.player.Items[i].Name.Equals("Scythe"))
                {
                    Game1.player.CurrentToolIndex = i;
                    return;
                }
            }
            for (int i = 0; i < 12; i++)
            {
                if (Game1.player.Items[i] is MeleeWeapon)
                {
                    Game1.player.CurrentToolIndex = i;
                    return;
                }
            }
        }

        private void SetWeapon()
        {
            for (int i = 0; i < 12; i++)
            {
                if (Game1.player.Items[i] is MeleeWeapon && !Game1.player.Items[i].Name.Equals("Scythe"))
                {
                    Game1.player.CurrentToolIndex = i;
                    return;
                }
                    
            }
            for (int i = 0; i < 12; i++)
            {
                if (Game1.player.Items[i] != null && Game1.player.Items[i].Name.Equals("Scythe"))
                {
                    Game1.player.CurrentToolIndex = i;
                    return;
                }
            }
        }
    }
}
