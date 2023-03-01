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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneMagic.Source.Effects
{
    public class Warded : SpellEffect
    {
        public Warded(Spell spell) : base(spell, Duration.Short)
        {
            Start();
        }

        public override void Start()
        {
            Game1.buffsDisplay.addOtherBuff(new Buff(21)
            {
                which = 15065,
                millisecondsDuration = 999999,
                sheetIndex = 29,
                glow = Color.Gray
            });
            base.Start();
        }

        public override void End()
        {
            Game1.buffsDisplay.removeOtherBuff(15065);
            base.End();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}