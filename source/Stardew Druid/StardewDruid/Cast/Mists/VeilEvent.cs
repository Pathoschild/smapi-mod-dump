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
using StardewDruid.Cast;
using StardewDruid.Event;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;

namespace StardewDruid.Cast.Mists
{
    public class VeilEvent : EventHandle
    {

        public VeilEvent(Vector2 target)
          : base(target)
        {
            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 3.0;

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "veil");

            VeilEffect();

        }

        public void VeilEffect()
        {

            Vector2 mistCorner = targetPlayer.Position - new Vector2(96, 128);

            for (int i = 0; i < 4; i++)
            {

                for (int j = 0; j < 4; j++)
                {

                    if ((i == 0 || i == 5) && (j == 0 || j == 5))
                    {
                        continue;
                    }

                    Vector2 glowVector = mistCorner + new Vector2(i * 32, j * 32);

                    TemporaryAnimatedSprite glowSprite = new TemporaryAnimatedSprite(0, 3000f, 1, 1, glowVector, false, false)
                    {
                        sourceRect = new Rectangle(88, 1779, 30, 30),
                        sourceRectStartingPos = new Vector2(88, 1779),
                        texture = Game1.mouseCursors,
                        motion = new(0.016f * (randomIndex.Next(2) == 0 ? 1 : -1) * randomIndex.Next(1, 4), 0.016f * (randomIndex.Next(2) == 0 ? 1 : -1) * randomIndex.Next(1, 4)),
                        scale = 4f,
                        layerDepth = 999f,
                        timeBasedMotion = true,
                        alpha = 1f,
                        alphaFade = 0.0005f,
                        color = new Color(0.75f, 0.75f, 1f, 1f),
                    };

                    targetLocation.temporarySprites.Add(glowSprite);

                }

            }

            int num = Math.Min(Mod.instance.rite.caster.maxHealth / 10, Mod.instance.rite.caster.maxHealth - Mod.instance.rite.caster.health);

            if (num > 0)
            {

                Mod.instance.rite.caster.health += num;

                Rectangle healthBox = Mod.instance.rite.caster.GetBoundingBox();

                targetLocation.debris.Add(
                    new Debris(
                        num,
                        new Vector2(healthBox.Center.X + 16, healthBox.Center.Y),
                        Color.Green,
                        1f,
                        Mod.instance.rite.caster
                    )
                );

            }

        }

    }

}
