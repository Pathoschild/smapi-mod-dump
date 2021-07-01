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
using System.Linq;
using LovedLabels.Framework;
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
        private ModConfig _modConfigs;
        private Texture2D _hearts;
        private string _hoverText;

        public override void Entry(IModHelper helper)
        {
            _modConfigs = helper.ReadConfig<ModConfig>();
            _hearts = helper.Content.Load<Texture2D>("assets/hearts.png");

            helper.Events.GameLoop.GameLaunched += onLaunched;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Display.Rendered += OnRendered;
        }

        private void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            var genericModConfigMenuAPI = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (genericModConfigMenuAPI != null)
            {
                genericModConfigMenuAPI.RegisterModConfig(ModManifest, () => _modConfigs = new ModConfig(), () => Helper.WriteConfig(_modConfigs));

                genericModConfigMenuAPI.RegisterSimpleOption(ModManifest, "Toggle UI Key", "The keybind that toggles the UI in-game.", () => _modConfigs.ToggleUIKey, (KeybindList val) => _modConfigs.ToggleUIKey = val);
                genericModConfigMenuAPI.RegisterSimpleOption(ModManifest, "Toggle UI", "If true shows the UI, if false hides it.", () => _modConfigs.IsUIEnabled, (bool val) => _modConfigs.IsUIEnabled = val);

                genericModConfigMenuAPI.RegisterSimpleOption(ModManifest, "Already petted message", "Message already petted", () => _modConfigs.AlreadyPettedMessage, (string val) => _modConfigs.AlreadyPettedMessage = val);
                genericModConfigMenuAPI.RegisterSimpleOption(ModManifest, "Needs petting message", "Message not petted", () => _modConfigs.NeedsPettingMessage, (string val) => _modConfigs.NeedsPettingMessage = val);
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (_modConfigs.ToggleUIKey.JustPressed())
            {
                _modConfigs.IsUIEnabled = !_modConfigs.IsUIEnabled;
            }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree || !Game1.currentLocation.IsFarm) { return; }
            _hoverText = null;
            if (!_modConfigs.IsUIEnabled)
            {
                return;
            }

            GameLocation location = Game1.currentLocation;
            Vector2 mousePos = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;

            FarmAnimal[] animals = new FarmAnimal[0];
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
                RectangleF animalBoundaries = new RectangleF(animal.position.X, animal.position.Y - animal.Sprite.getHeight(), animal.Sprite.getWidth() * 3 + animal.Sprite.getWidth() / 1.5f, animal.Sprite.getHeight() * 4);

                if (animalBoundaries.Contains(mousePos.X * Game1.tileSize, mousePos.Y * Game1.tileSize))
                    _hoverText = animal.wasPet.Value ? _modConfigs.AlreadyPettedMessage : _modConfigs.NeedsPettingMessage;
            }

            foreach (Pet pet in location.characters.OfType<Pet>())
            {
                RectangleF petBoundaries = new RectangleF(pet.position.X, pet.position.Y - pet.Sprite.getHeight() * 2, pet.Sprite.getWidth() * 3 + pet.Sprite.getWidth() / 1.5f, pet.Sprite.getHeight() * 4);
                if (petBoundaries.Contains(mousePos.X * Game1.tileSize, mousePos.Y * Game1.tileSize))
                {
                    NetLongDictionary<int, NetInt> lastPettedDays = Helper.Reflection.GetField<NetLongDictionary<int, NetInt>>(pet, "lastPetDay").GetValue();
                    bool wasPet = lastPettedDays.Values.Any(day => day == Game1.Date.TotalDays);
                    _hoverText = wasPet ? _modConfigs.AlreadyPettedMessage : _modConfigs.NeedsPettingMessage;
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
            var sourceY = (hoverText == _modConfigs.AlreadyPettedMessage) ? 0 : 32;
            var heartpos = new Vector2(x + textSize.X + halfHeartSize, y + halfHeartSize);
            b.Draw(_hearts, heartpos, new Rectangle(0, sourceY, 32, 32), Color.White);
        }
    }

    //Generic Mod Config Menu API
    public interface GenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

        void UnregisterModConfig(IManifest mod);

        void SetDefaultIngameOptinValue(IManifest mod, bool optedIn);

        void StartNewPage(IManifest mod, string pageName);

        void OverridePageDisplayName(IManifest mod, string pageName, string displayName);

        void RegisterLabel(IManifest mod, string labelName, string labelDesc);

        void RegisterPageLabel(IManifest mod, string labelName, string labelDesc, string newPage);

        void RegisterParagraph(IManifest mod, string paragraph);

        void RegisterImage(IManifest mod, string texPath, Rectangle? texRect = null, int scale = 4);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<SButton> optionGet, Action<SButton> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<KeybindList> optionGet, Action<KeybindList> optionSet);

        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet, int min, int max);

        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet, float min, float max);

        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet, int min, int max, int interval);

        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet, float min, float max, float interval);

        void RegisterChoiceOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet, string[] choices);

        void RegisterComplexOption(IManifest mod, string optionName, string optionDesc,
                                   Func<Vector2, object, object> widgetUpdate,
                                   Func<SpriteBatch, Vector2, object, object> widgetDraw,
                                   Action<object> onSave);

        void SubscribeToChange(IManifest mod, Action<string, bool> changeHandler);

        void SubscribeToChange(IManifest mod, Action<string, int> changeHandler);

        void SubscribeToChange(IManifest mod, Action<string, float> changeHandler);

        void SubscribeToChange(IManifest mod, Action<string, string> changeHandler);

        void OpenModMenu(IManifest mod);
    }
}