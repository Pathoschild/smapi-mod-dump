/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thespbgamer/LovedLabelsRedux
**
*************************************************/

using System;
using System.IO;
using System.Linq;
using LovedLabels.Framework;
using LovedLabelsRedux.GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Network;

namespace LovedLabelsRedux
{
    public class ModEntry : Mod
    {
        private ModConfig configsForTheMod;
        private Texture2D _hearts;
        private string _hoverText;

        public override void Entry(IModHelper helper)
        {
            CommonHelper.RemoveObsoleteFiles(this, "LovedLabelsRedux.pdb");

            configsForTheMod = helper.ReadConfig<ModConfig>();
            _hearts = helper.ModContent.Load<Texture2D>("assets/hearts.png");

            helper.Events.GameLoop.GameLaunched += OnLaunched;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Display.Rendered += OnRendered;
        }

        private void OnLaunched(object sender, GameLaunchedEventArgs e)
        {
            var genericModConfigMenuAPI = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (genericModConfigMenuAPI != null)
            {
                genericModConfigMenuAPI.Register(ModManifest, () => configsForTheMod = new ModConfig(), () => Helper.WriteConfig(configsForTheMod));

                genericModConfigMenuAPI.AddSectionTitle(ModManifest, Helper.Translation.Get("keybinds.title.name").ToString, Helper.Translation.Get("keybinds.title.description").ToString);
                genericModConfigMenuAPI.AddKeybindList(ModManifest, () => configsForTheMod.KeybindListToggleUIKey, (KeybindList val) => configsForTheMod.KeybindListToggleUIKey = val, Helper.Translation.Get("keybinds.ToggleUIKey.name").ToString, Helper.Translation.Get("keybinds.ToggleUIKey.description").ToString);

                genericModConfigMenuAPI.AddSectionTitle(ModManifest, Helper.Translation.Get("others.title.name").ToString, Helper.Translation.Get("others.title.description").ToString);
                genericModConfigMenuAPI.AddBoolOption(ModManifest, () => configsForTheMod.IsUIEnabled, (bool val) => configsForTheMod.IsUIEnabled = val, Helper.Translation.Get("others.ToggleUICurrentState.name").ToString, Helper.Translation.Get("others.ToggleUICurrentState.description").ToString);
                genericModConfigMenuAPI.AddBoolOption(ModManifest, () => configsForTheMod.IsPettingEnabled, (bool val) => configsForTheMod.IsPettingEnabled = val, Helper.Translation.Get("others.ToggleIsPetting.name").ToString, Helper.Translation.Get("others.ToggleIsPetting.description").ToString);

                genericModConfigMenuAPI.AddTextOption(ModManifest, () => configsForTheMod.AlreadyPettedMessage, (string val) => configsForTheMod.AlreadyPettedMessage = val, Helper.Translation.Get("others.AlreadyPettedMessage.name").ToString, Helper.Translation.Get("others.AlreadyPettedMessage.description").ToString);
                genericModConfigMenuAPI.AddTextOption(ModManifest, () => configsForTheMod.NeedsPettingMessage, (string val) => configsForTheMod.NeedsPettingMessage = val, Helper.Translation.Get("others.NeedsPettingMessage.name").ToString, Helper.Translation.Get("others.NeedsPettingMessage.description").ToString);
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (configsForTheMod.KeybindListToggleUIKey.JustPressed())
            {
                configsForTheMod.IsUIEnabled = !configsForTheMod.IsUIEnabled;
            }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree || !Game1.currentLocation.IsFarm) { return; }
            _hoverText = null;

            if (configsForTheMod.IsPettingEnabled)
            {
                FarmAnimal[] farmAnimals = Game1.getFarm().getAllFarmAnimals().Where(p => !p.wasPet.Value).ToArray();

                if (farmAnimals.Any())
                {
                    foreach (FarmAnimal animal in farmAnimals)
                    {
                        //this.Monitor.Log($"Animal: {animal.Name} - {animal.displayName} - {animal.type} - {animal.age} - {animal.wasPet.Value} - {animal.fullness.Value} - {animal.friendshipTowardFarmer.Value} - {animal.daysSinceLastFed.Value}", LogLevel.Info);
                        animal.pet(Game1.player);
                    }
                }
            }

            if (!configsForTheMod.IsUIEnabled)
            {
                return;
            }

            GameLocation location = Game1.currentLocation;
            Vector2 mousePos = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;

            FarmAnimal[] animals = Array.Empty<FarmAnimal>();
            switch (location)
            {
                case AnimalHouse house:
                    animals = house.animals.Values.ToArray();
                    break;

                case Farm farm:
                    animals = farm.animals.Values.ToArray();
                    break;
            }

            foreach (FarmAnimal animal in animals)
            {
                RectangleF animalBoundaries = new(animal.position.X, animal.position.Y - animal.Sprite.getHeight(), animal.Sprite.getWidth() * 3 + animal.Sprite.getWidth() / 1.5f, animal.Sprite.getHeight() * 4);

                if (animalBoundaries.Contains(mousePos.X * Game1.tileSize, mousePos.Y * Game1.tileSize))
                    _hoverText = animal.wasPet.Value ? configsForTheMod.AlreadyPettedMessage : configsForTheMod.NeedsPettingMessage;
            }

            foreach (Pet pet in location.characters.OfType<Pet>())
            {
                RectangleF petBoundaries = new(pet.position.X, pet.position.Y - pet.Sprite.getHeight() * 2, pet.Sprite.getWidth() * 3 + pet.Sprite.getWidth() / 1.5f, pet.Sprite.getHeight() * 4);
                if (petBoundaries.Contains(mousePos.X * Game1.tileSize, mousePos.Y * Game1.tileSize))
                {
                    NetLongDictionary<int, NetInt> lastPettedDays = Helper.Reflection.GetField<NetLongDictionary<int, NetInt>>(pet, "lastPetDay").GetValue();
                    bool wasPet = lastPettedDays.Values.Any(day => day == Game1.Date.TotalDays);
                    _hoverText = wasPet ? configsForTheMod.AlreadyPettedMessage : configsForTheMod.NeedsPettingMessage;
                }
            }
        }

        private void OnRendered(object sender, RenderedEventArgs e)
        {
            if (Context.IsPlayerFree && _hoverText != null)
                DrawSimpleTooltip(Game1.spriteBatch, _hoverText, Game1.smallFont);
        }

        private void DrawSimpleTooltip(SpriteBatch b, string hoverText, SpriteFont font)
        {
            var textSize = font.MeasureString(hoverText);
            var width = (int)textSize.X + _hearts.Width + Game1.tileSize / 2;
            var height = Math.Max(60, (int)textSize.Y + Game1.tileSize / 2);
            var x = Game1.getOldMouseX() + Game1.tileSize / 2;
            var y = Game1.getOldMouseY() + Game1.tileSize / 2;
            if (x + width > Game1.viewport.Width)
            {
                x = Game1.viewport.Width - width;
                y += Game1.tileSize / 4;
            }
            if (y + height > Game1.viewport.Height)
            {
                x += Game1.tileSize / 4;
                y = Game1.viewport.Height - height;
            }
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, Color.White);
            if (hoverText.Length > 1)
            {
                var tPosVector = new Vector2(x + (Game1.tileSize / 4), y + (Game1.tileSize / 4 + 4));
                b.DrawString(font, hoverText, tPosVector + new Vector2(2f, 2f), Game1.textShadowColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector + new Vector2(0f, 2f), Game1.textShadowColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector + new Vector2(2f, 0f), Game1.textShadowColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector, Game1.textColor * 0.9f, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            var halfHeartSize = _hearts.Width * 0.5f;
            var sourceY = (hoverText == configsForTheMod.AlreadyPettedMessage) ? 0 : 32;
            var heartpos = new Vector2(x + textSize.X + halfHeartSize, y + halfHeartSize);
            b.Draw(_hearts, heartpos, new Rectangle(0, sourceY, 32, 32), Color.White);
        }

        private class CommonHelper
        {
            internal static void RemoveObsoleteFiles(IMod mod, params string[] relativePaths)
            {
                string basePath = mod.Helper.DirectoryPath;

                foreach (string relativePath in relativePaths)
                {
                    string fullPath = Path.Combine(basePath, relativePath);
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            File.Delete(fullPath);
                            mod.Monitor.Log($"Removed obsolete file '{relativePath}'.");
                        }
                        catch (Exception ex)
                        {
                            mod.Monitor.Log($"Failed deleting obsolete file '{relativePath}':\n{ex}");
                        }
                    }
                }
            }
        }
    }

    namespace GenericModConfigMenu
    {
        /// <summary>The API which lets other mods add a config UI through Generic Mod Config Menu.</summary>
        public interface IGenericModConfigMenuApi
        {
            /*********
            ** Methods
            *********/
            /****
            ** Must be called first
            ****/

            /// <summary>Register a mod whose config can be edited through the UI.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="reset">Reset the mod's config to its default values.</param>
            /// <param name="save">Save the mod's current config to the <c>config.json</c> file.</param>
            /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
            /// <remarks>Each mod can only be registered once, unless it's deleted via <see cref="Unregister"/> before calling this again.</remarks>
            void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

            /****
            ** Basic options
            ****/

            /// <summary>Add a section title at the current position in the form.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="text">The title text shown in the form.</param>
            /// <param name="tooltip">The tooltip text shown when the cursor hovers on the title, or <c>null</c> to disable the tooltip.</param>
            void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);

            /// <summary>Add a paragraph of text at the current position in the form.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="text">The paragraph text to display.</param>
            void AddParagraph(IManifest mod, Func<string> text);

            /// <summary>Add an image at the current position in the form.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="texture">The image texture to display.</param>
            /// <param name="texturePixelArea">The pixel area within the texture to display, or <c>null</c> to show the entire image.</param>
            /// <param name="scale">The zoom factor to apply to the image.</param>
            void AddImage(IManifest mod, Func<Texture2D> texture, Rectangle? texturePixelArea = null, int scale = Game1.pixelZoom);

            /// <summary>Add a boolean option at the current position in the form.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="getValue">Get the current value from the mod config.</param>
            /// <param name="setValue">Set a new value in the mod config.</param>
            /// <param name="name">The label text to show in the form.</param>
            /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
            /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
            void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

            /// <summary>Add an integer option at the current position in the form.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="getValue">Get the current value from the mod config.</param>
            /// <param name="setValue">Set a new value in the mod config.</param>
            /// <param name="name">The label text to show in the form.</param>
            /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
            /// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
            /// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
            /// <param name="interval">The interval of values that can be selected.</param>
            /// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
            /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
            void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);

            /// <summary>Add a float option at the current position in the form.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="getValue">Get the current value from the mod config.</param>
            /// <param name="setValue">Set a new value in the mod config.</param>
            /// <param name="name">The label text to show in the form.</param>
            /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
            /// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
            /// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
            /// <param name="interval">The interval of values that can be selected.</param>
            /// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
            /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
            void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);

            /// <summary>Add a string option at the current position in the form.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="getValue">Get the current value from the mod config.</param>
            /// <param name="setValue">Set a new value in the mod config.</param>
            /// <param name="name">The label text to show in the form.</param>
            /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
            /// <param name="allowedValues">The values that can be selected, or <c>null</c> to allow any.</param>
            /// <param name="formatAllowedValue">Get the display text to show for a value from <paramref name="allowedValues"/>, or <c>null</c> to show the values as-is.</param>
            /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
            void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);

            /// <summary>Add a key binding at the current position in the form.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="getValue">Get the current value from the mod config.</param>
            /// <param name="setValue">Set a new value in the mod config.</param>
            /// <param name="name">The label text to show in the form.</param>
            /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
            /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
            void AddKeybind(IManifest mod, Func<SButton> getValue, Action<SButton> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

            /// <summary>Add a key binding list at the current position in the form.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="getValue">Get the current value from the mod config.</param>
            /// <param name="setValue">Set a new value in the mod config.</param>
            /// <param name="name">The label text to show in the form.</param>
            /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
            /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
            void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

            /****
            ** Multi-page management
            ****/

            /// <summary>Start a new page in the mod's config UI, or switch to that page if it already exists. All options registered after this will be part of that page.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="pageId">The unique page ID.</param>
            /// <param name="pageTitle">The page title shown in its UI, or <c>null</c> to show the <paramref name="pageId"/> value.</param>
            /// <remarks>You must also call <see cref="AddPageLink"/> to make the page accessible. This is only needed to set up a multi-page config UI. If you don't call this method, all options will be part of the mod's main config UI instead.</remarks>
            void AddPage(IManifest mod, string pageId, Func<string> pageTitle = null);

            /// <summary>Add a link to a page added via <see cref="AddPage"/> at the current position in the form.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="pageId">The unique ID of the page to open when the link is clicked.</param>
            /// <param name="text">The link text shown in the form.</param>
            /// <param name="tooltip">The tooltip text shown when the cursor hovers on the link, or <c>null</c> to disable the tooltip.</param>
            void AddPageLink(IManifest mod, string pageId, Func<string> text, Func<string> tooltip = null);

            /****
            ** Advanced
            ****/

            /// <summary>Add an option at the current position in the form using custom rendering logic.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="name">The label text to show in the form.</param>
            /// <param name="draw">Draw the option in the config UI. This is called with the sprite batch being rendered and the pixel position at which to start drawing.</param>
            /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
            /// <param name="beforeMenuOpened">A callback raised just before the menu containing this option is opened.</param>
            /// <param name="beforeSave">A callback raised before the form's current values are saved to the config (i.e. before the <c>save</c> callback passed to <see cref="Register"/>).</param>
            /// <param name="afterSave">A callback raised after the form's current values are saved to the config (i.e. after the <c>save</c> callback passed to <see cref="Register"/>).</param>
            /// <param name="beforeReset">A callback raised before the form is reset to its default values (i.e. before the <c>reset</c> callback passed to <see cref="Register"/>).</param>
            /// <param name="afterReset">A callback raised after the form is reset to its default values (i.e. after the <c>reset</c> callback passed to <see cref="Register"/>).</param>
            /// <param name="beforeMenuClosed">A callback raised just before the menu containing this option is closed.</param>
            /// <param name="height">The pixel height to allocate for the option in the form, or <c>null</c> for a standard input-sized option. This is called and cached each time the form is opened.</param>
            /// <param name="fieldId">The unique field ID for use with <see cref="OnFieldChanged"/>, or <c>null</c> to auto-generate a randomized ID.</param>
            /// <remarks>The custom logic represented by the callback parameters is responsible for managing its own state if needed. For example, you can store state in a static field or use closures to use a state variable.</remarks>
            void AddComplexOption(IManifest mod, Func<string> name, Action<SpriteBatch, Vector2> draw, Func<string> tooltip = null, Action beforeMenuOpened = null, Action beforeSave = null, Action afterSave = null, Action beforeReset = null, Action afterReset = null, Action beforeMenuClosed = null, Func<int> height = null, string fieldId = null);

            /// <summary>Set whether the options registered after this point can only be edited from the title screen.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
            /// <remarks>This lets you have different values per-field. Most mods should just set it once in <see cref="Register"/>.</remarks>
            void SetTitleScreenOnlyForNextOptions(IManifest mod, bool titleScreenOnly);

            /// <summary>Register a method to notify when any option registered by this mod is edited through the config UI.</summary>
            /// <param name="mod">The mod's manifest.</param>
            /// <param name="onChange">The method to call with the option's unique field ID and new value.</param>
            /// <remarks>Options use a randomized ID by default; you'll likely want to specify the <c>fieldId</c> argument when adding options if you use this.</remarks>
            void OnFieldChanged(IManifest mod, Action<string, object> onChange);

            /// <summary>Open the config UI for a specific mod.</summary>
            /// <param name="mod">The mod's manifest.</param>
            void OpenModMenu(IManifest mod);

            /// <summary>Get the currently-displayed mod config menu, if any.</summary>
            /// <param name="mod">The manifest of the mod whose config menu is being shown, or <c>null</c> if not applicable.</param>
            /// <param name="page">The page ID being shown for the current config menu, or <c>null</c> if not applicable. This may be <c>null</c> even if a mod config menu is shown (e.g. because the mod doesn't have pages).</param>
            /// <returns>Returns whether a mod config menu is being shown.</returns>
            bool TryGetCurrentMenu(out IManifest mod, out string page);

            /// <summary>Remove a mod from the config UI and delete all its options and pages.</summary>
            /// <param name="mod">The mod's manifest.</param>
            void Unregister(IManifest mod);
        }
    }
}