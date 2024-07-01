/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;

namespace StardewDruid.Event.Relics
{
    internal class RelicMists : EventHandle
    {

        public RelicMists()
        {

        }

        public override bool TriggerAbort()
        {

            if ((Game1.getLocationFromName("CommunityCenter") as CommunityCenter).areasComplete[2])
            {

                Mod.instance.questHandle.CompleteQuest(eventId);

                TriggerRemove();

                triggerAbort = true;

            }

            return triggerAbort;

        }

        public override bool TriggerCheck()
        {

            if ((Game1.getLocationFromName("CommunityCenter") as CommunityCenter).numberOfCompleteBundles() == 0)
            {
                
                Mod.instance.CastDisplay("The forest spirits have not left instructions for how to fix this yet");

                return false;

            }

            return base.TriggerCheck();
        
        }

        public override void EventActivate()
        {

            base.EventActivate();

            ModUtility.AnimateHands(Game1.player,Game1.player.FacingDirection,600);

            Mod.instance.iconData.DecorativeIndicator(location, Game1.player.Position, IconData.decorations.mists, 4f, new());

            location.playSound("thunder_small");

        }

        public override void EventInterval()
        {

            activeCounter++;

            if(activeCounter == 1)
            {

                foreach (IconData.relics relic in new List<IconData.relics>()
                {

                    IconData.relics.avalant_disc,
                    IconData.relics.avalant_chassis,
                    IconData.relics.avalant_gears,
                    IconData.relics.avalant_casing,
                    IconData.relics.avalant_needle,
                    IconData.relics.avalant_measure,

                })
                {
                    ThrowHandle throwRelic = new(Game1.player.Position, origin + new Vector2((Mod.instance.randomIndex.Next(5) * 32) - 64, -128), relic);

                    throwRelic.delay = Mod.instance.randomIndex.Next(5) * 10;

                    throwRelic.impact = IconData.impacts.splash;

                    throwRelic.register();

                }

            }

            if(activeCounter == 3)
            {

                CommunityCenter communityCenter = location as CommunityCenter;

                Dictionary<string, string> bundleData = Game1.netWorldState.Value.BundleData;

                string areaNameFromNumber = CommunityCenter.getAreaNameFromNumber(2);

                foreach (string key in bundleData.Keys)
                {
                    
                    if (key.Contains(areaNameFromNumber))
                    {
                        
                        int bundleId = Convert.ToInt32(key.Split('/')[1]);

                        communityCenter.bundleRewards[bundleId] = true;

                        communityCenter.bundles.FieldDict[bundleId][0] = true;

                    }

                }

                communityCenter.markAreaAsComplete(2);

                communityCenter.restoreAreaCutscene(2);

                communityCenter.areaCompleteReward(2);

                Mod.instance.questHandle.CompleteQuest(eventId);

            }

        }

    }

}