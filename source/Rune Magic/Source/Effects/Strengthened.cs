/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneMagic.Source.Effects
{
    public class Strengthened : SpellEffect
    {
        public Strengthened(Spell spell) : base(spell, Duration.Medium)
        {
            Start();
        }

        public override void Start()
        {
            base.Start();
            Game1.player.attackIncreaseModifier = 5;
        }

        public override void End()
        {
            Game1.player.attackIncreaseModifier = 0;
            base.End();
        }
    }
}