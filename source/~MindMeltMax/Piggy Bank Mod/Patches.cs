/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using PiggyBank.Data;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiggyBank
{
    internal static class Patches
    {
        private static IMonitor IMonitor => ModEntry.IMonitor;
        private static IModHelper IHelper => ModEntry.IHelper;
        private static Config IConfig => ModEntry.IConfig;

        internal static void Apply(string id)
        {
            Harmony harmony = new(id);

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.draw), [typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)]),
                postfix: new(typeof(Patches), nameof(drawPostfix))
            );
        }

        internal static void drawPostfix(Object __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            try
            {
                if (__instance.ItemId != ModEntry.ObjectInfo.Id || !__instance.modData.ContainsKey(IHelper.ModRegistry.ModID) || !IConfig.DisplayHoverText || !Context.IsPlayerFree)
                    return;
                var data = ModEntry.readData(__instance);
                if (data is null || string.IsNullOrWhiteSpace(data.Label))
                    return;
                IClickableMenu.drawHoverText(spriteBatch, data.Label, Game1.smallFont, moneyAmountToDisplayAtBottom: IConfig.DisplayMoneyInTextBox ? data.Gold : -1);
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(Object.draw)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().FullName} - {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
