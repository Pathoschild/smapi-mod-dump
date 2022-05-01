/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/FarmHouseRedone
**
*************************************************/

using StardewValley;
using xTile.Dimensions;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone
{

    class GameLocation__updateAmbientLighting_Patch
    {
        public static bool Prefix(GameLocation __instance)
        {
            if (__instance.Name.StartsWith("DECORHOST_"))
            {
                //new Color(100, 88, 0);
                Game1.ambientLight = new Color(255, 226, 10);
                Game1.changeMusicTrack("communityCenter", Game1.currentTrackOverrideable, Game1.MusicContext.Default);

                int ghostCount = (__instance.map.Layers[0].LayerWidth * __instance.map.Layers[0].LayerHeight) / 400;
                Logger.Log("Adding " + ghostCount + " ghosts");
                for (int ghostIndex = 0; ghostIndex < ghostCount; ghostIndex++)
                {
                    __instance.addCharacterAtRandomLocation(new StardewValley.Monsters.Ghost(Vector2.Zero));
                }

                return false;
            }
            return true;
        }
    }

    class GameLocation_performAction_Patch
    {
        public static bool Prefix(string action, Farmer who, Location tileLocation, bool __result, GameLocation __instance)
        {
            if (action == "Mailbox")
            {
                Logger.Log(Game1.player.name + " checked the mailbox at " + tileLocation.ToString() + "...");
                if (__instance is Farm)
                {
                    Logger.Log("Mailbox was on the farm...");
                    Point mailboxPosition = FarmState.getMailboxPosition(Game1.player);
                    Logger.Log(Game1.player.name + "'s mailbox is at " + mailboxPosition.ToString());
                    if (tileLocation.X != mailboxPosition.X || tileLocation.Y != mailboxPosition.Y)
                    {
                        Logger.Log("Mailbox did not belong to " + Game1.player.name);
                        return true;
                    }
                    Logger.Log("Mailbox belonged to " + Game1.player.name);
                    __instance.mailbox();
                    __result = true;
                    return false;
                }
            }
            return true;
        }
    }

    //class GameLocation_houseUpgradeAccept_Patch
    //{
    //    public static bool Prefix(GameLocation __instance)
    //    {
    //        int upgradeLevel = Game1.player.houseUpgradeLevel;
    //        if (upgradeLevel < 3)
    //            return true;
    //        LevelNUpgrade upgrade = FarmHouseStates.upgrades[FarmHouseStates.selectedUpgrade];
    //        bool canBuy = true;
    //        string reason = "";
    //        if (Game1.player.Money >= upgrade.moneyCost)
    //        {
    //            foreach (int itemID in upgrade.cost.Keys)
    //            {
    //                if (!Game1.player.hasItemInInventory(itemID, upgrade.cost[itemID]))
    //                {
    //                    reason = "Not enough " + ObjectIDHelper.getName(itemID);
    //                    canBuy = false;
    //                    break;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            canBuy = false;
    //            reason = Game1.content.LoadString("Strings\\UI:NotEnoughMoney3");
    //        }

    //        if (canBuy)
    //        {
    //            Game1.player.daysUntilHouseUpgrade.Value = 3;
    //            Game1.player.Money -= upgrade.moneyCost;
    //            foreach(int itemID in upgrade.cost.Keys)
    //            {
    //                Game1.player.removeItemsFromInventory(itemID, upgrade.cost[itemID]);
    //            }
    //            Game1.getCharacterFromName("Robin", true).setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"), false, false);
    //            Game1.drawDialogue(Game1.getCharacterFromName("Robin", true));
    //            FarmHouseStates.getState(Utility.getHomeOfFarmer(Game1.player)).pendingUpgrade = upgrade;
    //        }
    //        else
    //        {
    //            Game1.drawObjectDialogue(reason);
    //        }
    //        return false;
    //    }
    //}

    //class GameLocation_houseUpgradeOffer_Patch
    //{
    //    public static bool Prefix(GameLocation __instance)
    //    {
    //        FarmHouseStates.selectedUpgrade = -1;
    //        int upgradeLevel = Game1.player.houseUpgradeLevel + 1;
    //        //if (upgradeLevel < 3)
    //        //    return true;
    //        int upgradeIndex = -1;
    //        foreach(LevelNUpgrade upgrade in FarmHouseStates.upgrades)
    //        {
    //            if(upgrade.level == upgradeLevel)
    //            {
    //                upgradeIndex = FarmHouseStates.upgrades.IndexOf(upgrade);
    //                break;
    //            }
    //        }
    //        if (upgradeIndex == -1)
    //            return true;
    //        FarmHouseStates.selectedUpgrade = upgradeIndex;
    //        __instance.createQuestionDialogue(FarmHouseStates.upgrades[upgradeIndex].description, __instance.createYesNoResponses(), "upgrade");

    //        return false;
    //    }
    //}

    //class GameLocation_carpenters1_Patch
    //{
    //    public static bool Prefix(GameLocation __instance)
    //    {
    //        FarmHouseStates.selectedUpgrade = -1;
    //        int upgradeLevel = Game1.player.houseUpgradeLevel;
    //        if (upgradeLevel < 4)
    //            return true;

    //        bool hasUpgrade = false;
    //        foreach (LevelNUpgrade upgrade in FarmHouseStates.upgrades)
    //        {
    //            if (upgrade.level == upgradeLevel)
    //            {
    //                hasUpgrade = true;
    //                break;
    //            }
    //        }
    //        if (!hasUpgrade)
    //            return true;
    //        //__instance.responseList.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
    //    }
    //}

    //class GameLocation_carpenters_Patch
    //{
    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

    //        List<int> indicesToDelete = new List<int>();
    //        int logRange = 0;
    //        for (int i = 0; i < codes.Count; i++)
    //        {
    //            if (codes[i].opcode == OpCodes.Ldfld && codes[i].operand.ToString().Contains("houseUpgradeLevel"))
    //            {
    //                Logger.Log("Replacing vanilla house upgrade level cap at index " + (i - 1) + "...");
    //                logRange = i - 3;
    //                codes[i + 2] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmHouseStates), nameof(FarmHouseStates.hasUpgradeAvailable)));
    //                codes[i + 3] = new CodeInstruction(OpCodes.Brfalse, codes[i+3].operand);
    //                //Logger.Log("Index " + (i + 3) + ": " + codes[i+3].ToString());
    //                indicesToDelete.Add(i + 1);
    //                indicesToDelete.Add(i);
    //                indicesToDelete.Add(i - 1);
    //                break;
    //            }
    //        }

    //        //indicesToDelete.Reverse();

    //        foreach (int index in indicesToDelete)
    //        {
    //            codes.RemoveAt(index);
    //        }

    //        for(int i = logRange; i < logRange + 8; i++)
    //        {
    //            Logger.Log("Index " + (i) + ": " + codes[i].ToString());
    //        }
    //        return codes.AsEnumerable();
    //    }
    //}
}
