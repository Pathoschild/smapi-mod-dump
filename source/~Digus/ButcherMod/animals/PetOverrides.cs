/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.tools;
using StardewValley;
using StardewValley.Characters;

namespace AnimalHusbandryMod.animals
{
    public class PetOverrides
    {
        public static bool checkAction(Pet __instance, Farmer who)
        {
            if (__instance.IsInvisible)
            {
                return false;
            }
            if (!__instance.lastPetDay.ContainsKey(who.UniqueMultiplayerID))
                __instance.lastPetDay.Add(who.UniqueMultiplayerID, -1);
            if (__instance.lastPetDay[who.UniqueMultiplayerID] == Game1.Date.TotalDays && AnimalContestController.CanChangeParticipantPet())
            {
                __instance.playContentSound();
                __instance.Halt();
                __instance.CurrentBehavior = 0;
                __instance.OnNewBehavior();
                __instance.Halt();
                __instance.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>() { new FarmerSprite.AnimationFrame(18, 200) });

                who.Halt();
                int currentFrame = who.FarmerSprite.currentFrame;
                switch (who.FacingDirection)
                {
                    case 0:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(62, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                    case 1:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(58, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                    case 2:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(54, 200, false, false, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                    case 3:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(58, 200, false, true, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                }
                who.FarmerSprite.oldFrame = currentFrame;
                AnimalContestController.RemovePetParticipant();
                Game1.player.addItemByMenuIfNecessary(new ParticipantRibbon());
                return false;
            }
            return true;
        }

        public static bool update(Pet __instance)
        {
            if (__instance.IsInvisible)
            {
                return false;
            }
            return true;
        }

        public static bool draw(Pet __instance)
        {
            if (__instance.IsInvisible)
            {
                return false;
            }
            return true;
        }
    }
}
