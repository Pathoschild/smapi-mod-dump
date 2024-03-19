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
        readonly object _tokens;
        readonly ITranslationHelper _i18N;

        bool PrestigeEnabled => this._prestigeIcon != null;
        readonly Texture2D _prestigeIcon;
        readonly Texture2D _normalIcon;
        private bool _isPrestiged;
        readonly IModHelper _modHelper;

        public KeyedProfession(Skills.Skill skill, string id, Texture2D icon, Texture2D prestigeIcon,
            IModHelper modHelper, object tokens = null) : base(skill, id)
        {
            this.Icon = icon;
            this._i18N = modHelper.Translation;
            this._tokens = tokens;
            this._modHelper = modHelper;

            if (prestigeIcon == null)
            {
                return;
            }

            this._prestigeIcon = prestigeIcon;
            this._normalIcon = icon;

            modHelper.Events.Display.MenuChanged += this.DisplayEvents_MenuChanged_MARGO;
            modHelper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded_MARGO;
        }

        private void GameLoop_SaveLoaded_MARGO(object sender, SaveLoadedEventArgs e)
        {
            if (!Game1.player.HasProfession(this.Id, true))
            {
                return;
            }

            this.Icon = this._prestigeIcon;
            this._isPrestiged = true;
        }

        private void DisplayEvents_MenuChanged_MARGO(object sender, MenuChangedEventArgs e)
        {
            // After the upgrade selection menu, unset the prestige description and icon of the profession that wasn't chosen.
            if (e.OldMenu is not SkillLevelUpMenu { isProfessionChooser: true })
            {
                return;
            }

            if (Game1.player.HasProfession(this.Id, true))
            {
                return;
            }

            this.Icon = this._normalIcon;
            this._isPrestiged = false;
        }

        public override string GetDescription()
        {
            return this._i18N.Get(
                this.CheckPrestigeMenu() ? $"profession.{this.Id}.pdesc" : $"profession.{this.Id}.desc", this._tokens);
        }

        private bool CheckPrestigeMenu()
        {
            if (!this.PrestigeEnabled)
            {
                return false;
            }

            if (this._isPrestiged)
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

            string currSkill = this._modHelper.Reflection.GetField<string>(currMenu, "currentSkill").GetValue();
            if (currSkill != this.Skill.Id)
            {
                return false;
            }

            int currentLevel = this._modHelper.Reflection.GetField<int>(currMenu, "currentLevel").GetValue();
            if (currentLevel <= 10)
            {
                return false;
            }

            // All checks pass, we are in or after the prestiged skill select menu.
            // Set our description and icon to prestiged variants.
            // It's a bit weird to do this in GetDescription, but there's no earlier place.
            this.Icon = this._prestigeIcon;
            this._isPrestiged = true;

            return true;
        }

        public override string GetName()
        {
            return this._i18N.Get($"profession.{this.Id}.name", this._tokens);
        }
    }
}
