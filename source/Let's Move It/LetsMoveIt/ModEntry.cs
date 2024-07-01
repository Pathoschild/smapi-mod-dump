/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Exblosis/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using LetsMoveIt.TargetData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace LetsMoveIt
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        private static ModConfig Config = null!;

        private static Dictionary<Vector2, Target> SelectedTargets = [];
        private static Target? OneTarget;

        private static bool Select = false;
        private static Vector2 StartCursorTile;
        private static Rectangle SelectedArea;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            I18n.Init(helper.Translation);
            Target.Init(Config, Helper, Monitor);

            if (!Config.ModEnabled)
                return;
            
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.Display.RenderingHud += OnRenderingHud;
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Input.ButtonReleased += OnButtonReleased;
        }
        private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if (!Config.ModEnabled)
            {
                SelectedTargets.Clear();
                OneTarget = null;
                return;
            }
            if (Helper.Input.IsDown(Config.ModKey) && Select && Config.MultiSelect)
            {
                Game1.player.canOnlyWalk = true;
                SelectedArea = new Rectangle((int)Math.Min(Game1.currentCursorTile.X, StartCursorTile.X), (int)Math.Min(Game1.currentCursorTile.Y, StartCursorTile.Y), (int)Math.Abs(Game1.currentCursorTile.X - StartCursorTile.X) + 1, (int)Math.Abs(Game1.currentCursorTile.Y - StartCursorTile.Y) + 1);
                for (int x_offset = 0; x_offset < SelectedArea.Width; x_offset++)
                {
                    for (int y_offset = 0; y_offset < SelectedArea.Height; y_offset++)
                    {
                        e.SpriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2(SelectedArea.X, SelectedArea.Y) * 64 + new Vector2(x_offset, y_offset) * 64), new Rectangle?(new Rectangle(194, 388, 16, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                    }
                }
            }
            if (OneTarget is not null)
            {
                if (OneTarget.TargetObject is null)
                {
                    OneTarget = null;
                    return;
                }
                OneTarget.Render(e.SpriteBatch, Game1.currentLocation, Game1.currentCursorTile);
            }
            if (SelectedTargets.Count > 0)
            {
                foreach (Target t in SelectedTargets.Values)
                {
                    if (t.TargetObject is null)
                    {
                        SelectedTargets.Remove(t.TilePosition);
                        return;
                    }
                    t.Render(e.SpriteBatch, Game1.currentLocation, Game1.currentCursorTile + (t.TilePosition - new Vector2(SelectedArea.X, SelectedArea.Y)));
                }
            }
        }

        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            if (OneTarget is null)
                return;
            string toolbarMessage = Config.CancelKey + " " + I18n.Message("Info.Cancel") + " | " + Config.OverwriteKey + " " + I18n.Message("Info.Force") + " | " + Config.RemoveKey + " " + I18n.Message("Info.Remove");
            Vector2 bounds = Game1.smallFont.MeasureString(toolbarMessage);
            Vector2 msgPosition = new(Game1.uiViewport.Width / 2 - bounds.X / 2, Game1.uiViewport.Height - 140);
            Utility.drawTextWithColoredShadow(e.SpriteBatch, toolbarMessage, Game1.smallFont, msgPosition, Color.White, Color.Black);
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Config.ModEnabled || !Context.IsPlayerFree && Game1.activeClickableMenu is not CarpenterMenu)
                return;
            if (Config.ToggleCopyModeKey != SButton.None && e.Button == Config.ToggleCopyModeKey)
            {
                Config.CopyMode = !Config.CopyMode;
                if (Config.CopyMode)
                {
                    Game1.playSound("drumkit6");
                }
                else
                {
                    Game1.playSound("drumkit6", 200);
                }
            }
            if (Config.ToggleMultiSelectKey != SButton.None && e.Button == Config.ToggleMultiSelectKey)
            {
                Config.MultiSelect = !Config.MultiSelect;
                if (Config.MultiSelect)
                {
                    Game1.playSound("drumkit6");
                }
                else
                {
                    Game1.playSound("drumkit6", 200);
                }
            }
            if (e.Button == Config.CancelKey && OneTarget is not null || e.Button == Config.CancelKey && SelectedTargets.Count > 0)
            {
                PlaySound();
                SelectedTargets.Clear();
                OneTarget = null;
                Helper.Input.Suppress(e.Button);
                return;
            }
            if (e.Button == Config.RemoveKey && OneTarget is not null || e.Button == Config.RemoveKey && SelectedTargets.Count > 0)
            {
                string select = I18n.Dialogue("Remove.Select1");
                if (OneTarget?.TargetObject is Character)
                    select = I18n.Dialogue("Remove.Select2");
                if (OneTarget?.TargetObject is Building)
                    select = I18n.Dialogue("Remove.Select3");
                Game1.player.currentLocation.createQuestionDialogue(I18n.Dialogue("Remove", new { select }), Mod1.YesNoResponses(), (Farmer f, string response) =>
                {
                    if (response == "Yes")
                    {
                        OneTarget?.Remove();
                        foreach (Target t in SelectedTargets.Values)
                        {
                            t.Remove();
                        }
                        Game1.playSound("trashcan");
                    }
                });
                Helper.Input.Suppress(e.Button);
                return;
            }
            if (e.Button == Config.MoveKey)
            {
                if (Config.ModKey == SButton.None)
                    return;
                if (Helper.Input.IsDown(Config.ModKey))
                {
                    Game1.player.canOnlyWalk = true;
                    SelectedTargets.Clear();
                    OneTarget = null;
                    if (Config.MultiSelect)
                    {
                        Select = true;
                        StartCursorTile = Game1.currentCursorTile;
                        return;
                    }
                    else
                    {
                        Helper.Input.Suppress(Config.MoveKey);
                        OneTarget = new Target(Game1.currentLocation, Game1.currentCursorTile, Mod1.GetGlobalMousePosition());
                        if (OneTarget.TargetObject is null)
                        {
                            OneTarget = null;
                            return;
                        }
                        PlaySound();
                        return;
                    }
                }
                if (OneTarget is not null)
                {
                    Helper.Input.Suppress(Config.MoveKey);
                    bool overwriteTile = Helper.Input.IsDown(Config.OverwriteKey);
                    if (OneTarget.IsOccupied(Game1.currentLocation, Game1.currentCursorTile) && !overwriteTile)
                    {
                        Game1.playSound("cancel");
                        return;
                    }
                    if (Config.CopyMode)
                    {
                        OneTarget.CopyTo(Game1.currentLocation, Game1.currentCursorTile, overwriteTile);
                    }
                    else
                    {
                        OneTarget.MoveTo(Game1.currentLocation, Game1.currentCursorTile, overwriteTile);
                        PlaySound();
                    }
                }
                if (SelectedTargets.Count > 0)
                {
                    Helper.Input.Suppress(Config.MoveKey);
                    bool overwriteTile = Helper.Input.IsDown(Config.OverwriteKey);
                    foreach (Target t in SelectedTargets.Values)
                    {
                        if (t.TargetObject is not null)
                        {
                            if (t.IsOccupied(Game1.currentLocation, Game1.currentCursorTile + (t.TilePosition - new Vector2(SelectedArea.X, SelectedArea.Y))) && !overwriteTile)
                            {
                                continue;
                            }
                            if (Config.CopyMode)
                            {
                                t.CopyTo(Game1.currentLocation, Game1.currentCursorTile + (t.TilePosition - new Vector2(SelectedArea.X, SelectedArea.Y)), overwriteTile);
                            }
                            else
                            {
                                t.MoveTo(Game1.currentLocation, Game1.currentCursorTile + (t.TilePosition - new Vector2(SelectedArea.X, SelectedArea.Y)), overwriteTile);
                            }
                        }
                    }
                    PlaySound();
                }
            }
        }

        private void OnButtonReleased(object? sender, ButtonReleasedEventArgs e)
        {
            if (e.Button == Config.MoveKey)
            {
                Select = false;
                if (Helper.Input.IsDown(Config.ModKey) && Config.MultiSelect)
                {
                    for (int x = 0; x < SelectedArea.Width; x++)
                    {
                        for (int y = 0; y < SelectedArea.Height; y++)
                        {
                            int xTile = SelectedArea.X + x;
                            int yTile = SelectedArea.Y + y;
                            Vector2 tile = new(xTile, yTile);
                            Target currentTarget = new(Game1.currentLocation, tile, Game1.GlobalToLocal(tile).ToPoint());
                            if (currentTarget.TargetObject is not null)
                            {
                                SelectedTargets.TryAdd(currentTarget.TilePosition, currentTarget);
                            }
                        }
                    }
                    if (SelectedTargets.Count > 0)
                    {
                        Vector2 min = new(SelectedTargets.Keys.Min(x => x.X), SelectedTargets.Keys.Min(y => y.Y));
                        Vector2 max = new(SelectedTargets.Keys.Max(x => x.X), SelectedTargets.Keys.Max(y => y.Y));
                        SelectedArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
                        PlaySound();
                    }
                }
            }
        }
        public static void PlaySound()
        {
            if (!string.IsNullOrEmpty(Config.Sound))
                Game1.playSound(Config.Sound);
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (Game1.activeClickableMenu is null)
                return;
            if (Game1.activeClickableMenu is DialogueBox)
                return;
            SelectedTargets.Clear();
            OneTarget = null;
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            SelectedTargets.Clear();
            OneTarget = null;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );
            // Config
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("ModEnabled"),
                tooltip: () => I18n.Config("ModEnabled.Tooltip"),
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => I18n.Config("ModKey"),
                getValue: () => Config.ModKey,
                setValue: value => Config.ModKey = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => I18n.Config("MoveKey"),
                getValue: () => Config.MoveKey,
                setValue: value => Config.MoveKey = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => I18n.Config("OverwriteKey"),
                tooltip: () => I18n.Config("OverwriteKey.Tooltip"),
                getValue: () => Config.OverwriteKey,
                setValue: value => Config.OverwriteKey = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => I18n.Config("CancelKey"),
                getValue: () => Config.CancelKey,
                setValue: value => Config.CancelKey = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => I18n.Config("RemoveKey"),
                getValue: () => Config.RemoveKey,
                setValue: value => Config.RemoveKey = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => I18n.Config("ToggleCopyModeKey"),
                getValue: () => Config.ToggleCopyModeKey,
                setValue: value => Config.ToggleCopyModeKey = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("CopyMode"),
                getValue: () => Config.CopyMode,
                setValue: value => Config.CopyMode = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => I18n.Config("ToggleMultiSelectKey"),
                getValue: () => Config.ToggleMultiSelectKey,
                setValue: value => Config.ToggleMultiSelectKey = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("MultiSelect"),
                tooltip: () => I18n.Config("MultiSelect.Tooltip"),
                getValue: () => Config.MultiSelect,
                setValue: value => Config.MultiSelect = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => I18n.Config("Sound"),
                getValue: () => Config.Sound,
                setValue: value => Config.Sound = value
            );
            // Prioritize Crops
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => I18n.Config("PrioritizeCrops")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("MoveCropWithoutTile"),
                getValue: () => Config.MoveCropWithoutTile,
                setValue: value => Config.MoveCropWithoutTile = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("MoveCropWithoutIndoorPot"),
                getValue: () => Config.MoveCropWithoutIndoorPot,
                setValue: value => Config.MoveCropWithoutIndoorPot = value
            );
            //configMenu.AddParagraph(
            //    mod: ModManifest,
            //    text: () => I18n.Config("IndoorPot.Note")
            //);
            // Enable & Disable Components Page
            configMenu.AddPageLink(
                mod: ModManifest,
                pageId: "Components",
                text: () => I18n.Config("Page.Components.Link"),
                tooltip: () => I18n.Config("Page.Components.Link.Tooltip")
            );
            configMenu.AddPage(
                mod: ModManifest,
                pageId: "Components",
                pageTitle: () => I18n.Config("Page.Components.Title")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveBuilding"),
                getValue: () => Config.EnableMoveBuilding,
                setValue: value => Config.EnableMoveBuilding = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveEntity"),
                tooltip: () => I18n.Config("EnableMoveEntity.Tooltip"),
                getValue: () => Config.EnableMoveEntity,
                setValue: value => Config.EnableMoveEntity = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveCrop"),
                getValue: () => Config.EnableMoveCrop,
                setValue: value => Config.EnableMoveCrop = value
            );

            configMenu.AddParagraph(
                mod: ModManifest,
                text: () => "________________" // SPACE
            );
            // Objects
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveObject"),
                getValue: () => Config.EnableMoveObject,
                setValue: value => Config.EnableMoveObject = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMovePlaceableObject"),
                tooltip: () => I18n.Config("EnableMovePlaceableObject.Tooltip"),
                getValue: () => Config.EnableMovePlaceableObject,
                setValue: value => Config.EnableMovePlaceableObject = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveCollectibleObject"),
                tooltip: () => I18n.Config("EnableMoveCollectibleObject.Tooltip"),
                getValue: () => Config.EnableMoveCollectibleObject,
                setValue: value => Config.EnableMoveCollectibleObject = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveGeneratedObject"),
                tooltip: () => I18n.Config("EnableMoveGeneratedObject.Tooltip"),
                getValue: () => Config.EnableMoveGeneratedObject,
                setValue: value => Config.EnableMoveGeneratedObject = value
            );

            configMenu.AddParagraph(
                mod: ModManifest,
                text: () => "________________" // SPACE
            );
            // Resource Clumps
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveResourceClump"),
                getValue: () => Config.EnableMoveResourceClump,
                setValue: value => Config.EnableMoveResourceClump = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveGiantCrop"),
                getValue: () => Config.EnableMoveGiantCrop,
                setValue: value => Config.EnableMoveGiantCrop = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveStump"),
                getValue: () => Config.EnableMoveStump,
                setValue: value => Config.EnableMoveStump = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveHollowLog"),
                getValue: () => Config.EnableMoveHollowLog,
                setValue: value => Config.EnableMoveHollowLog = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveBoulder"),
                getValue: () => Config.EnableMoveBoulder,
                setValue: value => Config.EnableMoveBoulder = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveMeteorite"),
                getValue: () => Config.EnableMoveMeteorite,
                setValue: value => Config.EnableMoveMeteorite = value
            );

            configMenu.AddParagraph(
                mod: ModManifest,
                text: () => "________________" // SPACE
            );
            // Terrain Features
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveTerrainFeature"),
                getValue: () => Config.EnableMoveTerrainFeature,
                setValue: value => Config.EnableMoveTerrainFeature = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveFlooring"),
                getValue: () => Config.EnableMoveFlooring,
                setValue: value => Config.EnableMoveFlooring = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveTree"),
                getValue: () => Config.EnableMoveTree,
                setValue: value => Config.EnableMoveTree = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveFruitTree"),
                getValue: () => Config.EnableMoveFruitTree,
                setValue: value => Config.EnableMoveFruitTree = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveGrass"),
                getValue: () => Config.EnableMoveGrass,
                setValue: value => Config.EnableMoveGrass = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveFarmland"),
                getValue: () => Config.EnableMoveFarmland,
                setValue: value => Config.EnableMoveFarmland = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config("EnableMoveBush"),
                getValue: () => Config.EnableMoveBush,
                setValue: value => Config.EnableMoveBush = value
            );
        }
    }
}
