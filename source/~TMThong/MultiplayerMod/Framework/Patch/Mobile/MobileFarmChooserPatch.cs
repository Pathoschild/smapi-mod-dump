/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MultiplayerMod.Framework.Patch.Mobile
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    internal class MobileFarmChooserPatch : IPatch
    {
        public Type PATCH_TYPE { get; }

        public MobileFarmChooserPatch()
        {
            PATCH_TYPE = typeof(IClickableMenu).Assembly.GetType("StardewValley.Menus.MobileFarmChooser");
        }

        public void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(PATCH_TYPE, "optionButtonClick"), prefix: new HarmonyMethod(this.GetType(), nameof(prefix_optionButtonClick)));
        }

        private static bool prefix_optionButtonClick(object __instance, string name)
        {
            if (name == "OK")
            {
                int num2 = 15;
                Game1.playSound("bigSelect");
                Game1.player.farmName.Value = ModUtilities.Helper.Reflection.GetField<TextBox>(__instance, "farmnameBox").GetValue().Text.Trim();
                if (Game1.player.farmName.Length > num2)
                {
                    Game1.player.farmName.Value = Game1.player.farmName.Value.Substring(0, num2);
                }
                if (Game1.activeClickableMenu is TitleMenu)
                {

                    (Game1.activeClickableMenu as TitleMenu).createdNewCharacter(skipIntro: MobileCustomizerPatch.skipIntro);
                }
                else
                {

                    Game1.exitActiveMenu();
                    if (Game1.currentMinigame != null && Game1.currentMinigame is Intro)
                    {
                        (Game1.currentMinigame as Intro).doneCreatingCharacter();
                    }
                }
                ModUtilities.Helper.Reflection.GetField<Vector2>(__instance, "nameSize").SetValue(Game1.dialogueFont.MeasureString(ModUtilities.Helper.Reflection.GetField<string>(__instance, "nameString").GetValue()));
                ModUtilities.Helper.Reflection.GetField<Vector2>(__instance, "descSize").SetValue(Game1.dialogueFont.MeasureString(ModUtilities.Helper.Reflection.GetField<string>(__instance, "descString").GetValue()));
                Game1.multiplayerMode = 2;
                Game1.options.setServerMode("friends");
                return false;
            }
            return true;
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
