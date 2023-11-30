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
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using System;
using System.ComponentModel.Design;
using static StardewValley.Debris;

namespace StardewDruid.Cast.Earth
{
    internal class Grass : CastHandle
    {

        public Grass(Vector2 target, Rite rite)
            : base(target, rite)
        {

            castCost = 0;

        }

        public override void CastEffect()
        {

            if (!targetLocation.terrainFeatures.ContainsKey(targetVector))
            {

                return;

            }

            if (targetLocation.terrainFeatures[targetVector] is not StardewValley.TerrainFeatures.Grass)
            {

                return;

            }

            int probability = randomIndex.Next(2);

            List<Throw> throwList = new();

            if (randomIndex.Next(100) == 0) // 1:1000 chance
            {

                throwList.Add(new(targetPlayer, targetVector * 64, 114, 0));

            }

            if (probability == 0 && !riteData.castToggle.ContainsKey("forgetSeeds") && riteData.castTask.ContainsKey("masterCreature"))
            {

                int wildSeed;

                switch (Game1.currentSeason)
                {

                    case "spring":

                        wildSeed = 495;
                        break;

                    case "summer":

                        wildSeed = 496;
                        break;

                    case "fall":

                        wildSeed = 497;
                        break;

                    default:

                        wildSeed = 498;

                        break;

                }

                for (int i = 0; i < randomIndex.Next(4); i++)
                {

                    throwList.Add(new(targetPlayer, targetVector * 64, wildSeed, 0));

                }

            }
            else
            {

                for (int i = 0; i < randomIndex.Next(4); i++)
                {

                    throwList.Add(new(targetPlayer, targetVector * 64, 771, 0));

                }

            }

            for (int i = 0; i < throwList.Count; i++)
            {

                throwList[i].ThrowObject();

            }

        }

    }
}
