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
using StardewValley;
// ReSharper disable PossibleLossOfFraction

namespace SkillPrestige.Framework.Menus.Dialogs
{
    /// <summary>Represents a message dialog for skill level up messages.</summary>
    internal class LevelUpMessageDialog : MessageDialog
    {
        /*********
        ** Fields
        *********/
        private readonly Skill Skill;
        // ReSharper disable once MemberCanBePrivate.Global
        protected readonly int YPositionOfHeaderPartition;


        /*********
        ** Public methods
        *********/
        public LevelUpMessageDialog(Rectangle bounds, string message, Skill skill)
            : base(bounds, message)
        {
            this.Skill = skill;
            this.YPositionOfHeaderPartition = this.yPositionOnScreen + (Game1.tileSize * 2.5).Floor();
        }


        /*********
        ** Protected methods
        *********/
        protected override void DrawDecorations(SpriteBatch spriteBatch)
        {
            base.DrawDecorations(spriteBatch);
            this.DrawLevelUpHeader(spriteBatch);
        }

        private void DrawLevelUpHeader(SpriteBatch spriteBatch)
        {
            string title = $"{this.Skill.Type.Name} Level Up";
            this.DrawSkillIcon(spriteBatch, new Vector2(this.xPositionOnScreen + spaceToClearSideBorder + borderWidth, this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4));
            spriteBatch.DrawString(Game1.dialogueFont, title, new Vector2(this.xPositionOnScreen + this.width / 2 - Game1.dialogueFont.MeasureString(title).X / 2f, this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4), Game1.textColor);
            this.DrawSkillIcon(spriteBatch, new Vector2(this.xPositionOnScreen + this.width - spaceToClearSideBorder - borderWidth - Game1.tileSize, this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4));
            this.drawHorizontalPartition(spriteBatch, this.yPositionOnScreen + (Game1.tileSize * 2.5).Floor());
        }

        private void DrawSkillIcon(SpriteBatch spriteBatch, Vector2 location)
        {
            Utility.drawWithShadow(spriteBatch, this.Skill.SkillIconTexture, location, this.Skill.SourceRectangleForSkillIcon, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.88f);
        }

        protected override void DrawMessage(SpriteBatch spriteBatch)
        {
            int textPadding = 2 * Game1.pixelZoom;
            int xLocationOfMessage = this.xPositionOnScreen + borderWidth + textPadding;
            int yLocationOfMessage = this.YPositionOfHeaderPartition + spaceToClearTopBorder;
            this.DrawMessage(spriteBatch, Game1.dialogueFont, new Vector2(xLocationOfMessage, yLocationOfMessage), this.width - borderWidth * 2);
        }
    }
}
