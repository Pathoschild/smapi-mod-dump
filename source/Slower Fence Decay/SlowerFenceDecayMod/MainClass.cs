/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/speeder1/FenceSlowDecayMod
**
*************************************************/

/*
    Copyright 2016 Maurício Gomes (Speeder)

    Slower Fence Decay Mod is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Slower Fence Decay Mod is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Slower Fence Decay Mod.  If not, see <http://www.gnu.org/licenses/>.
 */

using StardewValley;
using StardewModdingAPI;
using System;

namespace SlowerFenceDecayMod
{
    public class FenceDecayModMainClass : Mod
    {        
        public override void Entry(params object[] objects)
        {
            StardewModdingAPI.Events.TimeEvents.DayOfMonthChanged += Event_DayOfMonthChanged;
        }

        static void Event_DayOfMonthChanged(object sender, EventArgs e)
        {
            //this event will fire every night, or when some player use a cheat to change the day... 
            //we hope they won't do that!

            GameLocation farm = Game1.getLocationFromName("Farm");
            Fence fenceObj;
            foreach (StardewValley.Object obj in farm.objects.Values)
            {
                if(obj is Fence)
                {
                    fenceObj = (Fence)obj;                    
                    switch(fenceObj.whichType)
                    {
                        case 1: //wood fence
                            if (fenceObj.maxHealth < 60f) FixNewFenceHealth(fenceObj);
                            break;
                        case 2: //stone fence
                            if (fenceObj.maxHealth < 100f) FixNewFenceHealth(fenceObj);
                            break;
                        case 3: //iron fence
                        case 4: //gate
                            if (fenceObj.maxHealth < 210f) FixNewFenceHealth(fenceObj);
                            break;
                        case 5: //hardwood fence
                        default: //none, but just in case...
                            if (fenceObj.maxHealth < 400f) FixNewFenceHealth(fenceObj);
                            break;
                    }
                }
            }
        }

        static Random randomGen;

        static void FixNewFenceHealth(Fence fence)
        {
            if(randomGen == null)
            {
                randomGen = new Random();
            }

            int diceValue;
            int medianValue;
            switch (fence.whichType)
            {
                case 1:
                    diceValue = 5;
                    medianValue = 60;
                    break;
                case 2:
                    diceValue = 10;
                    medianValue = 110;
                    break;
                case 3:
                case 4:
                    diceValue = 15;
                    medianValue = 160;
                    break;
                case 5:
                default:
                    diceValue = 20;
                    medianValue = 210;
                    break;
            }

            int counter = 0;
            float healthToAdd = 0;
            while(counter < 20) //20 dice! to make a bell curve!
            {
                healthToAdd += randomGen.Next(1, diceValue);
                ++counter;
            }
            
            healthToAdd -= medianValue;
            if (healthToAdd < 0) healthToAdd = -healthToAdd; //if negative, flip the value
            healthToAdd += medianValue;            

            fence.maxHealth += healthToAdd;
            fence.health = fence.maxHealth;
        }
    }
}
