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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpaceCore.Skills;

namespace RuneMagic.Source.Interface
{
    public abstract class SpellSlot
    {
        public Spell Spell { get; set; }
        public Texture2D Icon { get; set; }
        public Texture2D ButtonTexture { get; set; }
        public bool Selected { get; set; } = false;
        public Rectangle Rectangle { get; set; }
        public Color Color { get; set; }

        public SpellSlot(Spell spell, Rectangle rectangle)
        {
            Spell = spell;
            if (spell != null)
                Icon = spell.Icon;
            else
                Icon = null;
            Rectangle = rectangle;
            Color = Color.White;
        }

        public abstract void SetButtonTexture();

        public override string ToString()
        {
            return $"Spell: {Spell}, Coordenates: {Rectangle.X}, {Rectangle.Y}, Size: {Rectangle.Size.X}, {Rectangle.Size.Y}";
        }
    }
}