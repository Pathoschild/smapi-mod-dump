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
using StardewDruid.Event;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley;
using System;
using System.Collections.Generic;
using StardewDruid.Data;
using StardewDruid.Journal;

namespace StardewDruid.Cast.Weald
{
    internal class Clear : CastHandle
    {

        public float damage;

        public Clear(Vector2 target,  float Damage)
            : base(target)
        {

            castCost = 1;

            damage = Damage;

        }

        public override void CastEffect()
        {
            
            if (!Mod.instance.questHandle.IsComplete(QuestHandle.clearLesson))
            {

                Mod.instance.questHandle.UpdateTask(QuestHandle.clearLesson, 1);

            }

            int radius = 2 + Mod.instance.PowerLevel;

            SpellHandle explode = new(Game1.player, targetVector * 64, radius * 64, (int)(damage * 0.25));

            explode.type = SpellHandle.spells.explode;

            explode.sound = SpellHandle.sounds.flameSpellHit;

            explode.display = SpellHandle.displays.Impact;

            explode.indicator = IconData.cursors.weald;

            explode.power = 2;

            explode.environment = radius;

            Mod.instance.spellRegister.Add(explode);

            castFire = true;

        }

    }

}
