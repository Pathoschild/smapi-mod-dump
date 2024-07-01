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
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewDruid.Journal;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using static StardewDruid.Journal.HerbalData;


namespace StardewDruid.Cast.Fates
{
    internal class Enchant : Event.EventHandle
    {


        public int radialCounter = 0;

        public int faeth = 0;

        public Enchant()
        {

        }

        public override bool EventActive()
        {

            if (!inabsentia && !eventLocked)
            {

                if (Mod.instance.Config.riteButtons.GetState() != SButtonState.Held)
                {

                    return false;

                }

                if (Vector2.Distance(origin, Game1.player.Position) > 32)
                {

                    return false;

                }

            }

            return base.EventActive();

        }

        public override void EventDecimal()
        {

            if (!EventActive())
            {

                RemoveAnimations();

                return;

            }

            if (!inabsentia && !eventLocked)
            {

                decimalCounter++;

                if (decimalCounter == 5)
                {

                    TemporaryAnimatedSprite skyAnimation = Mod.instance.iconData.SkyIndicator(location, origin, IconData.skies.temple, 1f, new() { interval = 1000, });

                    skyAnimation.scaleChange = 0.002f;

                    skyAnimation.motion = new(-0.064f, -0.064f);

                    skyAnimation.timeBasedMotion = true;

                    animations.Add(skyAnimation);

                }

                if (decimalCounter == 15)
                {

                    eventLocked = true;

                    SpellHandle spellHandle = new(origin, 320, IconData.impacts.nature, new());

                    spellHandle.scheme = IconData.schemes.fates;

                    Mod.instance.spellRegister.Add(spellHandle);

                }

                return;

            }

            if(radialCounter % 2 == 0)
            {

                Enchantment();

            }

            radialCounter++;

            if (radialCounter == 18)
            {

                eventComplete = true;

            }

        }
        public override void EventRemove()
        {
            
            base.EventRemove();

            if (faeth > 0)
            {

                string message = "Consumed " + faeth + " Faeth";

                ConsumePotion hudmessage = new(message, Mod.instance.herbalData.herbalism[herbals.faeth.ToString()]);

                Game1.addHUDMessage(hudmessage);

                Mod.instance.rite.castCost *= faeth * 12;

                Mod.instance.rite.ApplyCost();
            
            }
  
        }

        public void Enchantment()
        {

            if (!Mod.instance.rite.targetCasts.ContainsKey(location.Name))
            {

                Mod.instance.rite.targetCasts[location.Name] = new();

            }

            List<Vector2> affected = ModUtility.GetTilesWithinRadius(location, ModUtility.PositionToTile(origin), radialCounter/2);

            foreach (Vector2 tile in affected)
            {

                if (Mod.instance.save.herbalism[HerbalData.herbals.faeth] == 0)
                {

                    break;

                }

                if (Mod.instance.rite.targetCasts[location.Name].ContainsKey(tile))
                {

                    continue;

                }

                if (!location.objects.ContainsKey(tile))
                {

                    continue;

                }

                if (!location.objects[tile].HasTypeBigCraftable())
                {

                    continue;

                }

                StardewValley.Object target = location.objects[tile];

                if (target.MinutesUntilReady > 0)
                {

                    Utility.addSprinklesToLocation(location, (int)tile.X, (int)tile.Y, 1, 2, 400, 40, Color.White);

                    target.MinutesUntilReady = 10;

                    DelayedAction.functionAfterDelay(delegate { target.minutesElapsed(10); }, 50);

                    Mod.instance.rite.targetCasts[location.Name][tile] = target.name;

                    Mod.instance.save.herbalism[HerbalData.herbals.faeth] -= 1;

                    Vector2 cursorVector = tile * 64 + new Vector2(32, 32);

                    Microsoft.Xna.Framework.Color colour = Mod.instance.iconData.gradientColours[IconData.schemes.fates][Mod.instance.randomIndex.Next(3)];

                    Mod.instance.iconData.ImpactIndicator(location, cursorVector, IconData.impacts.glare, 0.75f + Mod.instance.randomIndex.Next(5) * 0.25f, new() { color = colour, });

                    faeth++;

                }
                else 
                {
                    if (target.heldObject.Value != null)
                    {

                        ThrowHandle throwIt = new(Game1.player, target.TileLocation * 64, target.heldObject.Value);

                        throwIt.register();

                        target.heldObject.Set(null);

                    }

                    switch (target.name)
                    {

                        case "Deconstructor": FillDeconstructor(target); break;

                        case "Bone Mill": FillBoneMill(target); break;

                        case "Keg": FillKeg(target); break;

                        case "Preserves Jar": FillPreservesJar(target); break;

                        case "Cheese Press": FillCheesePress(target); break;

                        case "Mayonnaise Machine": FillMayonnaiseMachine(target); break;

                        case "Loom": FillLoom(target); break;

                        case "Oil Maker": FillOilMaker(target); break;

                        case "Furnace": FillFurnace(target); break;

                        case "Geode Crusher": FillGeodeCrusher(target); break;

                        default:

                            continue;

                    }

                    Mod.instance.rite.targetCasts[location.Name][tile] = target.name;

                    Mod.instance.save.herbalism[HerbalData.herbals.faeth] -= 1;

                    if (!Mod.instance.questHandle.IsComplete(QuestHandle.fatesThree))
                    {

                        Mod.instance.questHandle.UpdateTask(QuestHandle.fatesThree, 1);

                    }
                    Vector2 cursorVector = tile * 64 + new Vector2(32, 32);

                    Microsoft.Xna.Framework.Color colour = Mod.instance.iconData.gradientColours[IconData.schemes.fates][Mod.instance.randomIndex.Next(3)];

                    Mod.instance.iconData.ImpactIndicator(location, cursorVector, IconData.impacts.glare, 0.75f + Mod.instance.randomIndex.Next(5) * 0.25f, new() { color = colour, });

                    faeth++;

                }
 
            }

        }

        public void FillDeconstructor(StardewValley.Object targetObject)
        {

            KeyValuePair<string, string> craftingRecipe = CraftingRecipe.craftingRecipes.ElementAt(Mod.instance.randomIndex.Next(CraftingRecipe.craftingRecipes.Count));

            string[] array = craftingRecipe.Value.Split('/')[0].Split(' ');

            List<StardewValley.Object> list = new();

            for (int i = 0; i < array.Count(); i += 2)
            {
                list.Add(new StardewValley.Object(array[i], Convert.ToInt32(array[i + 1])));
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

        public void FillBoneMill(StardewValley.Object targetObject)
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

            targetObject.heldObject.Value = new StardewValley.Object(num2.ToString(), num3);

            targetObject.MinutesUntilReady = 240;

            location.playSound("skeletonStep");

            DelayedAction.playSoundAfterDelay("skeletonHit", 150);

        }

        public void FillKeg(StardewValley.Object targetObject)
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

            int cropIndex = cropList[Mod.instance.randomIndex.Next(cropList.Count)];

            StardewValley.Object input = new(cropIndex.ToString(), 0);

            if (input == null) { location.playSound("ghost"); return; }

            switch (@input.ParentSheetIndex)
            {
                case 262:
                    targetObject.heldObject.Value = new StardewValley.Object("346",1);
                    targetObject.heldObject.Value.name = "Beer";
                    break;
                case 304:
                    targetObject.heldObject.Value = new StardewValley.Object("303", 1);
                    targetObject.heldObject.Value.name = "Pale Ale";
                    break;
                case 815:
                    targetObject.heldObject.Value = new StardewValley.Object("614", 1);
                    targetObject.heldObject.Value.name = "Green Tea";
                    break;
                case 433:
                    targetObject.heldObject.Value = new StardewValley.Object("395", 1);
                    targetObject.heldObject.Value.name = "Coffee";
                    break;
                case 340:
                    targetObject.heldObject.Value = new StardewValley.Object("459", 1);
                    targetObject.heldObject.Value.name = "Mead";
                    break;

            }

            if (targetObject.heldObject.Value == null)
            {
                
                switch (@input.Category)
                {
                    case -75:
                        targetObject.heldObject.Value = new StardewValley.Object("350", 1);
                        targetObject.heldObject.Value.Price = (int)(@input.Price * 2.25);
                        targetObject.heldObject.Value.name = @input.Name + " Juice";
                        targetObject.heldObject.Value.preserve.Value = StardewValley.Object.PreserveType.Juice;
                        targetObject.heldObject.Value.preservedParentSheetIndex.Value = @input.ParentSheetIndex.ToString();
                        targetObject.MinutesUntilReady = 6000;
                        break;
                    case -79:
                        targetObject.heldObject.Value = new StardewValley.Object("348", 1);
                        targetObject.heldObject.Value.Price = @input.Price * 3;
                        targetObject.heldObject.Value.name = @input.Name + " Wine";
                        targetObject.heldObject.Value.preserve.Value = StardewValley.Object.PreserveType.Wine;
                        targetObject.heldObject.Value.preservedParentSheetIndex.Value = @input.ParentSheetIndex.ToString();
                        targetObject.MinutesUntilReady = 10000;
                        break;
                    default:
                        targetObject.heldObject.Value = new StardewValley.Object("346", 1);
                        targetObject.heldObject.Value.name = "Beer";
                        break;

                }

            }

            if (targetObject.heldObject.Value == null)
            {
                location.playSound("ghost");
                return;
            }


            location.playSound("Ship");

            location.playSound("bubbles");

            TemporaryAnimatedSprite temporarySprite = new(
                "TileSheets\\animations", 
                new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 
                80f, 
                6, 
                999999,
                targetObject.TileLocation * 64f + new Vector2(0f, -128f), 
                flicker: false, 
                flipped: false, 
                (targetObject.TileLocation.Y + 1f) * 64f / 10000f + 0.0001f, 
                0f, 
                Color.Yellow * 0.75f, 
                1f, 
                0f, 
                0f, 
                0f
                )
            {
                alphaFade = 0.005f
            };

            location.temporarySprites.Add(temporarySprite);

            targetObject.MinutesUntilReady = 1000;

        }

        public void FillPreservesJar(StardewValley.Object targetObject)
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

            int cropIndex = cropList[Mod.instance.randomIndex.Next(cropList.Count)];

            StardewValley.Object input = new(cropIndex.ToString(), 0);

            if (input == null) { location.playSound("ghost"); return; }

            switch (@input.Category)
            {
                case -75:
                    targetObject.heldObject.Value = new StardewValley.Object("342", 1);
                    targetObject.heldObject.Value.Price = 50 + @input.Price * 2;
                    targetObject.heldObject.Value.name = "Pickled " + @input.Name;
                    targetObject.heldObject.Value.preserve.Value = StardewValley.Object.PreserveType.Pickle;
                    targetObject.heldObject.Value.preservedParentSheetIndex.Value = @input.ParentSheetIndex.ToString();
                    break;
                case -79:
                    targetObject.heldObject.Value = new StardewValley.Object("344", 1);
                    targetObject.heldObject.Value.Price = 50 + @input.Price * 2;
                    targetObject.heldObject.Value.name = @input.Name + " Jelly";
                    targetObject.heldObject.Value.preserve.Value = StardewValley.Object.PreserveType.Jelly;
                    targetObject.heldObject.Value.preservedParentSheetIndex.Value = @input.ParentSheetIndex.ToString();
                    break;
            }

            if(targetObject.heldObject.Value == null)
            {
                location.playSound("ghost");
                return;

            }

            location.playSound("Ship");

            location.playSound("bubbles");

            TemporaryAnimatedSprite temporarySprite = new(
                "TileSheets\\animations",
                new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128),
                80f,
                6,
                999999,
                targetObject.TileLocation * 64f + new Vector2(0f, -128f),
                flicker: false,
                flipped: false,
                (targetObject.TileLocation.Y + 1f) * 64f / 10000f + 0.0001f,
                0f,
                Color.Yellow * 0.75f,
                1f,
                0f,
                0f,
                0f
                )
            {
                alphaFade = 0.005f
            };
            location.temporarySprites.Add(temporarySprite);

            targetObject.MinutesUntilReady = 4000;

        }

        public void FillCheesePress(StardewValley.Object targetObject)
        {

            location.playSound("Ship");

            targetObject.MinutesUntilReady = 240;

            switch (Mod.instance.randomIndex.Next(2))
            {
                case 0:
                    targetObject.heldObject.Value = new StardewValley.Object("426", 1);
                    break;
                default:
                    targetObject.heldObject.Value = new StardewValley.Object("424", 1);
                    break;
            }

        }

        public void FillMayonnaiseMachine(StardewValley.Object targetObject)
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

            int eggIndex = eggList[Mod.instance.randomIndex.Next(eggList.Count)];

            location.playSound("Ship");

            targetObject.MinutesUntilReady = 240;

            switch (eggIndex)
            {
                case 289:
                    targetObject.heldObject.Value = new StardewValley.Object("306", 1);
                    targetObject.heldObject.Value.Stack = 10;
                    targetObject.heldObject.Value.Quality = 2;

                    break;

                case 174:
                case 182:
                    targetObject.heldObject.Value = new StardewValley.Object("306", 1)
                    {
                        Quality = 2
                    };

                    break;

                case 176:
                case 180:
                    targetObject.heldObject.Value = new StardewValley.Object("306", 1);

                    break;

                case 442:
                    targetObject.heldObject.Value = new StardewValley.Object("307", 1);

                    break;

                case 305:
                    targetObject.heldObject.Value = new StardewValley.Object("308", 1);

                    break;

                case 107:
                    targetObject.heldObject.Value = new StardewValley.Object("807", 1);

                    break;

                case 928:
                    targetObject.heldObject.Value = new StardewValley.Object("306", 1)
                    {
                        Quality = 2
                    };
                    targetObject.heldObject.Value.Stack = 3;

                    break;
            }


        }

        public void FillLoom(StardewValley.Object targetObject)
        {

            location.playSound("Ship");

            targetObject.heldObject.Value = new StardewValley.Object("428", 1)
            {
                Stack = 2
            };
            targetObject.MinutesUntilReady = 240;

        }

        public void FillOilMaker(StardewValley.Object targetObject)
        {

            List<int> oilList = new()
            {

                270,
                421,
                430,
                431,

            };

            int oilIndex = oilList[Mod.instance.randomIndex.Next(oilList.Count)];

            location.playSound("bubbles");

            location.playSound("sipTea");

            TemporaryAnimatedSprite temporarySprite = new(
                "TileSheets\\animations",
                new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128),
                80f,
                6,
                999999,
                targetObject.TileLocation * 64f + new Vector2(0f, -128f),
                flicker: false,
                flipped: false,
                (targetObject.TileLocation.Y + 1f) * 64f / 10000f + 0.0001f,
                0f,
                Color.Yellow * 0.75f,
                1f,
                0f,
                0f,
                0f
                )
            {
                alphaFade = 0.005f
            };
            location.temporarySprites.Add(temporarySprite);

            targetObject.MinutesUntilReady = 1000;

            switch (oilIndex)
            {
                case 270:
                    targetObject.heldObject.Value = new StardewValley.Object("247", 1);
                    break;

                case 421:
                    targetObject.heldObject.Value = new StardewValley.Object("247", 1);
                    break;

                case 430:
                    targetObject.heldObject.Value = new StardewValley.Object("432", 1);
                    break;

                case 431:
                    targetObject.heldObject.Value = new StardewValley.Object("247", 1);
                    break;

            }

        }

        public void FillFurnace(StardewValley.Object targetObject)
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

            int furnaceIndex = furnaceList[Mod.instance.randomIndex.Next(furnaceList.Count)];

            switch (furnaceIndex)
            {
                case 378:
                    targetObject.heldObject.Value = new StardewValley.Object("334", 1);

                    break;
                case 380:
                    targetObject.heldObject.Value = new StardewValley.Object("335", 1);

                    break;
                case 384:
                    targetObject.heldObject.Value = new StardewValley.Object("336", 1);

                    break;
                case 386:
                    targetObject.heldObject.Value = new StardewValley.Object("337", 1);

                    break;
                case 80:
                    targetObject.heldObject.Value = new StardewValley.Object("338", 1);

                    break;
                case 82:
                    targetObject.heldObject.Value = new StardewValley.Object("338", 3);

                    break;
                case 909:
                    targetObject.heldObject.Value = new StardewValley.Object("910", 1);

                    break;
            }

            location.playSound("furnace");

            targetObject.initializeLightSource(targetObject.TileLocation);

            targetObject.showNextIndex.Value = true;

            targetObject.MinutesUntilReady = 240;

        }

        public void FillGeodeCrusher(StardewValley.Object targetObject)
        {

            targetObject.heldObject.Value = (StardewValley.Object)Utility.getTreasureFromGeode(new StardewValley.Object("749", 1));

            Game1.stats.GeodesCracked++;

            targetObject.MinutesUntilReady = 240;

            Game1.playSound("drumkit4");

            Game1.playSound("stoneCrack");

            DelayedAction.playSoundAfterDelay("steam", 200);

        }

    }

}
