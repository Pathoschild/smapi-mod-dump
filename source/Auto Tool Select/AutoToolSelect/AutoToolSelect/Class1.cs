/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MaciejMarkuszewski/StardewValleyAutoToolSelect
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Locations;
using System;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Monsters;

namespace AutoToolSelect
{
    public class ModConfig
    {
        public SButton ActivationKey { get; set; } = SButton.LeftControl;
        public SButton ToggleKey { get; set; } = SButton.F5;
        public bool HoeSelect { get; set; } = true;
        public bool IfNoneToolChooseWeapon { get; set; } = true;
        public bool RideHorseCursor { get; set; } = true;
        public bool PickaxeOverWateringCan { get; set; } = true;
        public bool CursorOverToolHitLocation { get; set; } = false;
    }

    public interface IGenericModConfigMenuAPI
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddKeybind(IManifest mod, Func<SButton> getValue, Action<SButton> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }

    class AutoToolSelectMod : Mod
    {
        bool togglemod = false;
        bool buttonPressed = false;
        private ModConfig Config;
        Texture2D Lock;
        public override void Entry(IModHelper helper)
        {
            Lock = helper.ModContent.Load<Texture2D>("assets/Lock.png");
            Config = (ModConfig)helper.ReadConfig<ModConfig>();
            Helper.Events.Input.ButtonPressed += new EventHandler<ButtonPressedEventArgs>(ButtonPressed);
            Helper.Events.Input.ButtonReleased += new EventHandler<ButtonReleasedEventArgs>(ButtonReleased);
            Helper.Events.Display.RenderedHud += PostRenderHud;
            Helper.Events.GameLoop.GameLaunched += OnLaunched;
        }
        private void OnLaunched(object sender, GameLaunchedEventArgs e)
        {
            Config = Helper.ReadConfig<ModConfig>();
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            api.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            api.AddKeybind(
                mod: this.ModManifest,
                name: () => "Activation Key",
                tooltip: () => "When hold, mod is activated",
                getValue: () => this.Config.ActivationKey,
                setValue: value => this.Config.ActivationKey = value
            );

            api.AddKeybind(
                mod: this.ModManifest,
                name: () => "Toggle Key",
                tooltip: () => "Press to turn on/off mod",
                getValue: () => this.Config.ToggleKey,
                setValue: value => this.Config.ToggleKey = value
            );

            api.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Hoe Select",
                tooltip: () => "Mod will select hoe when you target ground. I recommend not checking it",
                getValue: () => this.Config.HoeSelect,
                setValue: value => this.Config.HoeSelect = value
            );

            api.AddBoolOption(
                mod: this.ModManifest,
                name: () => "If None Tool Choose Weapon",
                tooltip: () => "Mod will select weapon/scythe when none tool was chosen. Works better with Hoe Select not checked",
                getValue: () => this.Config.IfNoneToolChooseWeapon,
                setValue: value => this.Config.IfNoneToolChooseWeapon = value
            );

            api.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Ride Horse Cursor",
                tooltip: () => "Mod will use cursor location to determine which tool should be chosen while riding horse (tractor) - for mouse and keyboard. If not checked mod will use tool hit location to - for controller",
                getValue: () => this.Config.RideHorseCursor,
                setValue: value => this.Config.RideHorseCursor = value
            );

            api.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Pickaxe Over Watering Can",
                tooltip: () => "Mod will select pickaxe on hoed dirt, instead of watering can. Can be used to collect clay. Works better with Hoe Select checked. I recommend not checking it",
                getValue: () => this.Config.PickaxeOverWateringCan,
                setValue: value => this.Config.PickaxeOverWateringCan = value
            );

            api.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Cursor Over Tool Hit Location",
                tooltip: () => "Switches to use cursor location to determine which tool should be chosen. Set it true if you use Ranged Tools or Combat Controls mods",
                getValue: () => this.Config.CursorOverToolHitLocation,
                setValue: value => this.Config.CursorOverToolHitLocation = value
            );
        }

            private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == this.Config.ActivationKey && !togglemod)
            {
                Helper.Events.GameLoop.UpdateTicked += this.GameTicked;
                buttonPressed = true;
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
                buttonPressed = false;
            }
        }

        private void PostRenderHud(object sender, EventArgs e)
        {
            foreach (IClickableMenu menu in Game1.onScreenMenus)
            {
                if (menu is Toolbar && Game1.activeClickableMenu == null && (togglemod || buttonPressed))
                {
                    Vector2 position;
                    if (Game1.options.pinToolbarToggle || (double) Game1.GlobalToLocal(Game1.viewport,new Vector2((float) Game1.player.GetBoundingBox().Center.X, (float) Game1.player.GetBoundingBox().Center.Y)).Y <= (double) (Game1.viewport.Height / 2 +64))
                    {
                        position = new Vector2((float)(Game1.uiViewport.Width / 2 - 384 + Game1.player.CurrentToolIndex * 64), (float)(Game1.uiViewport.Height - Utility.makeSafeMarginY(8) - 80));
                    }
                    else
                    {
                        position = new Vector2((float)(Game1.uiViewport.Width / 2 - 384 + Game1.player.CurrentToolIndex * 64), (float)(16 + Utility.makeSafeMarginY(8)));
                    }
                    Game1.spriteBatch.Draw(Lock, position, Color.White * 1f);
                }
            }
        }
        
        private void GameTicked(object sender, EventArgs e)
        {
            if (Context.IsWorldReady && Context.CanPlayerMove)
            {
                Vector2 ToolLocationVector;
                if ((Game1.player.isRidingHorse() && Config.RideHorseCursor) || Config.CursorOverToolHitLocation)
                {
                    ToolLocationVector = Game1.currentCursorTile;
                }
                else
                {
                    ToolLocationVector = new Vector2((int)Game1.player.GetToolLocation().X / Game1.tileSize, (int)Game1.player.GetToolLocation().Y / Game1.tileSize);
                }
                Point ToolLocationPoint = new (((int)ToolLocationVector.X) * Game1.tileSize + Game1.tileSize / 2, ((int)ToolLocationVector.Y) * Game1.tileSize + Game1.tileSize / 2);
                Rectangle ToolRect = new (((int)ToolLocationVector.X) * Game1.tileSize, ((int)ToolLocationVector.Y) * Game1.tileSize, Game1.tileSize, Game1.tileSize);
                Rectangle PanRect = new (Game1.player.currentLocation.orePanPoint.X * 64 - 64, Game1.player.currentLocation.orePanPoint.Y * 64 - 64, 256, 256);
                if (this.Config.IfNoneToolChooseWeapon)
                {
                    SetWeapon();
                }
                if (Game1.player.currentLocation.doesTileHaveProperty((int)ToolLocationVector.X, (int)ToolLocationVector.Y, "Diggable", "Back") != null && this.Config.HoeSelect)
                {
                    SetTool(typeof(Hoe));
                }
                if (Game1.player.currentLocation.doesTileHaveProperty((int)ToolLocationVector.X, (int)ToolLocationVector.Y, "Water", "Back") != null && Game1.player.currentLocation is not VolcanoDungeon)
                {
                    SetTool(typeof(FishingRod));
                }
                if ((Game1.player.currentLocation is Farm || Game1.player.currentLocation.IsGreenhouse || (Game1.player.currentLocation is VolcanoDungeon && !(Game1.player.currentLocation as VolcanoDungeon).IsCooledLava((int)ToolLocationVector.X,(int)ToolLocationVector.Y))) && (Game1.player.currentLocation.getTileIndexAt((int)ToolLocationVector.X, (int)ToolLocationVector.Y, "Buildings") == 1938 || Game1.player.currentLocation.doesTileHaveProperty((int)ToolLocationVector.X, (int)ToolLocationVector.Y, "Water", "Back") != null || Game1.player.currentLocation.doesTileHaveProperty((int)ToolLocationVector.X, (int)ToolLocationVector.Y, "WaterSource", "Back") != null || Game1.player.currentLocation is BuildableGameLocation && (Game1.player.currentLocation as BuildableGameLocation).getBuildingAt(ToolLocationVector) != null && ((Game1.player.currentLocation as BuildableGameLocation).getBuildingAt(ToolLocationVector).buildingType.Equals("Well") && (Game1.player.currentLocation as BuildableGameLocation).getBuildingAt(ToolLocationVector).daysOfConstructionLeft.Value <= 0)))
                {
                    SetTool(typeof(WateringCan));
                }
                if (PanRect.Contains(ToolLocationPoint) && (double)Utility.distance((float)Game1.player.getStandingX(), (float)PanRect.Center.X, (float)Game1.player.getStandingY(), (float)PanRect.Center.Y) <= 192.0)
                {
                    SetTool(typeof(Pan));
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
                    if (Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Weeds") || Game1.player.currentLocation.objects[ToolLocationVector].name.Equals("Barrel"))
                    {
                        SetWeapon();
                    }
                }
                if (Game1.player.currentLocation.terrainFeatures.ContainsKey(ToolLocationVector))
                {
                    if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is HoeDirt)
                    {
                        if ((Game1.player.currentLocation is Farm || Game1.player.currentLocation.IsGreenhouse) && (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] as HoeDirt).crop != null && (((Game1.player.currentLocation.terrainFeatures[ToolLocationVector] as HoeDirt).crop.harvestMethod.Value==1 && (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] as HoeDirt).crop.fullyGrown.Value) || (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] as HoeDirt).crop.dead.Value))
                        {
                            SetScythe();
                        }
                        else
                        {
                            if (this.Config.PickaxeOverWateringCan)
                            {
                                SetTool(typeof(Pickaxe));
                            }
                            else if (Game1.player.currentLocation is Farm || Game1.player.currentLocation.IsGreenhouse)
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
                        SetWeapon();
                    }
                }
                if (Game1.player.currentLocation is Farm)
                {
                    foreach (FarmAnimal animal in (Game1.player.currentLocation as Farm).animals.Values)
                    {
                        if (animal.GetHarvestBoundingBox().Intersects(ToolRect) && animal.toolUsedForHarvest.Equals("Shears") && animal.currentProduce.Value > 0 && animal.age.Value >= animal.ageWhenMature.Value)
                        {
                            SetTool(typeof(Shears));
                        }
                        if (animal.GetHarvestBoundingBox().Intersects(ToolRect) && animal.toolUsedForHarvest.Equals("Milk Pail") && animal.currentProduce.Value > 0 && animal.age.Value >= animal.ageWhenMature.Value)
                        {
                            SetTool(typeof(MilkPail));
                        }
                    }
                }
                if (Game1.player.currentLocation is AnimalHouse)
                {
                    foreach (FarmAnimal animal in (Game1.player.currentLocation as AnimalHouse).animals.Values)
                    {
                        if (animal.GetHarvestBoundingBox().Intersects(ToolRect) && animal.toolUsedForHarvest.Equals("Shears") && animal.currentProduce.Value > 0 && animal.age.Value >= animal.ageWhenMature.Value)
                        {
                            SetTool(typeof(Shears));
                        }
                        if (animal.GetHarvestBoundingBox().Intersects(ToolRect) && animal.toolUsedForHarvest.Equals("Milk Pail") && animal.currentProduce.Value > 0 && animal.age.Value >= animal.ageWhenMature.Value)
                        {
                            SetTool(typeof(MilkPail));
                        }
                    }
                }
                if (Game1.player.currentLocation is Woods)
                {
                    for (int i = (Game1.player.currentLocation as Woods).stumps.Count - 1; i >= 0; --i)
                    {
                        if ((Game1.player.currentLocation as Woods).stumps[i].getBoundingBox((Game1.player.currentLocation as Woods).stumps[i].tile.Value).Contains(ToolLocationPoint))
                        {
                            SetTool(typeof(Axe), 1);
                        }
                    }
                }
                if (Game1.player.currentLocation is Forest && (Game1.player.currentLocation as Forest).log != null && (Game1.player.currentLocation as Forest).log.getBoundingBox((Game1.player.currentLocation as Forest).log.tile.Value).Contains(ToolLocationPoint))
                {
                    SetTool(typeof(Axe), 2);
                }
                if (Game1.player.currentLocation is Farm || Game1.player.currentLocation is MineShaft || Game1.player.currentLocation is IslandWest)
                {
                    for (int i = Game1.player.currentLocation.resourceClumps.Count - 1; i >= 0; --i)
                    {
                        if (Game1.player.currentLocation.resourceClumps[i].getBoundingBox(Game1.player.currentLocation.resourceClumps[i].tile.Value).Contains(ToolLocationPoint))
                        {
                            if (Game1.player.currentLocation.resourceClumps[i].parentSheetIndex.Value == 600)
                            {
                                SetTool(typeof(Axe), 1);
                            }
                            if (Game1.player.currentLocation.resourceClumps[i].parentSheetIndex.Value == 602)
                            {
                                SetTool(typeof(Axe), 2);
                            }
                            if (Game1.player.currentLocation.resourceClumps[i].parentSheetIndex.Value == 622)
                            {
                                SetTool(typeof(Pickaxe), 3);
                            }
                            if (Game1.player.currentLocation.resourceClumps[i].parentSheetIndex.Value == 672)
                            {
                                SetTool(typeof(Pickaxe), 2);
                            }
                            if (Game1.player.currentLocation.resourceClumps[i].parentSheetIndex.Value == 752 || Game1.player.currentLocation.resourceClumps[i].parentSheetIndex.Value == 754 || Game1.player.currentLocation.resourceClumps[i].parentSheetIndex.Value == 756 || Game1.player.currentLocation.resourceClumps[i].parentSheetIndex.Value == 758)
                            {
                                SetTool(typeof(Pickaxe));
                            }
                        }
                    }
                }
                if (Game1.player.currentLocation is MineShaft)
                {
                    foreach (Character monster in Game1.player.currentLocation.characters)
                    {
                        if (monster is Monster)
                        {
                            if ((monster is LavaCrab || monster is RockCrab) && !monster.Name.Equals("Stick Bug") && !monster.isMoving() && ToolRect.Contains(monster.Position))
                            {
                                SetTool(typeof(Pickaxe));
                            }
                            if (monster is RockCrab && monster.Name.Equals("Stick Bug") && !monster.isMoving() && ToolRect.Contains(monster.Position))
                            {
                                SetTool(typeof(Axe));
                            }
                        }
                    }
                }
            }
        }
        
        private static void SetTool(Type t, int Level = 0)
        {
            for (int i = 0; i < 12; i++)
            {
                if (Game1.player.Items[i]!=null && Game1.player.Items[i].GetType() == t && (Game1.player.Items[i] as Tool).UpgradeLevel>=Level)
                {
                    Game1.player.CurrentToolIndex = i;
                    return;
                }
            }
        }

        private static void SetScythe()
        {
            for (int i = 0; i < 12; i++)
            {
                if (Game1.player.Items[i] != null && Game1.player.Items[i].Name.Contains("Scythe"))
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

        private static void SetWeapon()
        {
            if (Game1.player.currentLocation is Farm || Game1.player.currentLocation.IsGreenhouse)
            {
                SetScythe();
                return;
            }
            for (int i = 0; i < 12; i++)
            {
                if (Game1.player.Items[i] is MeleeWeapon && !Game1.player.Items[i].Name.Contains("Scythe"))
                {
                    Game1.player.CurrentToolIndex = i;
                    return;
                }
                    
            }
            for (int i = 0; i < 12; i++)
            {
                if (Game1.player.Items[i] != null && Game1.player.Items[i].Name.Contains("Scythe"))
                {
                    Game1.player.CurrentToolIndex = i;
                    return;
                }
            }
        }
    }
}
