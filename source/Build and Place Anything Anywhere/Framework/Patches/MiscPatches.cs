/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using Common.Helpers;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network.NetEvents;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AnythingAnywhere.Framework.Patches;
internal sealed class MiscPatches : PatchHelper
{
    public void Apply()
    {
        Patch<Cask>(PatchType.Prefix, nameof(Cask.IsValidCaskLocation), nameof(IsValidCaskLocationPrefix));
        Patch<MiniJukebox>(PatchType.Prefix, nameof(MiniJukebox.checkForAction), nameof(CheckForActionPrefix), [typeof(Farmer), typeof(bool)]);
        Patch<GameLocation>(PatchType.Prefix, nameof(GameLocation.spawnWeedsAndStones), nameof(SpawnWeedsAndStonesPrefix), [typeof(int), typeof(bool), typeof(bool)]);
        Patch<GameLocation>(PatchType.Prefix, nameof(GameLocation.loadWeeds), nameof(LoadWeedsPrefix));

        Patch<GameLocation>(PatchType.Prefix, "communityUpgradeOffer", nameof(CommunityUpgradeOfferPrefix));
        Patch<GameLocation>(PatchType.Prefix, "communityUpgradeAccept", nameof(CommunityUpgradeAcceptPrefix));
    }

    // Enable cask functionality outside of the farm
    private static bool IsValidCaskLocationPrefix(Cask __instance, ref bool __result)
    {
        if (!ModEntry.Config.EnableCaskFunctionality) return true;

        __result = true;
        return false;
    }

    // Enable jukebox functionality outside of the farm
    private static bool CheckForActionPrefix(MiniJukebox __instance, Farmer who, ref bool __result, bool justCheckingForActivity = false)
    {
        if (!ModEntry.Config.EnableJukeboxFunctionality)
        {
            return true; //run the original method
        }
        if (justCheckingForActivity)
        {
            __result = true;
            return false;
        }
        GameLocation location = __instance.Location;
        if (location.IsOutdoors && location.IsRainingHere())
        {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Mini_JukeBox_OutdoorRainy"));
        }
        else
        {
            List<string> jukeboxTracks = Utility.GetJukeboxTracks(Game1.player, Game1.player.currentLocation);
            jukeboxTracks.Insert(0, "turn_off");
            jukeboxTracks.Add("random");
            Game1.activeClickableMenu = new ChooseFromListMenu(jukeboxTracks, __instance.OnSongChosen, isJukebox: true, location.miniJukeboxTrack.Value);
        }
        __result = true;
        return false;
    }

    private static bool SpawnWeedsAndStonesPrefix(GameLocation __instance, int numDebris = -1, bool weedsOnly = false, bool spawnFromOldWeeds = true)
    {
        if (!ModEntry.Config.EnableGoldClockAnywhere)
            return true;

        bool hasGoldClock = __instance.buildings.Any(building => building.buildingType.Value == "Gold Clock");
        return !hasGoldClock || Game1.netWorldState.Value.goldenClocksTurnedOff.Value;
    }

    private static bool LoadWeedsPrefix(GameLocation __instance)
    {
        if (!ModEntry.Config.EnableGoldClockAnywhere)
            return true;

        bool hasGoldClock = __instance.buildings.Any(building => building.buildingType.Value == "Gold Clock");
        return !hasGoldClock || Game1.netWorldState.Value.goldenClocksTurnedOff.Value;
    }

    // Make community upgrades free
    private static bool CommunityUpgradeOfferPrefix(GameLocation __instance)
    {
        if (!ModEntry.Config.EnableFreeCommunityUpgrade)
            return true;

        string msg;
        if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        {
            msg = Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_CommunityUpgrade1").Replace("500,000", "0").Replace("500.000", "0").Replace("500 000", "0").Replace("950", "0");
            __instance.createQuestionDialogue(Game1.parseText(msg), __instance.createYesNoResponses(), "communityUpgrade");
            Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "pamHouseUpgradeAsked", MailType.Received, add: true);
        }
        else if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
        {
            msg = Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_CommunityUpgrade2").Replace("300,000", "0").Replace("300.000", "0").Replace("300 000", "0");
            __instance.createQuestionDialogue(Game1.parseText(msg), __instance.createYesNoResponses(), "communityUpgrade");
        }

        return false;
    }

    private static bool CommunityUpgradeAcceptPrefix(GameLocation __instance)
    {
        if (!ModEntry.Config.EnableFreeCommunityUpgrade)
            return true;

        if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        {
            Game1.RequireCharacter("Robin").setNewDialogue("Data\\ExtraDialogue:Robin_PamUpgrade_Accepted");
            Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
            Game1.RequireLocation<Town>("Town").daysUntilCommunityUpgrade.Value = 3;
            ModEntry.Multiplayer?.globalChatInfoMessage("CommunityUpgrade", Game1.player.Name);
        }
        else if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
        {
            Game1.RequireCharacter("Robin").setNewDialogue("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted");
            Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
            Game1.RequireLocation<Town>("Town").daysUntilCommunityUpgrade.Value = 3;
            ModEntry.Multiplayer?.globalChatInfoMessage("CommunityUpgrade", Game1.player.Name);
        }

        return false;
    }
}