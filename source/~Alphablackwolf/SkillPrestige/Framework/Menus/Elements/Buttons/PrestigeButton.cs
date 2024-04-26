/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Framework.Menus.Dialogs;
using SkillPrestige.Logging;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Framework.Menus.Elements.Buttons
{
    /// <summary>Represents a prestige button inside the prestige menu.</summary>
    internal class PrestigeButton : Button
    {
        /*********
        ** Fields
        *********/
        private Skill Skill { get; }
        private bool IsDisabled { get; }
        private Color DisplayColor => this.IsDisabled ? Color.Gray : Color.White;

        protected override string Text => "Prestige";


        /*********
        ** Public methods
        *********/
        public PrestigeButton(bool isDisabled, Skill skill)
        {
            this.TitleTextFont = Game1.dialogueFont;
            this.IsDisabled = isDisabled;
            this.Skill = skill;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.ButtonTexture, this.Bounds, this.DisplayColor);
            this.DrawTitleText(spriteBatch);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="e">The event data.</param>
        /// <param name="isClick">Whether the button press is a click.</param>
        public override void OnButtonPressed(ButtonPressedEventArgs e, bool isClick)
        {
            base.OnButtonPressed(e, isClick);
            if (this.IsDisabled)
                return;

            if (isClick && this.IsHovered)
            {
                Game1.playSound("bigSelect");
                if (PerSaveOptions.Instance.PainlessPrestigeMode)
                {
                    Game1.activeClickableMenu.exitThisMenuNoSound();
                    Prestige.PrestigeSkill(this.Skill);
                    return;
                }
                //Magic numbers for tile size multipliers have been determined through trial and error.
                int dialogWidth = Game1.tileSize * 12;
                int dialogHeight = Game1.tileSize * 6;

                Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
                int screenXCenter = (int)(viewport.Width * (1.0 / Game1.options.zoomLevel)) / 2;
                int screenYCenter = (int)(viewport.Height * (1.0 / Game1.options.zoomLevel)) / 2;
                int dialogXCenter = (dialogWidth + IClickableMenu.borderWidth * 2) / 2;
                int dialogYCenter = (dialogHeight + IClickableMenu.borderWidth * 2) / 2;
                Rectangle bounds = new Rectangle(screenXCenter - dialogXCenter, screenYCenter - dialogYCenter, dialogWidth + IClickableMenu.borderWidth * 2, dialogHeight + IClickableMenu.borderWidth * 2);
                Logger.LogVerbose($"{this.Skill.Type.Name} skill prestige attempted.");
                string message = $"Are you sure you wish to prestige your {this.Skill.Type.Name} skill? This cannot be undone and will revert you back to level 0 {this.Skill.Type.Name}. All associated benefits {(PerSaveOptions.Instance.ResetRecipesOnPrestige ? "and" : "except for")} crafting/cooking recipes will be lost.";
                Game1.activeClickableMenu.exitThisMenuNoSound();
                Game1.activeClickableMenu = new WarningDialog(bounds, message, () => { Prestige.PrestigeSkill(this.Skill); }, () => { });
            }
        }


        /*********
        ** Protected methods
        *********/
        protected override string HoverText
        {
            get
            {
                if (!this.IsDisabled)
                {
                    return
                        $"Click to prestige your {this.Skill?.Type?.Name} skill.\n"
                        + $"{(PerSaveOptions.Instance.UseExperienceMultiplier ? $"Next XP Bonus: {((PrestigeSaveData.CurrentlyLoadedPrestigeSet.Prestiges.Single(x => x.SkillType == this.Skill?.Type).PrestigePoints + PerSaveOptions.Instance.PointsPerPrestige) * PerSaveOptions.Instance.ExperienceMultiplier * 100).Floor()}%" : string.Empty)}";
                }
                if (!PerSaveOptions.Instance.PainlessPrestigeMode)
                    return $"You must reach level 10 in this skill and then\nsleep at least once in order to prestige this skill.";
                int currentExperience = this.Skill.GetSkillExperience();
                int experienceNeeded = PerSaveOptions.Instance.ExperienceNeededPerPainlessPrestige;
                int availableExperience = currentExperience - 15000; //Remove what it takes to get to level 10 in the first place.
                int remainingExperienceNeeded = experienceNeeded - availableExperience;
                return $"You do not have enough experience to prestige this skill.{Environment.NewLine}You need {remainingExperienceNeeded} more experience points to prestige this skill.";
            }
        }

        /// <summary>Raised when the player begins hovering over the button.</summary>
        protected override void OnMouseHovered()
        {
            base.OnMouseHovered();
            if (this.IsDisabled)
                return;

            Game1.playSound("smallSelect");
        }
    }
}
