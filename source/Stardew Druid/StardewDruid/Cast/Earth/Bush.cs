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
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace StardewDruid.Cast.Earth
{
    internal class Bush : CastHandle
    {

        private readonly StardewValley.TerrainFeatures.Bush bushFeature;

        public Bush(Vector2 target, Rite rite, StardewValley.TerrainFeatures.Bush bush)
            : base(target, rite)
        {


            castCost = 6;

            if (rite.caster.ForagingLevel >= 8)
            {

                castCost = 4;

            }

            bushFeature = bush;

        }

        public override void CastEffect()
        {

            if (randomIndex.Next(20) == 0 && riteData.spawnIndex["wildspawn"] && !riteData.castToggle.ContainsKey("forgetWildspawn"))
            {

                StardewValley.Monsters.Monster spawnMonster = Mod.instance.SpawnMonster(targetLocation, targetVector, new() { 99, }, "bush");

                if (!riteData.castTask.ContainsKey("masterCreature") && spawnMonster != null)
                {

                    Mod.instance.UpdateTask("lessonCreature", 1);

                }

            }

            bushFeature.performToolAction(null, 1, targetVector, null);

            int probability = randomIndex.Next(25 - riteData.caster.ForagingLevel);

            if (probability > 1)
            {
                return;
            }

            int objectIndex;

            if (probability == 0)
            {

                switch (Game1.currentSeason)
                {

                    case "spring":

                        objectIndex = 296; // salmonberry

                        break;

                    case "summer":

                        objectIndex = 398; // grape

                        break;

                    case "fall":

                        objectIndex = 410; // blackberry

                        break;

                    default:

                        objectIndex = 414; // crystal fruit

                        break;

                }

            }
            else
            {

                Dictionary<int, int> objectIndexes = new()
                {
                    [0] = 257, // 257 morel
                    [1] = 257, // 257 morel
                    [2] = 281, // 281 chanterelle
                    [3] = 404, // 404 mushroom
                    [4] = 404, // 404 mushroom

                };

                objectIndex = objectIndexes[randomIndex.Next(5)];

            }

            int randomQuality = randomIndex.Next(11 - targetPlayer.foragingLevel.Value);

            int objectQuality = 0;

            if (randomQuality == 0)
            {

                objectQuality = 2;

            }

            if (targetPlayer.professions.Contains(16))
            {

                objectQuality = 3;

            }

            int throwAmount = 1;

            if (targetPlayer.professions.Contains(13))
            {

                throwAmount = randomIndex.Next(1, 3);

            }

            for (int i = 0; i < throwAmount; i++)
            {

                Throw throwObject = new(targetPlayer, targetVector * 64, objectIndex, objectQuality);

                throwObject.ThrowObject();

            };

            castFire = true;

            ModUtility.AnimateGrowth(targetLocation, targetVector, new(0.8f, 1, 0.8f, 1));

            targetPlayer.gainExperience(2, 2); // gain foraging experience

            bushFeature.performToolAction(null, 1, targetVector, null);

            if (Game1.currentSeason == "summer")
            {

                Game1.currentLocation.critters.Add(new Firefly(targetVector + new Vector2(randomIndex.Next(-2, 3), randomIndex.Next(-2, 3))));

                Game1.currentLocation.critters.Add(new Firefly(targetVector + new Vector2(randomIndex.Next(-2, 3), randomIndex.Next(-2, 3))));

            }
            else
            {

                Game1.currentLocation.critters.Add(new Butterfly(targetVector + new Vector2(randomIndex.Next(-2, 3), randomIndex.Next(-2, 3)), false));

                Game1.currentLocation.critters.Add(new Butterfly(targetVector + new Vector2(randomIndex.Next(-2, 3), randomIndex.Next(-2, 3)), false));

            }

        }

    }
}
