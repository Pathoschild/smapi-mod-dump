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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;


namespace StardewDruid.Cast.Fates
{
    internal class Enchant : CastHandle
    {

        public StardewValley.Object targetObject;

        public int source;

        public Enchant(Vector2 target, Rite rite, StardewValley.Object machine, int Source = 768)
            : base(target, rite)
        {
            castCost = 24;

            targetObject = machine;

            source = Source;

        }

        public override void CastEffect()
        {

            DelayedAction.functionAfterDelay(CastAfterDelay, 1000);

            ModUtility.AnimateFate(targetLocation, targetPlayer.getTileLocation(), targetVector, source);

            castFire = true;

        }

        public void CastAfterDelay()
        {

            if (!riteData.castTask.ContainsKey("masterEnchant"))
            {

                Mod.instance.UpdateTask("lessonEnchant", 1);

            }

            if (targetObject.MinutesUntilReady > 0 && riteData.castTask.ContainsKey("masterEnchant"))
            {

                Utility.addSprinklesToLocation(targetLocation, (int)targetVector.X, (int)targetVector.Y, 1, 2, 400, 40, Color.White);

                targetObject.MinutesUntilReady = 10;

                DelayedAction.functionAfterDelay(delegate { targetObject.minutesElapsed(10, targetLocation); }, 50);

            }
            else if (targetObject.heldObject.Value == null)
            {
                switch (targetObject.name)
                {

                    case "Deconstructor": FillDeconstructor(); break;

                    case "Bone Mill": FillBoneMill(); break;

                    case "Keg": FillKeg(); break;

                    case "Preserves Jar": FillPreservesJar(); break;

                    case "Cheese Press": FillCheesePress(); break;

                    case "Mayonnaise Machine": FillMayonnaiseMachine(); break;

                    case "Loom": FillLoom(); break;

                    case "Oil Maker": FillOilMaker(); break;

                    case "Furnace": FillFurnace(); break;

                    case "Geode Crusher": FillGeodeCrusher(); break;

                }

            }

        }

        public void FillDeconstructor()
        {

            KeyValuePair<string, string> craftingRecipe = CraftingRecipe.craftingRecipes.ElementAt(randomIndex.Next(CraftingRecipe.craftingRecipes.Count));

            string[] array = craftingRecipe.Value.Split('/')[0].Split(' ');

            List<StardewValley.Object> list = new();

            for (int i = 0; i < array.Count(); i += 2)
            {
                list.Add(new StardewValley.Object(Convert.ToInt32(array[i]), Convert.ToInt32(array[i + 1])));
            }

            if (list.Count == 0)
            {
                return;
            }

            list.Sort((StardewValley.Object a, StardewValley.Object b) => a.sellToStorePrice(-1L) * a.Stack - b.sellToStorePrice(-1L) * b.Stack);

            targetObject.heldObject.Value = list.Last();

            targetObject.MinutesUntilReady = 240;

            Game1.playSound("furnace");

        }

        public void FillBoneMill()
        {

            int num2 = -1;
            int num3 = 1;
            switch (Game1.random.Next(4))
            {
                case 0:
                    num2 = 466;
                    num3 = 3;
                    break;
                case 1:
                    num2 = 465;
                    num3 = 5;
                    break;
                case 2:
                    num2 = 369;
                    num3 = 10;
                    break;
                case 3:
                    num2 = 805;
                    num3 = 5;
                    break;
            }

            if (Game1.random.NextDouble() < 0.1)
            {
                num3 *= 2;
            }

            targetObject.heldObject.Value = new StardewValley.Object(num2, num3);

            targetObject.MinutesUntilReady = 240;

            targetLocation.playSound("skeletonStep");

            DelayedAction.playSoundAfterDelay("skeletonHit", 150);

        }

        public void FillKeg()
        {

            Dictionary<int, int> cropList = new()
            {

                [0] = 24,
                [1] = 188,
                [2] = 190,
                [3] = 248,
                [4] = 250,
                [5] = 256,
                [6] = 264,
                [7] = 266,
                [8] = 272,
                [9] = 274,
                [10] = 276,
                [11] = 278,
                [12] = 280,
                [13] = 284,
                [14] = 300,
                [15] = 304,
                [16] = 830,
                [17] = 259,
                [18] = 270,
                [19] = 486,
                [20] = 262,
                [21] = 304,
                [22] = 815,
                [23] = 340,
                [24] = 91,
                [25] = 832,
                [26] = 834,
                [27] = 634,
                [28] = 635,
                [29] = 636,
                [30] = 637,
                [31] = 638,
                [32] = 613,
                [33] = 400,
                [34] = 398,
                [35] = 282,
                [36] = 260,
                [37] = 258,
                [38] = 254,
                [39] = 252,
                [40] = 88,
                [41] = 90

            };

            int cropIndex = cropList[randomIndex.Next(cropList.Count)];

            StardewValley.Object input = new(cropIndex, 0);

            if (input == null) { targetLocation.playSound("ghost"); return; }

            switch (@input.ParentSheetIndex)
            {
                case 262:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 346, "Beer", canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                    targetObject.heldObject.Value.name = "Beer";
                    break;
                case 304:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 303, "Pale Ale", canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                    targetObject.heldObject.Value.name = "Pale Ale";
                    break;
                case 815:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 614, "Green Tea", canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                    targetObject.heldObject.Value.name = "Green Tea";
                    break;
                case 433:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 395, "Coffee", canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                    targetObject.heldObject.Value.name = "Coffee";
                    break;
                case 340:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 459, "Mead", canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                    targetObject.heldObject.Value.name = "Mead";
                    break;

            }

            if (targetObject.heldObject.Value == null)
            {
                
                switch (@input.Category)
                {
                    case -75:
                        targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 350, @input.Name + " Juice", canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                        targetObject.heldObject.Value.Price = (int)(@input.Price * 2.25);
                        targetObject.heldObject.Value.name = @input.Name + " Juice";
                        targetObject.heldObject.Value.preserve.Value = StardewValley.Object.PreserveType.Juice;
                        targetObject.heldObject.Value.preservedParentSheetIndex.Value = @input.ParentSheetIndex;
                        targetObject.MinutesUntilReady = 6000;
                        break;
                    case -79:
                        targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 348, @input.Name + " Wine", canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                        targetObject.heldObject.Value.Price = @input.Price * 3;
                        targetObject.heldObject.Value.name = @input.Name + " Wine";
                        targetObject.heldObject.Value.preserve.Value = StardewValley.Object.PreserveType.Wine;
                        targetObject.heldObject.Value.preservedParentSheetIndex.Value = @input.ParentSheetIndex;
                        targetObject.MinutesUntilReady = 10000;
                        break;
                    default:
                        targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 346, "Beer", canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                        targetObject.heldObject.Value.name = "Beer";
                        break;

                }

            }

            if (targetObject.heldObject.Value == null)
            {
                targetLocation.playSound("ghost");
                return;
            }


            targetLocation.playSound("Ship");

            targetLocation.playSound("bubbles");

            TemporaryAnimatedSprite temporarySprite = new("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, targetVector * 64f + new Vector2(0f, -128f), flicker: false, flipped: false, (targetVector.Y + 1f) * 64f / 10000f + 0.0001f, 0f, Color.Yellow * 0.75f, 1f, 0f, 0f, 0f)
            {
                alphaFade = 0.005f
            };

            targetLocation.temporarySprites.Add(temporarySprite);

            targetObject.MinutesUntilReady = 1000;

        }

        public void FillPreservesJar()
        {

            Dictionary<int, int> cropList = new()
            {

                [0] = 24,
                [1] = 188,
                [2] = 190,
                [3] = 248,
                [4] = 250,
                [5] = 256,
                [6] = 264,
                [7] = 266,
                [8] = 272,
                [9] = 274,
                [10] = 276,
                [11] = 278,
                [12] = 280,
                [13] = 284,
                [14] = 300,
                [15] = 304,
                [16] = 830,
                [17] = 259,
                [18] = 270,
                [19] = 486,
                [20] = 262,
                [21] = 304,
                [22] = 91,
                [23] = 832,
                [24] = 834,
                [25] = 634,
                [26] = 635,
                [27] = 636,
                [28] = 637,
                [29] = 638,
                [30] = 613,
                [31] = 400,
                [32] = 398,
                [33] = 282,
                [34] = 260,
                [35] = 258,
                [36] = 254,
                [37] = 252,
                [38] = 88,
                [39] = 90

            };

            int cropIndex = cropList[randomIndex.Next(cropList.Count)];

            StardewValley.Object input = new(cropIndex, 0);

            if (input == null) { targetLocation.playSound("ghost"); return; }

            switch (@input.Category)
            {
                case -75:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 342, "Pickled " + @input.Name, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                    targetObject.heldObject.Value.Price = 50 + @input.Price * 2;
                    targetObject.heldObject.Value.name = "Pickled " + @input.Name;
                    targetObject.heldObject.Value.preserve.Value = StardewValley.Object.PreserveType.Pickle;
                    targetObject.heldObject.Value.preservedParentSheetIndex.Value = @input.ParentSheetIndex;
                    break;
                case -79:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 344, @input.Name + " Jelly", canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                    targetObject.heldObject.Value.Price = 50 + @input.Price * 2;
                    targetObject.heldObject.Value.name = @input.Name + " Jelly";
                    targetObject.heldObject.Value.preserve.Value = StardewValley.Object.PreserveType.Jelly;
                    targetObject.heldObject.Value.preservedParentSheetIndex.Value = @input.ParentSheetIndex;
                    break;
            }

            if(targetObject.heldObject.Value == null)
            {
                targetLocation.playSound("ghost");
                return;

            }

            targetLocation.playSound("Ship");

            targetLocation.playSound("bubbles");

            TemporaryAnimatedSprite temporarySprite = new("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, targetVector * 64f + new Vector2(0f, -128f), flicker: false, flipped: false, (targetVector.Y + 1f) * 64f / 10000f + 0.0001f, 0f, Color.Yellow * 0.75f, 1f, 0f, 0f, 0f)
            {
                alphaFade = 0.005f
            };

            targetLocation.temporarySprites.Add(temporarySprite);

            targetObject.MinutesUntilReady = 4000;

        }

        public void FillCheesePress()
        {

            targetLocation.playSound("Ship");

            targetObject.MinutesUntilReady = 240;

            switch (randomIndex.Next(2))
            {
                case 0:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 426, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                    break;
                default:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 424, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                    break;
            }

        }

        public void FillMayonnaiseMachine()
        {

            List<int> eggList = new()
            {

                289,
                174,
                182,
                176,
                180,
                442,
                305,
                107,
                928,

            };

            int eggIndex = eggList[randomIndex.Next(eggList.Count)];

            targetLocation.playSound("Ship");

            targetObject.MinutesUntilReady = 240;

            switch (eggIndex)
            {
                case 289:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 306, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                    targetObject.heldObject.Value.Stack = 10;
                    targetObject.heldObject.Value.Quality = 2;

                    break;

                case 174:
                case 182:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 306, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false)
                    {
                        Quality = 2
                    };

                    break;

                case 176:
                case 180:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 306, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);

                    break;

                case 442:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 307, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);

                    break;

                case 305:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 308, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);

                    break;

                case 107:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 807, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);

                    break;

                case 928:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 306, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false)
                    {
                        Quality = 2
                    };
                    targetObject.heldObject.Value.Stack = 3;

                    break;
            }


        }

        public void FillLoom()
        {

            targetLocation.playSound("Ship");

            targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 428, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false)
            {
                Stack = 2
            };
            targetObject.MinutesUntilReady = 240;

        }

        public void FillOilMaker()
        {

            List<int> oilList = new()
            {

                270,
                421,
                430,
                431,

            };

            int oilIndex = oilList[randomIndex.Next(oilList.Count)];

            targetLocation.playSound("bubbles");

            targetLocation.playSound("sipTea");

            TemporaryAnimatedSprite temporarySprite = new("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, targetVector * 64f + new Vector2(0f, -128f), flicker: false, flipped: false, (targetVector.Y + 1f) * 64f / 10000f + 0.0001f, 0f, Color.Yellow * 0.75f, 1f, 0f, 0f, 0f)
            {
                alphaFade = 0.005f
            };

            targetLocation.temporarySprites.Add(temporarySprite);

            targetObject.MinutesUntilReady = 1000;

            switch (oilIndex)
            {
                case 270:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 247, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                    break;

                case 421:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 247, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                    break;

                case 430:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 432, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                    break;

                case 431:
                    targetObject.heldObject.Value = new StardewValley.Object(247, 1);
                    break;

            }

        }

        public void FillFurnace()
        {

            List<int> furnaceList = new()
            {

                378,
                380,
                384,
                386,
                80,
                82,
                909,

            };

            int furnaceIndex = furnaceList[randomIndex.Next(furnaceList.Count)];

            switch (furnaceIndex)
            {
                case 378:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 334, 1);

                    break;
                case 380:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 335, 1);

                    break;
                case 384:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 336, 1);

                    break;
                case 386:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 337, 1);

                    break;
                case 80:
                    targetObject.heldObject.Value = new StardewValley.Object(Vector2.Zero, 338, "Refined Quartz", canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);

                    break;
                case 82:
                    targetObject.heldObject.Value = new StardewValley.Object(338, 3);

                    break;
                case 909:
                    targetObject.heldObject.Value = new StardewValley.Object(910, 1);

                    break;
            }

            targetLocation.playSound("furnace");

            targetObject.initializeLightSource(targetVector);

            targetObject.showNextIndex.Value = true;

            targetObject.MinutesUntilReady = 240;

        }

        public void FillGeodeCrusher()
        {

            targetObject.heldObject.Value = (StardewValley.Object)Utility.getTreasureFromGeode(new StardewValley.Object(749, 1));

            Game1.stats.GeodesCracked++;

            targetObject.MinutesUntilReady = 240;

            Game1.playSound("drumkit4");

            Game1.playSound("stoneCrack");

            DelayedAction.playSoundAfterDelay("steam", 200);

        }

    }

}
