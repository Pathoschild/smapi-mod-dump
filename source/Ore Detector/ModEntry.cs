/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/1Avalon/Ore-Detector
**
*************************************************/

using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Menus;
using System.Reflection;

namespace OreDetector
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        /// 

        public static ModEntry? instance;

        private OreDetector? detector;

        private Texture2D? ladderTexture;

        private Texture2D? holeTexture;

        public static Texture2D? blackListButtonTexture;

        public static Texture2D? whiteListButtonTexture;

        private bool informationHidden = false;

        public static ModConfig? Config;

        public static SaveModel? saveModel;

        private Dictionary<string, Color> lineColors = new Dictionary<string, Color>()
        {
            ["Red"] = Color.Red,
            ["Blue"] = Color.Blue,
            ["Green"] = Color.Green,
            ["Yellow"] = Color.Yellow,
        };


        public override void Entry(IModHelper helper)
        {
            I18n.Init(Helper.Translation);
            instance = this;
            detector = OreDetector.GetOreDetector();
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
            helper.Events.Display.Rendered += this.OnRendered;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.World.NpcListChanged += OnNPCListChanged;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.Saved += OnSaved;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            ladderTexture = Helper.ModContent.Load<Texture2D>("assets\\ladder.png");
            holeTexture = Helper.ModContent.Load<Texture2D>("assets\\hole.png");
            blackListButtonTexture = Helper.ModContent.Load<Texture2D>("assets\\blacklist.png");
            whiteListButtonTexture = Helper.ModContent.Load<Texture2D>("assets\\whitelist.png");
            Config = Helper.ReadConfig<ModConfig>();
            saveModel = new SaveModel();
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        ///
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => I18n.OreDetector_Config_PositionOption(),
                getValue: () => Config.PositionOption,
                setValue: value => Config.PositionOption = value,
                allowedValues: new string[] { I18n.OreDetector_Config_AbovePlayer(), I18n.OreDetector_Config_TopLeft(), I18n.OreDetector_Config_NextToCursor(), I18n.OreDetector_Config_Custom() }
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => I18n.OreDetector_Config_DrawLineToLadder(),
                getValue: () => Config.arrowPointingToLadder,
                setValue: value => Config.arrowPointingToLadder = value
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => I18n.OreDetector_Config_LineLadderColor(),
                getValue: () => Config.arrowToLadderColor,
                setValue: value => Config.arrowToLadderColor = value,
                allowedValues: new string[] { I18n.OreDetector_Config_Color_Red(), I18n.OreDetector_Config_Color_Green(), I18n.OreDetector_Config_Color_Blue(), I18n.OreDetector_Config_Color_Yellow() }
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => I18n.OreDetector_Config_DrawLineToHole(),
                getValue: () => Config.arrowPointingToHole,
                setValue: value => Config.arrowPointingToHole = value
            );

            configMenu.AddTextOption(
            mod: this.ModManifest,
            name: () => I18n.OreDetector_Config_LineHoleColor(),
            getValue: () => Config.arrowToHoleColor,
            setValue: value => Config.arrowToHoleColor = value,
            allowedValues: new string[] { I18n.OreDetector_Config_Color_Red(), I18n.OreDetector_Config_Color_Green(), I18n.OreDetector_Config_Color_Blue(), I18n.OreDetector_Config_Color_Yellow() }
            );

            configMenu.AddBoolOption(
            mod: this.ModManifest,
            name: () => I18n.OreDetector_DisplayOreName(),
            getValue: () => Config.showOreName,
            setValue: value => Config.showOreName = value
            );

            configMenu.AddKeybind(
            mod: this.ModManifest,
            name: () => I18n.OreDetector_SetCustomPostion(),
            getValue: () => Config.customPositionKeybind,
            setValue: value => Config.customPositionKeybind = value
            );
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => I18n.OreDetector_Config_HideInformation(),
                getValue: () => Config.hideInformationKeybind,
                setValue: value => Config.hideInformationKeybind = value
            );
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => I18n.OreDetector_Config_ShowBlacklist(),
                getValue: () => Config.blacklistMenuKeybind,
                setValue: value => Config.blacklistMenuKeybind = value
            );
        }

        private void OnSaved(object? sender, SavedEventArgs e)
        {
            Helper.Data.WriteSaveData("AvalonMFX.OreDetector", saveModel);
            Monitor.Log("Saved blacklist");
        }
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            saveModel = Helper.Data.ReadSaveData<SaveModel>("AvalonMFX.OreDetector");
                if (saveModel == null)
                    saveModel = new SaveModel();
            Monitor.Log($"Loaded blacklist\nElements: {saveModel.blacklistedNames.Count}");
        }
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) 
                return;

            if (e.Button == Config?.customPositionKeybind)
            {
                Game1.activeClickableMenu = new InvisibleMenu();
                Config.PositionOption = I18n.OreDetector_Config_Custom();
            }
            else if (e.Button == Config.hideInformationKeybind)
            {
                informationHidden = !informationHidden;
            }
            else if (e.Button == Config.blacklistMenuKeybind)
            {
                Game1.activeClickableMenu = new ListModifierUI(ref saveModel);
            }
        }
        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            if (e.NewLocation is MineShaft)
            {
                detector.GetOreInCurrentShaft();
                detector.LookForSpawnedLadders();
            }
        }

        private void OnNPCListChanged(object? sender, NpcListChangedEventArgs e)
        {
            if (detector.currentShaft == null)
                return;

            detector.LookForSpawnedLadders();
        }

        private void OnObjectListChanged(object? sender, ObjectListChangedEventArgs e) 
        {
            if (e.Location != detector.currentShaft)
                return;

            foreach (var item in e.Removed)
            {
                if (!detector.Ores.Keys.Contains(item.Value.DisplayName))
                    continue;

                detector.MinedOres[item.Value.DisplayName].Add(item.Value);
            }
            detector.LookForSpawnedLadders();
            detector.LookForSpawnedHoles();
        }

        private void OnRendered(object? sender, RenderedEventArgs e)
        {
            if(!Context.IsWorldReady) 
                return;

            if (Game1.player.currentLocation != detector.currentShaft || informationHidden)
                return;

            SpriteBatch batch = Game1.spriteBatch;

            string positionOption = Config.PositionOption;
            if (positionOption == I18n.OreDetector_Config_NextToCursor())
                DrawOverlayCursor(batch);
            else if (positionOption == I18n.OreDetector_Config_AbovePlayer())
                DrawOverlayAbovePlayer(batch);
            else if (positionOption == I18n.OreDetector_Config_TopLeft())
                DrawOverlayTopLeftCorner(batch);
            else if (positionOption == I18n.OreDetector_Config_Custom())
                DrawOverlayCustomPosition(batch);

            if (Config.arrowPointingToLadder)
            {
                DrawLineToTiles(batch, lineColors[Config.arrowToLadderColor], detector.ladderPositions);
            }
            if (Config.arrowPointingToHole)
            {
                DrawLineToTiles(batch, lineColors[Config.arrowToHoleColor], detector.HolePositions);
            }
        }

        private void DrawLineToTiles(SpriteBatch batch, Color color, List<Vector2> tiles)
        {
            if (tiles.Count <= 0) return;

            if (Game1.activeClickableMenu != null || Game1.IsFading()) return;

            foreach (Vector2 position in tiles)
            {
                int width = 5;
                Vector2 ladderPosition = new Vector2((position.X * Game1.tileSize) - Game1.viewport.X + Game1.tileSize / 2, (position.Y * Game1.tileSize) - Game1.viewport.Y + Game1.tileSize / 2);
                Vector2 playerPosition = new Vector2(Game1.player.Position.X - Game1.viewport.X + Game1.tileSize / 2, Game1.player.Position.Y - Game1.viewport.Y);
                Vector2 startPos = ladderPosition;
                Vector2 endPos = playerPosition;
                // Create a texture as wide as the distance between two points and as high as
                // the desired thickness of the line.
                var distance = (int)Vector2.Distance(startPos, endPos);
                var texture = new Texture2D(batch.GraphicsDevice, distance, width);

                // Fill texture with given color.

                var data = new Color[distance * width];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = color;
                }
                texture.SetData(data);

                // Rotate about the beginning middle of the line.
                var rotation = (float)Math.Atan2(endPos.Y - startPos.Y, endPos.X - startPos.X);
                var origin = new Vector2(0, width / 2);

                batch.Draw(
                    texture,
                    startPos,
                    null,
                    Color.White,
                    rotation,
                    origin,
                    1.0f,
                    SpriteEffects.None,
                    1.0f);
            }

        }
        private void DrawOverlay(SpriteBatch batch, Vector2 position, float transparency)
        {
            int padding = 0;
            string result = "";
            foreach (var item in detector.Ores)
            {
                if (saveModel.blacklistedNames.Contains(item.Key))
                    continue;

                string oreName = Config.showOreName ? $"{item.Key}: " : ""; 
                string text = $"{oreName}{detector.MinedOres[item.Key].Count} / {item.Value.Count}\n";
                result += text;
            }
            padding = padding > 0 ? padding : 16;
            result += Config.showOreName ? $"{I18n.OreDetector_Ladder()}: " : "";
            result += detector.LadderRevealed ? I18n.OreDetector_Yes() : I18n.OreDetector_No();



            int counter = 0;
            int offset = Config.showOreName ? -8 : -4;
            foreach (var item in detector.Ores)
            {
                if (saveModel.blacklistedNames.Contains(item.Key))
                    continue;

                string itemId = detector.itemIds[item.Key];

                ParsedItemData data = ItemRegistry.GetDataOrErrorItem(itemId);
                string itemTypeId = data.GetItemTypeId();
                Texture2D texture = data.GetTexture();
                Rectangle sourceRect = data.GetSourceRect();
                int bigCraftableOffset = itemTypeId == "(BC)" ? 12 : 0;
                bool isBigCraftable = itemTypeId == "(BC)";
                batch.Draw(texture, position + new Vector2(offset * padding + bigCraftableOffset, Game1.dialogueFont.LineSpacing * counter), sourceRect, Color.White * transparency, 0f, Vector2.Zero, isBigCraftable ? 1.5f : 3f, SpriteEffects.None, 0f);
                counter++;
            }
            batch.Draw(ladderTexture, position + new Vector2(offset * padding, Game1.dialogueFont.LineSpacing * counter), new Rectangle(0, 0, 16, 16), Color.White * transparency, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
            if (detector.isDesertMine)
            {
                result += Config.showOreName ? $"\n{I18n.OreDetector_Hole()}: " : "\n";
                result += detector.HoleRevealed ? I18n.OreDetector_Yes() : I18n.OreDetector_No();
                batch.Draw(holeTexture, position + new Vector2(offset * padding, Game1.dialogueFont.LineSpacing * (counter + 1)), new Rectangle(0, 0, 16, 16), Color.White * transparency, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
            }

            batch.DrawString(Game1.dialogueFont, result, position, Color.White * transparency);
        }

        private void DrawOverlayCustomPosition(SpriteBatch batch)
        {
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null || Game1.IsFading())
                return;

            Vector2 position = Config.customPosition;
            DrawOverlay(batch, position , 1f);
        }
        private void DrawOverlayTopLeftCorner(SpriteBatch batch)
        {
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null || Game1.IsFading())
                return;

            Vector2 position = new Vector2(64, 80);
            DrawOverlay(batch, position, 1f);
        }

        private void DrawOverlayCursor(SpriteBatch batch)
        {
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
                return;
            Farmer player = Game1.player;
            float transparency = player.isMoving() && !player.UsingTool ? 0.1f : 1f;
            Vector2 position = Game1.getMousePosition().ToVector2() + new Vector2(125, 0);
            DrawOverlay(batch, position, transparency);
        }
        private void DrawOverlayAbovePlayer(SpriteBatch batch)
        {
            if (Game1.activeClickableMenu != null)
                return;

            Farmer player = Game1.player;
            float transparency = player.isMoving() && !player.UsingTool ? 0.1f : 1.0f;
            Vector2 position = new Vector2(player.Position.X - Game1.viewport.X, player.Position.Y - Game1.viewport.Y);

            string result = "";
            int text_offsetY = detector.Ores.Count - 1;

            int padding = 0;
            foreach (var item in detector.Ores)
            {
                if (saveModel.blacklistedNames.Contains(item.Key))
                    continue;
                string oreName = Config.showOreName ? $"{item.Key}: " : "";
                string text = $"{oreName}{detector.MinedOres[item.Key].Count} / {item.Value.Count}\n";
                if (text.Length > padding)
                    padding = text.Length;
                result += text;
            }
            padding = padding > 0 ? padding : 16;
            int offset = Config.showOreName ? -4 : -8;
            result += Config.showOreName ? $"{I18n.OreDetector_Ladder()}: " : "";
            result += detector.LadderRevealed ? I18n.OreDetector_Yes() : I18n.OreDetector_No();
            Vector2 finalPosition = position + new Vector2(-4 * padding, -200 - Game1.dialogueFont.LineSpacing * text_offsetY);

            int counter = 0;
            foreach (var item in detector.Ores)
            {
                if (saveModel.blacklistedNames.Contains(item.Key))
                    continue;
                string itemId = detector.itemIds[item.Key];

                ParsedItemData data = ItemRegistry.GetDataOrErrorItem(itemId);
                string itemTypeId = data.GetItemTypeId();
                Texture2D texture = data.GetTexture();
                Rectangle sourceRect = data.GetSourceRect();
                int bigCraftableOffset = itemTypeId == "(BC)" ? 12 : 0;
                bool isBigCraftable = itemTypeId == "(BC)";
                batch.Draw(texture, finalPosition + new Vector2(offset * padding + bigCraftableOffset, Game1.dialogueFont.LineSpacing * counter), sourceRect, Color.White * transparency, 0f, Vector2.Zero, isBigCraftable ? 1.5f : 3f, SpriteEffects.None, 0f);
                counter++;
            }
            batch.Draw(ladderTexture, finalPosition + new Vector2(offset * padding, Game1.dialogueFont.LineSpacing * counter + 10), new Rectangle(0, 0, 16, 16), Color.White * transparency, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
            if (detector.isDesertMine)
            {
                result += Config.showOreName ? $"\n{I18n.OreDetector_Hole()}: " : "\n";
                result += detector.HoleRevealed ? I18n.OreDetector_Yes() : I18n.OreDetector_No();
                batch.Draw(holeTexture, finalPosition + new Vector2(offset * padding, Game1.dialogueFont.LineSpacing * (counter + 1)), new Rectangle(0, 0, 16, 16), Color.White * transparency, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
            }

            batch.DrawString(Game1.dialogueFont, result, finalPosition, Color.White * transparency);
        }
    }
}