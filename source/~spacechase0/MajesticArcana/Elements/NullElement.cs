/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MajesticArcana.Elements
{
    // Basically a filler for the elements we use later
    internal class NullElement : Element
    {
        public NullElement(string id, Rectangle texRect, Color color, string[] parentIds = null)
        :   base(id, parentIds)
        {
            TextureRect = texRect;
            Color = color;
        }

        public override Rectangle TextureRect { get; }

        public override Color Color { get; }

        public override void ApplyAttribute(Character caster, Character target)
        {
            // ...
        }

        public override void Manifest(Character caster, Vector2 castDir, float strength)
        {
            // ...
        }
    }
}
