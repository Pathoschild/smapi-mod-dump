/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Framework.Menus.Elements.Buttons;
using SkillPrestige.Professions;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Framework.Menus.Dialogs
{
    /// <summary>represents a message dialog box for skill level up where a specific profession is granted.</summary>
    internal class LevelUpMessageDialogWithProfession : LevelUpMessageDialog
    {
        private readonly Profession Profession;
        private static int TextPadding => 4 * Game1.pixelZoom;
        private int SectionWidth => (this.width - spaceToClearSideBorder * 2) / 2 - TextPadding;
        private Vector2 ProfessionIconLocation;

        public LevelUpMessageDialogWithProfession(Rectangle bounds, string message, Skill skill, Profession profession)
            : base(bounds, message, skill)
        {
            this.Profession = profession;
            this.allClickableComponents = new List<ClickableComponent> { this.upperRightCloseButton };
        }

        protected override void DrawMessage(SpriteBatch spriteBatch)
        {
            int xLocationOfMessage = this.xPositionOnScreen + spaceToClearSideBorder * 2 + TextPadding;
            int yLocationOfMessage = this.YPositionOfHeaderPartition + spaceToClearTopBorder / 2;
            this.DrawMessage(spriteBatch, Game1.dialogueFont, new Vector2(xLocationOfMessage, yLocationOfMessage), this.SectionWidth);
        }

        protected override void DrawDecorations(SpriteBatch spriteBatch)
        {
            base.DrawDecorations(spriteBatch);
            this.DrawProfession(spriteBatch);
        }

        private void DrawProfession(SpriteBatch spriteBatch)
        {
            int xLocationOfProfessionBox = this.xPositionOnScreen + this.SectionWidth + TextPadding;
            int yLocationOfProfessionBox = this.YPositionOfHeaderPartition + spaceToClearTopBorder / 2 + TextPadding;
            var professionBoxBounds = new Rectangle(xLocationOfProfessionBox, yLocationOfProfessionBox, this.SectionWidth, (Game1.tileSize * 3.5).Ceiling());
            this.DrawProfessionBox(spriteBatch, professionBoxBounds);
        }

        private void DrawProfessionBox(SpriteBatch spriteBatch, Rectangle bounds)
        {

            spriteBatch.Draw(MinimalistProfessionButton.ProfessionButtonTexture, bounds, Color.White);
            this.DrawProfessionTitleText(spriteBatch, bounds);
            this.DrawProfessionIcon(spriteBatch, bounds);
            this.DrawProfessionEffectText(spriteBatch, bounds);
        }

        private void DrawProfessionTitleText(SpriteBatch spriteBatch, Rectangle professionBounds)
        {
            var textLocation = new Vector2(professionBounds.X + TextPadding, professionBounds.Y + TextPadding);
            spriteBatch.DrawString(Game1.dialogueFont, this.Profession.DisplayName, textLocation, Game1.textColor);
        }

        private void DrawProfessionIcon(SpriteBatch spriteBatch, Rectangle professionBounds)
        {
            var locationOfIconRelativeToButton = new Vector2(professionBounds.Width - (TextPadding + this.Profession.IconSourceRectangle.Width * Game1.pixelZoom), TextPadding);
            var buttonLocation = new Vector2(professionBounds.X, professionBounds.Y);
            this.ProfessionIconLocation = buttonLocation + locationOfIconRelativeToButton;
            spriteBatch.Draw(this.Profession.Texture, this.ProfessionIconLocation, this.Profession.IconSourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        }

        private void DrawProfessionEffectText(SpriteBatch spriteBatch, Rectangle professionBounds)
        {
            float bottomOfIcon = this.ProfessionIconLocation.Y + this.Profession.IconSourceRectangle.Height * Game1.pixelZoom;
            var nextLineLocation = new Vector2(professionBounds.X + TextPadding, bottomOfIcon + TextPadding);
            var effectTextFont = Game1.smallFont;
            spriteBatch.DrawString(effectTextFont, string.Join("\n", this.Profession.EffectText.Select(effect => effect.WrapText(effectTextFont, professionBounds.Width - TextPadding * 2))), nextLineLocation, Game1.textColor);
        }
    }
}
