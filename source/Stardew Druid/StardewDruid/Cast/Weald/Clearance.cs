/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley;
using System;
using System.Collections.Generic;
using StardewDruid.Data;
using StardewDruid.Journal;

namespace StardewDruid.Cast.Weald
{
    public class Clearance
    {

        public Clearance()
        {

        }

        public void CastActivate(Vector2 target, float damage, bool sound = true)
        {

            if (!Mod.instance.questHandle.IsComplete(QuestHandle.wealdOne))
            {

                Mod.instance.questHandle.UpdateTask(QuestHandle.wealdOne, 1);

            }

            int radius = 2 + Mod.instance.PowerLevel;

            SpellHandle explode = new(Game1.player, target * 64, radius * 64, (int)(damage * 0.25));

            explode.type = SpellHandle.spells.explode;

            if (sound)
            {
                explode.sound = SpellHandle.sounds.flameSpellHit;
            }

            explode.display = IconData.impacts.impact;

            explode.indicator = IconData.cursors.weald;

            explode.projectile = 3;

            explode.power = 2;

            explode.environment = radius;

            Mod.instance.spellRegister.Add(explode);

        }

    }

}
