/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewGMCMOptions
**
*************************************************/

// // Copyright 2023 Jamie Taylor
using System;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace GMCMOptions.Framework {
    public static class TooltipHelper {
        public static string? Title { get; set; }
        public static string? BodyText { get; set; }

        public static void Init(Harmony harmony, IMonitor monitor) {
            try {
                harmony.Patch(
                    original: AccessTools.Method("GenericModConfigMenu.Framework.SpecificModConfigMenu:draw", new Type[] { typeof(SpriteBatch) }),
                    prefix: new HarmonyMethod(typeof(TooltipHelper), nameof(Draw_Pre)),
                    postfix: new HarmonyMethod(typeof(TooltipHelper), nameof(Draw_Post)));
                monitor.Log("TooltipHelper installed pre and postfix patches on GenericModConfigMenu.Framework.SpecificModConfigMenu:draw", LogLevel.Trace);
            } catch (Exception e) {
                monitor.Log("TooltipHelper was unable to install harmony patches.  Tooltip functionality will not be enabled.", LogLevel.Warn);
                monitor.Log($"Exception details: {e.Message}", LogLevel.Debug);
                monitor.Log($"\n{e.StackTrace}", LogLevel.Trace);
            }
        }

        private static void Draw_Pre() {
            Title = null;
            BodyText = null;
        }
        private static void Draw_Post(SpriteBatch b) {
            var title = Title;
            var text = BodyText;
            if (title is null || text is null) return;
            if (!text.Contains("\n")) text = Game1.parseText(text, Game1.smallFont, 800);
            if (!title.Contains("\n")) title = Game1.parseText(title, Game1.dialogueFont, 800);
            IClickableMenu.drawToolTip(b, text, title, null);
        }
    }
}

