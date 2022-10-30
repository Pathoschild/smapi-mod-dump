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
using StardewModdingAPI;

namespace BirbShared
{
    internal class KeyedProfession : SpaceCore.Skills.Skill.Profession
    {
        readonly object Tokens;
        readonly ITranslationHelper I18n;
        public KeyedProfession(Skills.Skill skill, string id, Texture2D icon, ITranslationHelper i18n) : base(skill, id)
        {
            this.Icon = icon;
            this.I18n = i18n;
        }

        public KeyedProfession(SpaceCore.Skills.Skill skill, string id, Texture2D icon, ITranslationHelper i18n, object tokens) : base(skill, id)
        {
            this.Icon = icon;
            this.I18n = i18n;
            this.Tokens = tokens;
        }

        public override string GetDescription()
        {
            return this.I18n.Get($"{this.Id}.desc", this.Tokens);
        }

        public override string GetName()
        {
            return this.I18n.Get($"{this.Id}.name", this.Tokens);
        }
    }
}
