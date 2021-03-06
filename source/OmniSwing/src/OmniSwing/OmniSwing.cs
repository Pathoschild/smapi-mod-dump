/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AlejandroAkbal/Stardew-Valley-OmniSwing-Mod
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace OmniSwing
{
    public static class OmniSwing
    {
        private static readonly ModConfig Config = ModEntry.Config;

        private static readonly IReflectionHelper Reflection = ModEntry.Helper.Reflection;

        public static void AutoSwing()
        {
            Farmer player = Game1.player;

            MeleeWeapon currentTool = player.CurrentTool as MeleeWeapon;

            if (currentTool == null)
            {
                ModLogger.Trace("Current tool is not a melee weapon.");
                return;
            }

            if (Config.CheckIfToolIsScythe && currentTool.isScythe())
            {
                ModLogger.Trace("Current tool is a Scythe.");
                return;
            }

            Vector2 toolLocation = player.GetToolLocation(true);
            //int toolLocationX = (int)Math.Round(toolLocation.X);
            //int toolLocationY = (int)Math.Round(toolLocation.Y);

            ModLogger.Trace($"Swinging '{currentTool.BaseName}'.");

            // --- Swing mechanic

            // Works and respects the game
            Reflection.GetMethod(player, "performFireTool").Invoke();

            // Works, but turns weapon invisible
            //currentTool.doSwipe(currentTool.type, toolLocation, player.facingDirection, currentTool.speed, player);

            // Works, but turns tool invisible and stops animation
            //currentTool.beginUsing(player.currentLocation, toolLocationX, toolLocationY, player);
            //currentTool.endUsing(player.currentLocation, player);

            // Doesn't do anything
            //Farmer.useTool(player);

            // Doesn't do anything
            //currentTool.DoFunction(player.currentLocation, toolLocationX, toolLocationY, 1, player);

            // Doesn't do anything
            //currentTool.leftClick(player);

            // Doesn't do anything
            //currentTool.tickUpdate(new GameTime(), player);
        }
    }
}