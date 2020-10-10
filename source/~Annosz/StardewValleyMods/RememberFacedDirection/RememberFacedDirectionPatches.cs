/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/StardewValleyModding
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace RememberFacedDirection
{
    public static class RememberFacedDirectionPatches
    {
        private static IMonitor _monitor;

        private static Dictionary<long, int> farmerFacingDirection = new Dictionary<long, int>();

        public static void Initialize(IMonitor monitor)
        {
            _monitor = monitor;
        }

        public static void Game1_PressActionButton_Prefix()
        {
            try
            {
                farmerFacingDirection[Game1.player.UniqueMultiplayerID] = Game1.player.FacingDirection;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Game1_PressActionButton_Prefix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static void Farmer_HoldUpItemThenMessage_Prefix()
        {
            try
            {
                farmerFacingDirection[Game1.player.UniqueMultiplayerID] = Game1.player.FacingDirection;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Farmer_HoldUpItemThenMessage_Prefix)}:\n{ex}", LogLevel.Error);
            }
        }


        public static void Farmer_DoneEating_Postfix(StardewValley.Farmer __instance)
        {
            try
            {
                __instance.faceDirection(farmerFacingDirection[__instance.UniqueMultiplayerID]);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Farmer_DoneEating_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static void Farmer_ShowReceiveNewItemMessage_Postfix(StardewValley.Farmer who)
        {
            try
            {
                who.faceDirection(farmerFacingDirection[who.UniqueMultiplayerID]);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Farmer_ShowReceiveNewItemMessage_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
