/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace DeluxeGrabberRedux.Grabbers
{
    class TownGarbageCanGrabber : MapGrabber
    {
        public TownGarbageCanGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
        }

        public override bool GrabItems()
        {
            if (Config.garbageCans && Location is Town town)
            {
                // impl @ StardewValley::Town::checkAction
                var garbageCheckedReflection = Mod.Helper.Reflection.GetField<NetArray<bool, NetBool>>(town, "garbageChecked", true);
                var garbageChecked = garbageCheckedReflection.GetValue();
                var grabbed = false;
                for (int whichCan = 0; whichCan < garbageChecked.Length; whichCan++)
                {
                    Item obj = null;
                    if (garbageChecked[whichCan]) continue;
                    garbageChecked[whichCan] = true;
                    Random garbageRandom = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + 777 + whichCan * 77);
                    int prewarm = garbageRandom.Next(0, 100);
                    for (int k = 0; k < prewarm; k++) garbageRandom.NextDouble();
                    prewarm = garbageRandom.Next(0, 100);
                    for (int j = 0; j < prewarm; j++) garbageRandom.NextDouble();
                    //Game1.stats.incrementStat("trashCansChecked", 1);
					var mega = Game1.stats.getStat("trashCansChecked") > 20 && garbageRandom.NextDouble() < 0.01;
                    var doubleMega = Game1.stats.getStat("trashCansChecked") > 20 && garbageRandom.NextDouble() < 0.002;

                    if (doubleMega) obj = new Hat(66);
                    else if (mega || garbageRandom.NextDouble() < 0.2 + Player.team.AverageDailyLuck())
                    {
                        int item = 168;
                        switch (garbageRandom.Next(10))
                        {
                            case 0:
                                item = 168;
                                break;
                            case 1:
                                item = 167;
                                break;
                            case 2:
                                item = 170;
                                break;
                            case 3:
                                item = 171;
                                break;
                            case 4:
                                item = 172;
                                break;
                            case 5:
                                item = 216;
                                break;
                            case 6:
                                item = Utility.getRandomItemFromSeason(Game1.currentSeason, whichCan, forQuest: false);
                                break;
                            case 7:
                                item = 403;
                                break;
                            case 8:
                                item = 309 + garbageRandom.Next(3);
                                break;
                            case 9:
                                item = 153;
                                break;
                        }
                        if (whichCan == 3 && garbageRandom.NextDouble() < 0.2 + Player.team.AverageDailyLuck())
                        {
                            item = 535;
                            if (garbageRandom.NextDouble() < 0.05)
                            {
                                item = 749;
                            }
                        }
                        if (whichCan == 4 && garbageRandom.NextDouble() < 0.2 + Player.team.AverageDailyLuck())
                        {
                            item = 378 + garbageRandom.Next(3) * 2;
                            garbageRandom.Next(1, 5);
                        }
                        if (whichCan == 5 && garbageRandom.NextDouble() < 0.2 + Player.team.AverageDailyLuck() && Game1.dishOfTheDay != null)
                        {
                            item = (((int)Game1.dishOfTheDay.parentSheetIndex != 217) ? ((int)Game1.dishOfTheDay.parentSheetIndex) : 216);
                        }
                        if (whichCan == 6 && garbageRandom.NextDouble() < 0.2 + Player.team.AverageDailyLuck())
                        {
                            item = 223;
                        }
                        if (whichCan == 7 && garbageRandom.NextDouble() < 0.2)
                        {
                            if (!Utility.HasAnyPlayerSeenEvent(191393))
                            {
                                item = 167;
                            }
                            if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater") && !Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheaterJoja"))
                            {
                                item = ((!(garbageRandom.NextDouble() < 0.25)) ? 270 : 809);
                            }
                        }
                        if (Game1.random.NextDouble() <= 0.25 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
                        {
                            item = ItemIds.QiBeans;
                        }
                        obj = new SObject(item, 1);
                    }

                    grabbed = TryAddItem(obj) || grabbed;
                }
                return grabbed;
            }
            else
            {
                return false;
            }
        }
    }
}
