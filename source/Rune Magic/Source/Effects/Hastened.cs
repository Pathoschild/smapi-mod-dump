/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Minigames.TargetGame;

namespace RuneMagic.Source.Effects
{
    public class Hastened : SpellEffect
    {
        public Hastened(Spell spell) : base(spell, Duration.Short)
        {
            Start();
        }

        public override void Start()
        {
            base.Start();
            Game1.player.addedSpeed = 5;
        }

        public override void End()
        {
            Game1.player.addedSpeed = 0;
            base.End();
        }
    }
}