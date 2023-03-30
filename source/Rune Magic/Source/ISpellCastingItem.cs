/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Xml.Serialization;

namespace RuneMagic.Source
{
    public interface ISpellCastingItem
    {
        [XmlIgnore]
        public Spell Spell { get; set; }

        public void InitializeSpell();

        public virtual void Activate()
        {
            Player.MagicStats.ActiveSchool.Experience += 5;
        }

        public void Update();
    }
}