/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship
{

    public class SpouseInjections
    {
        private const string SPOUSE_STARDROP = "Have a Baby";

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public virtual bool checkAction(Farmer who, GameLocation l)
        public static bool CheckAction_SpouseStardrop_Prefix(NPC __instance, Farmer who, GameLocation l, ref bool __result)
        {
            try
            {
                var npcName = __instance.Name;
                if (__instance.IsInvisible || __instance.isSleeping.Value || !who.CanMove || npcName.Equals("Henchman") || l.Name.Equals("WitchSwamp"))
                {
                    return true; // run original logic
                }

                if (!npcName.Equals(who.spouse) || !who.IsLocalPlayer)
                {
                    return true; // run original logic
                }

                if (__instance.Sprite.CurrentAnimation == null)
                {
                    __instance.faceDirection(-3);
                }

                if (!who.friendshipData.ContainsKey(npcName))
                {
                    return true; // run original logic
                }

                var friendshipData = who.friendshipData[npcName];

                if (__instance.Sprite.CurrentAnimation != null || friendshipData.Points < 3125 || !_locationChecker.IsLocationMissingAndExists(SPOUSE_STARDROP))
                {
                    return true; // run original logic
                }

                _locationChecker.AddCheckedLocation(SPOUSE_STARDROP);
                __instance.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString(Game1.player.isRoommate(who.spouse) ? "Strings\\StringsFromCSFiles:Krobus_Stardrop" : "Strings\\StringsFromCSFiles:NPC.cs.4001"), __instance));
                __instance.shouldSayMarriageDialogue.Value = false;
                __instance.currentMarriageDialogue.Clear();
                who.mailReceived.Add("CF_Spouse");
                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckAction_SpouseStardrop_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
