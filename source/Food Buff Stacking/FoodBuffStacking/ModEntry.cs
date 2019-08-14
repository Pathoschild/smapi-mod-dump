/* Welcome to Stardew BuffStack!
 * 
 * This mod allows for the stacking of buffs from different foods
 * (instead of wiping out the previous buffs when a new food is consumed)
 * 
 * At a high-level, it does this by immediately removing the "current food", and instead
 * using the list of buffs applied to the player as the source of truth.
 * 
 * Some notes:
 *    - The Buff code in the game is a bit confusing. "Buff" could refer to a single buff, or to multiple buffs (from a food).
 *      It also has some confusing terminology. Where possible, I've deviated from the game terminology for more accurate terms.
 * 
 */ 
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace StardewBuffStack
{
    
    public class ModEntry : Mod
    {

        private class BuffStackHelper
        {
            private const int totalNumBuffIDs = 12;

            private IReflectionHelper reflector;
            private BuffsDisplay buffHolder;

            public BuffStackHelper(BuffsDisplay buffHolder, IReflectionHelper reflector)
            {
                this.buffHolder = buffHolder;
                this.reflector = reflector;
            }

            /* The API for manipulating the Buffs applied to the user through foods  */

            /// <summary>
            /// Removes the current food and its buffs from the player.
            /// </summary>
            public void removeCurrentFood()
            {
                buffHolder.food.removeBuff();
                buffHolder.food = null;
            }

            /// <summary>
            /// Returns if the oldBuff should be replaced by the newBuff.
            /// </summary>
            /// <remarks>
            public bool shouldReplaceBuff(Buff newBuff, Buff oldBuff)
            {
                int oldValue = getBuffValue(oldBuff);
                int newValue = getBuffValue(oldBuff);
                return oldValue < newValue || (oldValue == newValue && oldBuff.millisecondsDuration < newBuff.millisecondsDuration);
            }


            public Dictionary<int, Buff> splitFoodIntoBuffMap()
            {
                Buff food = this.buffHolder.food;
                int[] buffAttributes = getBuffAttributes(food);
                Dictionary<int, Buff> buffMap = new Dictionary<int, Buff>();
                for (int buffID = 0; buffID < totalNumBuffIDs; buffID++)
                {
                    int attributeValue = buffAttributes[buffID];
                    if (attributeValue != 0)
                    {
                        Buff singleBuff = getSingleBuffFromFood(food, attributeValue, buffID);
                        buffMap.Add(buffID, singleBuff);
                    }
                }
                return buffMap;
            }

            private int[] getBuffAttributes(Buff buff)
            {
                return this.reflector.GetField<int[]>(buff, "buffAttributes").GetValue();
            }

            private int getBuffValue(Buff buff)
            {
                return getBuffAttributes(buff)[buff.which];
            }

            private Buff getSingleBuffFromFood(Buff food, int value, int buffID)
            {
                Buff buff = new Buff("", food.millisecondsDuration, food.source, buffID);
                buff.displaySource = food.displaySource;
                buff.which = buffID;
                getBuffAttributes(buff)[buffID] = value;
                return buff;
            }
        };

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += DoFoodThings;
        }

        private void DoFoodThings(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.buffsDisplay.food != null)
            {
                BuffStackHelper helper = new BuffStackHelper(Game1.buffsDisplay, this.Helper.Reflection);

                Dictionary<int, Buff> foodBuffMap = helper.splitFoodIntoBuffMap();
                helper.removeCurrentFood();

                List<Buff> currentBuffs = Game1.buffsDisplay.otherBuffs;
                for (int index = 0; index < currentBuffs.Count; index++)
                {
                    Buff currentBuff = currentBuffs[index];
                    if (foodBuffMap.ContainsKey(currentBuff.which))
                    {
                        Buff newBuff = foodBuffMap[currentBuff.which];
                        if (helper.shouldReplaceBuff(newBuff, currentBuff))
                        {
                            // Instead of going through the song-and-dance of removing 
                            // the old and adding the new we just swap the two.
                            currentBuff.removeBuff();
                            newBuff.addBuff();
                            currentBuffs[index] = newBuff;
                        }

                        foodBuffMap.Remove(currentBuff.which);
                    }
                }

                // Whatever is leftover is a completely new buff, add it
                foreach (Buff buff in foodBuffMap.Values)
                {
                    currentBuffs.Add(buff);
                    buff.addBuff();
                }


                Game1.buffsDisplay.syncIcons();
            }
        }

    }
}