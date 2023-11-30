/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using SpaceCore.Interface;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BirbShared
{
    internal class KeyedProfession : Skills.Skill.Profession
    {
        readonly object Tokens;
        readonly ITranslationHelper I18n;

        bool PrestigeEnabled => this.PrestigeIcon != null;
        readonly Texture2D PrestigeIcon;
        readonly Texture2D NormalIcon;
        private bool IsPrestiged = false;
        readonly IModHelper ModHelper;

        public KeyedProfession(Skills.Skill skill, string id, Texture2D icon, Texture2D prestigeIcon, IModHelper modHelper, object tokens = null) : base(skill, id)
        {
            this.Icon = icon;
            this.I18n = modHelper.Translation;
            this.Tokens = tokens;
            this.ModHelper = modHelper;

            if (prestigeIcon != null)
            {
                this.PrestigeIcon = prestigeIcon;
                this.NormalIcon = icon;

                modHelper.Events.Display.MenuChanged += this.DisplayEvents_MenuChanged_MARGO;
                modHelper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded_MARGO;
            }
        }

        private void GameLoop_SaveLoaded_MARGO(object sender, SaveLoadedEventArgs e)
        {
            if (Game1.player.HasProfession(this.Id, true))
            {
                this.Icon = this.PrestigeIcon;
                this.IsPrestiged = true;
            }
        }

        private void DisplayEvents_MenuChanged_MARGO(object sender, MenuChangedEventArgs e)
        {
            // After the upgrade selection menu, unset the prestige description and icon of the profession that wasn't chosen.
            if (e.OldMenu is SkillLevelUpMenu oldMenu && oldMenu.isProfessionChooser)
            {
                if (Game1.player.HasProfession(this.Id, true))
                {
                    return;
                }
                this.Icon = this.NormalIcon;
                this.IsPrestiged = false;
            }
        }

        public override string GetDescription()
        {
            if (this.CheckPrestigeMenu())
            {
                return this.I18n.Get($"profession.{this.Id}.pdesc", this.Tokens);
            }
            else
            {
                return this.I18n.Get($"profession.{this.Id}.desc", this.Tokens);
            }
        }

        private bool CheckPrestigeMenu()
        {
            if (!this.PrestigeEnabled)
            {
                return false;
            }
            if (this.IsPrestiged)
            {
                return true;
            }
            if (Game1.activeClickableMenu is not SkillLevelUpMenu currMenu)
            {
                return false;
            }
            if (!currMenu.isProfessionChooser)
            {
                return false;
            }
            string currSkill = this.ModHelper.Reflection.GetField<string>(currMenu, "currentSkill").GetValue();
            if (currSkill != this.Skill.Id)
            {
                return false;
            }
            int currentLevel = this.ModHelper.Reflection.GetField<int>(currMenu, "currentLevel").GetValue();
            if (currentLevel <= 10)
            {
                return false;
            }

            // All checks pass, we are in or after the prestiged skill select menu.
            // Set our description and icon to prestiged variants.
            // It's a bit weird to do this in GetDescription, but there's no earlier place.
            this.Icon = this.PrestigeIcon;
            this.IsPrestiged = true;

            return true;
        }

        public override string GetName()
        {
            return this.I18n.Get($"profession.{this.Id}.name", this.Tokens);
        }
    }
}
