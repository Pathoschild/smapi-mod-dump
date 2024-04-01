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
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Cast.Weald;
using StardewDruid.Event;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.IO;


namespace StardewDruid.Cast.Stars
{
    internal class Meteor : CastHandle
    {

        float damage;

        bool comet;

        public Meteor(Vector2 target,  float Damage, bool Comet = false)
            : base(target)
        {

            castCost = Math.Max(6, 14 - Game1.player.CombatLevel);

            damage = Damage;

            comet = Comet;

        }

        public override void CastEffect()
        {

            if (!Mod.instance.TaskList().ContainsKey("masterMeteor"))
            {

                List<StardewValley.Monsters.Monster> monsters = ModUtility.MonsterProximity(targetLocation, new() { targetVector * 64, }, 3, true);

                for (int i = monsters.Count - 1; i >= 0; i--)
                {

                    Mod.instance.UpdateTask("lessonMeteor", 1);

                }

            }

            int radius = 3;

            int power = 3;

            int environment = 3;

            int terrain = 0;

            if (comet)
            {

                TemporaryAnimatedSprite startAnimation = new(0, 2000f, 1, 1, targetVector * 64 - new Vector2(128, 128), false, false)
                {

                    sourceRect = new(128, 0, 64, 64),

                    sourceRectStartingPos = new Vector2(128, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Decorations.png")),

                    scale = 5f,

                    layerDepth = 0.0001f,

                    rotationChange = 0.06f,

                    timeBasedMotion = true,

                    alpha = 0.75f,

                };

                targetLocation.temporarySprites.Add(startAnimation);

                radius = 5;

                damage *= 4;

                power = 4;

                environment = 8;

                terrain = 5;

            }

            SpellHandle meteor = new(targetLocation, targetVector*64, Game1.player.Position, radius, 1, -1, damage, power);

            meteor.environment = environment;

            meteor.terrain = terrain;

            meteor.type = SpellHandle.barrages.meteor;

            Mod.instance.spellRegister.Add(meteor);

            castFire = true;

        }

    }

}
