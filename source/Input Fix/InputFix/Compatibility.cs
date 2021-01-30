/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Windmill-City/InputFix
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Reflection;

namespace InputFix
{
    public class Compatibility
    {
        public static bool ignore;
        private static Composition comp;
        public static void PatchChatCommands(IMonitor monitor, HarmonyInstance harmony)
        {
            Type CCTB = AccessTools.TypeByName("ChatCommands.ClassReplacements.CommandChatTextBox");
            if (CCTB != null)
            {
                ModEntry.notifyHelper.NotifyMonitor("Patching CommandChatTextBox");
                MethodInfo m_draw2 = AccessTools.Method(CCTB, "Draw");
                harmony.Patch(m_draw2, new HarmonyMethod(typeof(Compatibility), "DrawBegin"),
                    new HarmonyMethod(typeof(Compatibility), "DrawEnd"));

                MethodInfo m_leftarrow = AccessTools.Method(CCTB, "OnLeftArrowPress");
                harmony.Patch(m_leftarrow, new HarmonyMethod(typeof(Compatibility), "CommandChatTextBoxOnArrow"));

                MethodInfo m_rightarrow = AccessTools.Method(CCTB, "OnRightArrowPress");
                harmony.Patch(m_rightarrow, new HarmonyMethod(typeof(Compatibility), "CommandChatTextBoxOnArrow"));

                comp = (Composition)Traverse.Create(typeof(KeyboardInput_)).Field("comp").GetValue();
            }
            else
            {
                ModEntry.notifyHelper.NotifyMonitor("CommandChatTextBox NOT FOUND", LogLevel.Error);
            }
        }

        private static void DrawBegin() => ignore = true;

        private static void DrawEnd(SpriteBatch spriteBatch)
        {
            ignore = false;
            comp.Draw(spriteBatch);
        }

        private static bool CommandChatTextBoxOnArrow()
        {
            return comp.text.Length == 0;
        }
    }
}