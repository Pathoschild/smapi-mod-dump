/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using MagicSkillCode.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonShared;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;

namespace MagicSkillCode.Objects
{
    public class UpgradePointProfession : KeyedProfession
    {
        /*********
        ** Public methods
        *********/
        public UpgradePointProfession(Skills.Skill skill, string theId, Texture2D icon, ITranslationHelper i18n)
            : base(skill, theId, icon, i18n) { }

        public override void DoImmediateProfessionPerk()
        {
            Game1.player.GetSpellBook().UseSpellPoints(-2);
        }
    }
}
