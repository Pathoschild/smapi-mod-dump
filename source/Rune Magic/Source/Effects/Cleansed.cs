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
    public class Cleansed : SpellEffect
    {
        public Cleansed(Spell spell) : base(spell, Duration.Short)
        {
            Start();
        }

        public override void Update()
        {
            base.Update();
            Game1.buffsDisplay.clearAllBuffs();
        }
    }
}