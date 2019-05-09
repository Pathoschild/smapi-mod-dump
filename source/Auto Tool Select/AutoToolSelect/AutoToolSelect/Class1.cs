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
        public bool HoeSelect { get; set; } = true;
    }

    class AutoToolSelectMod : Mod
    {
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            this.Config = (ModConfig)helper.ReadConfig<ModConfig>();
            Helper.Events.Input.ButtonPressed += new EventHandler<ButtonPressedEventArgs>(this.ButtonPressed);
            Helper.Events.Input.ButtonReleased += new EventHandler<ButtonReleasedEventArgs>(this.ButtonReleased);
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == this.Config.ActivationKey)
            {
                Helper.Events.GameLoop.UpdateTicked += this.GameTicked;
            }
        }

        private void ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button == this.Config.ActivationKey)
            {
                Helper.Events.GameLoop.UpdateTicked -= this.GameTicked;
            }
        }
        
        private void GameTicked(object sender, EventArgs e)
        {
            if (Context.IsWorldReady)
            {
                Vector2 ToolLocationVector = new Vector2((int)Game1.player.GetToolLocation(false).X / Game1.tileSize, (int)Game1.player.GetToolLocation(false).Y / Game1.tileSize);
                Point ToolLocationPoint = new Point(((int)Game1.player.GetToolLocation(false).X / Game1.tileSize) * Game1.tileSize + Game1.tileSize / 2, ((int)Game1.player.GetToolLocation(false).Y / Game1.tileSize) * Game1.tileSize + Game1.tileSize / 2);
                if (Game1.player.currentLocation.doesTileHaveProperty((int)ToolLocationVector.X, (int)ToolLocationVector.Y, "Water", "Back") != null || Game1.player.currentLocation.doesTileHaveProperty((int)ToolLocationVector.X, (int)ToolLocationVector.Y, "WaterSource", "Back") != null || Game1.player.currentLocation is BuildableGameLocation && (Game1.player.currentLocation as BuildableGameLocation).getBuildingAt(ToolLocationVector) != null && ((Game1.player.currentLocation as BuildableGameLocation).getBuildingAt(ToolLocationVector).buildingType.Equals("Well") && (Game1.player.currentLocation as BuildableGameLocation).getBuildingAt(ToolLocationVector).daysOfConstructionLeft.Value <= 0))
                {
                    for (int j = 0; j < 12; j++)
                    {
                        if (Game1.player.Items[j] is WateringCan)
                        {
                            Game1.player.CurrentToolIndex = j;
                            j = 12;
                        }
                    }
                }
                if (Game1.player.currentLocation.doesTileHaveProperty((int)ToolLocationVector.X, (int)ToolLocationVector.Y, "Diggable", "Back") != null && this.Config.HoeSelect)
                {

                    for (int j = 0; j < 12; j++)
                    {
                        if (Game1.player.Items[j] is Hoe)
                        {
                            Game1.player.CurrentToolIndex = j;
                            j = 12;
                        }
                    }
                }
                if (Game1.player.currentLocation is Farm)
                {
                    for (int i = (Game1.player.currentLocation as Farm).resourceClumps.Count - 1; i >= 0; --i)
                    {
                        if ((Game1.player.currentLocation as Farm).resourceClumps[i].getBoundingBox((Game1.player.currentLocation as Farm).resourceClumps[i].tile.Value).Contains(ToolLocationPoint))
                        {
                            switch ((Game1.player.currentLocation as Farm).resourceClumps[i].parentSheetIndex.Value)
                            {
                                case 622:
                                case 672:
                                case 752:
                                case 754:
                                case 756:
                                case 758:
                                    for (int j = 0; j < 12; j++)
                                    {
                                        if (Game1.player.Items[j] is Pickaxe)
                                        {
                                            Game1.player.CurrentToolIndex = j;
                                            j = 12;
                                        }
                                    }
                                    break;
                                case 600:
                                case 602:
                                    for (int j = 0; j < 12; j++)
                                    {
                                        if (Game1.player.Items[j] is Axe)
                                        {
                                            Game1.player.CurrentToolIndex = j;
                                            j = 12;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
                if (Game1.player.currentLocation is Woods)
                {
                    for (int i = (Game1.player.currentLocation as Woods).stumps.Count - 1; i >= 0; --i)
                    {
                        if ((Game1.player.currentLocation as Woods).stumps[i].getBoundingBox((Game1.player.currentLocation as Woods).stumps[i].tile.Value).Contains(ToolLocationPoint))
                        {
                            for (int j = 0; j < 12; j++)
                            {
                                if (Game1.player.Items[j] is Axe)
                                {
                                    Game1.player.CurrentToolIndex = j;
                                    j = 12;
                                }
                            }
                        }
                    }
                }
                if (Game1.player.currentLocation is Forest && (Game1.player.currentLocation as Forest).log.getBoundingBox((Game1.player.currentLocation as Forest).log.tile.Value).Contains(ToolLocationPoint))
                {
                    for (int j = 0; j < 12; j++)
                    {
                        if (Game1.player.Items[j] is Axe)
                        {
                            Game1.player.CurrentToolIndex = j;
                            j = 12;
                        }
                    }
                }
                if (Game1.player.currentLocation is MineShaft)
                {
                    for (int i = (Game1.player.currentLocation as MineShaft).resourceClumps.Count - 1; i >= 0; --i)
                    {
                        if ((Game1.player.currentLocation as MineShaft).resourceClumps[i].getBoundingBox((Game1.player.currentLocation as MineShaft).resourceClumps[i].tile.Value).Contains(ToolLocationPoint))
                        {
                            for (int j = 0; j < 12; j++)
                            {
                                if (Game1.player.Items[j] is Pickaxe)
                                {
                                    Game1.player.CurrentToolIndex = j;
                                    j = 12;
                                }
                            }
                        }
                    }
                }
                if (Game1.player.currentLocation.objects.ContainsKey(ToolLocationVector))
                {
                    if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Stone"))
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if (Game1.player.Items[j] is Pickaxe)
                            {
                                Game1.player.CurrentToolIndex = j;
                                j = 12;
                            }
                        }
                    }
                    if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Twig"))
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if (Game1.player.Items[j] is Axe)
                            {
                                Game1.player.CurrentToolIndex = j;
                                j = 12;
                            }
                        }
                    }
                    if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Weeds"))
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if (Game1.player.Items[j] is MeleeWeapon)
                            {
                                Game1.player.CurrentToolIndex = j;
                                j = 12;
                            }
                        }
                    }
                    if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Barrel"))
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if (Game1.player.Items[j] is MeleeWeapon)
                            {
                                Game1.player.CurrentToolIndex = j;
                                j = 12;
                            }
                        }
                    }
                    if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Artifact Spot"))
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if (Game1.player.Items[j] is Hoe)
                            {
                                Game1.player.CurrentToolIndex = j;
                                j = 12;
                            }
                        }
                    }
                }
                if (Game1.player.currentLocation.terrainFeatures.ContainsKey(ToolLocationVector))
                {
                    if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is Tree)
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if (Game1.player.Items[j] is Axe)
                            {
                                Game1.player.CurrentToolIndex = j;
                                j = 12;
                            }
                        }
                    }
                    if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is GiantCrop)
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if (Game1.player.Items[j] is Axe)
                            {
                                Game1.player.CurrentToolIndex = j;
                                j = 12;
                            }
                        }
                    }
                    if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is Grass)
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if (Game1.player.Items[j] != null && Game1.player.Items[j].Name.Equals("Scythe"))
                            {
                                Game1.player.CurrentToolIndex = j;
                                j = 12;
                            }
                            if (j == 11)
                            {
                                for (int k = 0; k < 12; k++)
                                {
                                    if (Game1.player.Items[k] is MeleeWeapon)
                                    {
                                        Game1.player.CurrentToolIndex = k;
                                        k = 12;
                                    }
                                }
                            }
                        }
                    }
                    if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is HoeDirt)
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if (Game1.player.Items[j] is WateringCan)
                            {
                                Game1.player.CurrentToolIndex = j;
                                j = 12;
                            }
                        }
                    }
                }
            }
        }  
    }
}
