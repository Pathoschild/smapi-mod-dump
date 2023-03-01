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
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuneMagic.Source.Effects
{
    public class Summon : SpellEffect
    {
        private Monster SummonedCreature;

        public Summon(Spell spell, Monster summonedCreature) : base(spell, Duration.Short)
        {
            SummonedCreature = summonedCreature;
            Start();
        }

        public override void Start()
        {
            base.Start();
            Game1.currentLocation.characters.Add(SummonedCreature);
            SummonedCreature.DamageToFarmer = 0;

            RuneMagic.Instance.Monitor.Log($"Summoned creature at {SummonedCreature.position.X}, {SummonedCreature.position.Y}", StardewModdingAPI.LogLevel.Debug);
        }

        public override void End()
        {
            Game1.currentLocation.characters.Remove(SummonedCreature);
            base.End();
        }

        public override void Update()
        {
            base.Update();
            //get the closest monster to the summoned creature that its not itself
            var closestMonster = Game1.currentLocation.characters.OfType<Monster>().Where(m => m != SummonedCreature).OrderBy(m => Vector2.Distance(m.position, SummonedCreature.position)).FirstOrDefault();
            if (closestMonster is null)
                return;
            SummonedCreature.focusedOnFarmers = false;
            SummonedCreature.moveTowardPlayerThreshold.Value = 0;
            SummonedCreature.faceGeneralDirection(closestMonster.position);
            SummonedCreature.tryToMoveInDirection(SummonedCreature.FacingDirection, false, 0, false);
            if (Vector2.Distance(closestMonster.position, SummonedCreature.position) < 64)
                Game1.currentLocation.damageMonster(new Rectangle(closestMonster.GetBoundingBox().X, closestMonster.GetBoundingBox().Y, 64, 64), 1, 1, false, Game1.player);

            RuneMagic.Instance.Monitor.Log($"Facing direction: {SummonedCreature.FacingDirection}", StardewModdingAPI.LogLevel.Debug);
        }
    }
}