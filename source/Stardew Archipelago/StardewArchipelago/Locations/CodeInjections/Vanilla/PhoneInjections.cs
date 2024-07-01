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
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class PhoneInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static WeaponsManager _weaponsManager;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, WeaponsManager weaponsManager)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _weaponsManager = weaponsManager;
        }

        // public void CallAdventureGuild()
        public static bool CallAdventureGuild_AllowRecovery_Prefix(DefaultPhoneHandler __instance)
        {
            try
            {
                Game1.currentLocation.playShopPhoneNumberSounds("AdventureGuild");
                Game1.player.freezePause = 4950;
                DelayedAction.functionAfterDelay(() =>
                {
                    Game1.playSound("bigSelect");
                    var character = Game1.getCharacterFromName("Marlon");
                    if (Game1.player.mailForTomorrow.Contains("MarlonRecovery"))
                    {
                        Game1.DrawDialogue(character, "Strings\\Characters:Phone_Marlon_AlreadyRecovering");
                    }
                    else
                    {
                        Game1.DrawDialogue(character, "Strings\\Characters:Phone_Marlon_Open");
                        Game1.afterDialogues += () =>
                        {
                            var equipmentsToRecover = _weaponsManager.GetEquipmentsForSale(IDProvider.ARCHIPELAGO_EQUIPMENTS_RECOVERY);
                            if (equipmentsToRecover.Any())
                            {
                                Game1.player.forceCanMove();
                                Utility.TryOpenShopMenu("AdventureGuildRecovery", "Marlon");
                            }
                            else
                            {
                                Game1.DrawDialogue(character, "Strings\\Characters:Phone_Marlon_NoDeathItems");
                            }
                        };
                    }
                }, 4950);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CallAdventureGuild_AllowRecovery_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
