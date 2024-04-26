/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Exblosis/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace LetsMoveIt
{
    /// <summary>The mod entry point.</summary>
    internal partial class ModEntry : Mod
    {

        private static IMonitor SMonitor;
        private static ModConfig Config;

        private static object MovingObject;
        private static Vector2 MovingTile;
        private static GameLocation MovingLocation;
        private static Vector2 MovingOffset;
        private static readonly HashSet<Vector2> BoundingBoxTile = new();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            if (!Config.ModEnabled)
                return;

            SMonitor = this.Monitor;

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }
        private void OnRenderedWorld(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            if (!Config.ModEnabled)
            {
                MovingObject = null;
                return;
            }
            if (MovingObject is null)
                return;
            try
            {
                BoundingBoxTile.Clear();
                if (MovingObject is ResourceClump)
                {
                    if(MovingObject is GiantCrop)
                    {
                        var gc = (MovingObject as GiantCrop).getBoundingBox();
                        for (int x_offset = 0; x_offset < gc.Width / 64; x_offset++)
                        {
                            for (int y_offset = 0; y_offset < gc.Height / 64; y_offset++)
                            {
                                e.SpriteBatch.Draw(Game1.mouseCursors, GetGridPosition(x_offset * 64, y_offset * 64), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(194, 388, 16, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                            }
                        }
                        string name = (MovingObject as ResourceClump).NetFields.GetFields().ToList()[7].ToString();
                        //SMonitor.Log("Name: " + name, LogLevel.Info); // <<< debug >>>
                        if (name is "Cauliflower")
                        {
                            e.SpriteBatch.Draw(Game1.cropSpriteSheet, GetGridPosition(yOffset: -40), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(112, 518, 48, 58)), Color.White * 0.6f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                        }
                        else if (name is "Melon")
                        {
                            e.SpriteBatch.Draw(Game1.cropSpriteSheet, GetGridPosition(yOffset: -40), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(160, 518, 48, 56)), Color.White * 0.6f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                        }
                        else if (name is "Pumpkin")
                        {
                            e.SpriteBatch.Draw(Game1.cropSpriteSheet, GetGridPosition(yOffset: -40), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(208, 518, 48, 54)), Color.White * 0.6f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                        }
                        else if (name is "Powdermelon")
                        {
                            e.SpriteBatch.Draw(Game1.mouseCursors_1_6, GetGridPosition(yOffset: -52), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(320, 451, 48, 61)), Color.White * 0.6f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                        }
                        else if (name is "QiFruit")
                        {
                            e.SpriteBatch.Draw(Game1.mouseCursors_1_6, GetGridPosition(yOffset: -40), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(368, 454, 48, 58)), Color.White * 0.6f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                        }
                    }
                    else
                    {
                        Rectangle sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (MovingObject as ResourceClump).parentSheetIndex.Value, 16, 16);
                        sourceRect.Width = (MovingObject as ResourceClump).width.Value * 16;
                        sourceRect.Height = (MovingObject as ResourceClump).height.Value * 16;
                        e.SpriteBatch.Draw(Game1.objectSpriteSheet, GetGridPosition(), sourceRect, Color.White * 0.6f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                    }
                    
                    var rc = (MovingObject as ResourceClump).getBoundingBox();
                    for (int x_offset = 0; x_offset < rc.Width / 64; x_offset++)
                    {
                        for (int y_offset = 0; y_offset < rc.Height / 64; y_offset++)
                        {
                            BoundingBoxTile.Add(Game1.currentCursorTile + new Vector2(x_offset, y_offset));
                        }
                    }
                }
                else if (MovingObject is TerrainFeature)
                {
                    //SMonitor.Log("TerrainFeature", LogLevel.Info); // <<< debug >>>
                    var tf = (MovingObject as TerrainFeature).getBoundingBox();
                    for (int x_offset = 0; x_offset < tf.Width / 64; x_offset++)
                    {
                        BoundingBoxTile.Add(Game1.currentCursorTile + new Vector2(x_offset, 0));
                        e.SpriteBatch.Draw(Game1.mouseCursors, GetGridPosition(x_offset * 64), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(194, 388, 16, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                    }
                }
                else if (MovingObject is Crop)
                {
                    //SMonitor.Log("Crop", LogLevel.Info); // <<< debug >>>
                    (MovingObject as Crop).drawWithOffset(e.SpriteBatch, Game1.currentCursorTile, Color.White * 0.6f, 0f, new Vector2(32));
                }
                else if (MovingObject is Object)
                {
                    //SMonitor.Log("Object", LogLevel.Info); // <<< debug >>>
                    (MovingObject as Object).draw(e.SpriteBatch, (int)Game1.currentCursorTile.X * 64, (int)Game1.currentCursorTile.Y * 64 - ((MovingObject as Object).bigCraftable.Value ? 64 : 0), 1, 0.6f);
                }
                else if (MovingObject is Character)
                {
                    Rectangle box = (MovingObject as Character).GetBoundingBox();
                    (MovingObject as Character).Sprite.draw(e.SpriteBatch, new Vector2(Game1.getMouseX() - 32, Game1.getMouseY() - 32) + new Vector2((float)((MovingObject as Character).GetSpriteWidthForPositioning() * 4 / 2), (float)(box.Height / 2)), (float)box.Center.Y / 10000f, 0, (MovingObject as Character).ySourceRectOffset, Color.White, false, 4f, 0f, true);
                }
                else if (MovingObject is Building)
                {
                    var building = (MovingObject as Building);
                    float x = Game1.currentCursorTile.X - MovingOffset.X / 64;
                    float y = Game1.currentCursorTile.Y - MovingOffset.Y / 64;
                    
                    for (int x_offset = 0; x_offset < building.tilesWide.Value; x_offset++)
                    {
                        for (int y_offset = 0; y_offset < building.tilesHigh.Value; y_offset++)
                        {
                            e.SpriteBatch.Draw(Game1.mouseCursors, new Vector2((x + x_offset) * 64 - Game1.viewport.X, (y + y_offset) * 64 - Game1.viewport.Y), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(194, 388, 16, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
                            //e.SpriteBatch.Draw(Game1.mouseCursors, GetGridPosition(x_offset * 64, y_offset * 64), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(194, 388, 16, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
                        }
                    }
                    //e.SpriteBatch.Draw(building.texture.Value, new Vector2(Game1.getMouseX() - movingOffset.X, Game1.getMouseY() + building.tilesHigh.Value * 64 - movingOffset.Y), new Rectangle?(building.getSourceRect()), building.color.Value, 0f, new Vector2(0f, (float)building.getSourceRect().Height), 4f, SpriteEffects.None, 1);
                }
            }
            catch { }
        }

        private static Vector2 GetGridPosition(int xOffset = 0, int yOffset = 0)
        {
            return Game1.GlobalToLocal(Game1.viewport, new Vector2(xOffset, yOffset) + new Vector2(Game1.currentCursorTile.X * 64f, Game1.currentCursorTile.Y * 64f));
        }

        private void OnButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (!Config.ModEnabled || !Context.IsPlayerFree && Game1.activeClickableMenu is not CarpenterMenu)
                return;
            if(e.Button == Config.CancelKey && MovingObject is not null)
            {
                PlaySound();
                MovingObject = null;
                this.Helper.Input.Suppress(e.Button);
                return;
            }
            if (e.Button == Config.MoveKey)
            {
                // GameLocation location = Game1.currentLocation;
                this.PickupObject(Game1.currentLocation);
            }
        }

        private void OnMenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            MovingObject = null;
        }

        //private void Player_Warped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        //{
        //    if (e.Player.IsMainPlayer)
        //        movingObject = null;
        //}

        private void OnSaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            MovingObject = null;
        }

        private void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Mod Enabled",
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Move crops separately from farmland",
                getValue: () => Config.MoveCropWithoutTile,
                setValue: value => Config.MoveCropWithoutTile = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Move Buildings",
                getValue: () => Config.MoveBuilding,
                setValue: value => Config.MoveBuilding = value
            );
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Mod Key",
                getValue: () => Config.ModKey,
                setValue: value => Config.ModKey = value
            );
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Move Key",
                getValue: () => Config.MoveKey,
                setValue: value => Config.MoveKey = value
            );
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Overwrite Key (Warning: can delete something)",
                getValue: () => Config.OverwriteKey,
                setValue: value => Config.OverwriteKey = value
            );
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Cancel Movieng",
                getValue: () => Config.CancelKey,
                setValue: value => Config.CancelKey = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Sound",
                getValue: () => Config.Sound,
                setValue: value => Config.Sound = value
            );
        }

    }
}
