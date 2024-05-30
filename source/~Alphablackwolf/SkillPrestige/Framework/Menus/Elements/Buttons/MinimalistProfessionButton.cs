/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Logging;
using SkillPrestige.Professions;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

// ReSharper disable PossibleLossOfFraction

namespace SkillPrestige.Framework.Menus.Elements.Buttons
{
    /// <summary>Represents a profession button on the prestige menu. Used to allow the user to choose to permanently obtain a profession.</summary>
    internal class MinimalistProfessionButton : Button
    {
        public bool IsDisabled => this.Selected || !this.IsObtainable || !this.CanBeAfforded;
        private Color DrawColor => this.IsDisabled ? Color.Gray : Color.White;
        private readonly Rectangle CheckmarkSourceRectangle = new (0, 0, 32, 32);

        private static int TextYOffset => 4 * Game1.pixelZoom;
        private Vector2 IconLocation;

        protected override Texture2D ButtonTexture
        {
            get => ProfessionButtonTexture;
            init => ProfessionButtonTexture = value;
        }

        protected override string HoverText => $"{this.HoverTextPrefix}\n\n{(this.Profession?.EffectText == null ? string.Empty : string.Join("\n", this.Profession.EffectText))}";

        private string HoverTextPrefix => this.Selected
            ? $"You already permanently have the {this.Profession.DisplayName} profession."
            : this.IsObtainable
                ? this.CanBeAfforded
                    ? $"Click to permanently obtain the {this.Profession.DisplayName} profession."
                    : $"You cannot afford this profession,\nyou need {this.GetPrestigeCost()} prestige point(s) in this skill to purchase it."
                : $"This profession is not available to obtain permanently until the \n{(this.Profession as TierTwoProfession)?.TierOneProfession.DisplayName} profession has been permanently obtained.";

        protected override string Text => string.Join("\n", this.Profession.DisplayName.Split(' '));

        public static Texture2D ProfessionButtonTexture { get; set; }

        public Profession Profession { get; init; }
        public bool Selected { private get; set; }
        public bool IsObtainable { private get; set; }
        public bool CanBeAfforded { private get; set; }

        public MinimalistProfessionButton()
        {
            this.TitleTextFont = Game1.dialogueFont;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.ButtonTexture, this.Bounds, this.DrawColor);
            this.DrawIcon(spriteBatch);
            this.DrawText(spriteBatch);
            this.DrawCheckmark(spriteBatch);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="e">The event data.</param>
        /// <param name="isClick">Whether the button press is a click.</param>
        public override void OnButtonPressed(ButtonPressedEventArgs e, bool isClick)
        {
            base.OnButtonPressed(e, isClick);
            if (this.IsDisabled)
                return;

            // ReSharper disable once InvertIf
            if (isClick && this.IsHovered)
            {
                Game1.playSound("bigSelect");
                Prestige.AddPrestigeProfession(this.Profession.Id);
                this.Selected = true;
            }
        }

        private void DrawIcon(SpriteBatch spriteBatch)
        {
            var locationOfIconRelativeToButton = new Vector2(this.Bounds.Width / 2 - this.Profession.IconSourceRectangle.Width * Game1.pixelZoom / 2, TextYOffset);
            var buttonLocation = new Vector2(this.Bounds.X, this.Bounds.Y);
            this.IconLocation = buttonLocation + locationOfIconRelativeToButton;
            spriteBatch.Draw(this.Profession.Texture, this.IconLocation, this.Profession.IconSourceRectangle, this.DrawColor, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        }

        private void DrawText(SpriteBatch spriteBatch)
        {
            int buttonXCenter = this.Bounds.Width / 2;
            float textCenter = this.TitleTextFont.MeasureString(this.Text).X / 2;
            float textXLocationRelativeToButton = buttonXCenter - textCenter;
            int textYLocationRelativeToButton = TextYOffset * 2 + this.Profession.IconSourceRectangle.Height * Game1.pixelZoom;
            var locationOfTextRelativeToButton = new Vector2(textXLocationRelativeToButton, textYLocationRelativeToButton);
            this.DrawTitleText(spriteBatch, locationOfTextRelativeToButton);
        }

        private bool CheckmarkTextureMissingLogged;

        private void DrawCheckmark(SpriteBatch spriteBatch)
        {
            if (!this.Selected)
                return;
            if (ModEntry.CheckmarkTexture == null)
            {
                if (this.CheckmarkTextureMissingLogged) return;
                Logger.LogWarning("ProfessionButton - Checkmark texture not loaded, skipping checkmark draw action.");
                this.CheckmarkTextureMissingLogged = true;
                return;
            }
            var locationOfCheckmarkRelativeToButton = new Vector2(this.Bounds.Width - this.CheckmarkSourceRectangle.Width * Game1.pixelZoom / 8, 0);
            var buttonLocation = new Vector2(this.Bounds.X, this.Bounds.Y);
            var checkmarkLocation = buttonLocation + locationOfCheckmarkRelativeToButton;

            spriteBatch.Draw(ModEntry.CheckmarkTexture, checkmarkLocation, this.CheckmarkSourceRectangle, Color.White, 0f, Vector2.Zero, Game1.pixelZoom / 2f, SpriteEffects.None, 1f);
        }

        /// <summary>Raised when the player begins hovering over the button.</summary>
        protected override void OnMouseHovered()
        {
            base.OnMouseHovered();
            if (this.IsDisabled)
                return;

            Game1.playSound("smallSelect");
        }

        private int GetPrestigeCost()
        {
            int tier = this.Profession.LevelAvailableAt / 5;
            switch (tier)
            {
                case 1:
                    return PerSaveOptions.Instance.CostOfTierOnePrestige;
                case 2:
                    return PerSaveOptions.Instance.CostOfTierTwoPrestige;
                default:
                    Logger.LogWarning("Tier for profession not found, defaulting to a cost of 1.");
                    return 1;
            }
        }
    }
}
