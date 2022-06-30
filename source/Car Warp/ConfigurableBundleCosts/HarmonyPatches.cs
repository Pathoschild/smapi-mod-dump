/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;

namespace ConfigurableBundleCosts;

public class HarmonyPatches
{

    /// <returns><c>True</c> if successfully patched, <c>False</c> if Exception is encountered.</returns>
    public static bool ApplyHarmonyPatches()
    {
        try
        {
            Harmony harmony = new(Globals.Manifest.UniqueID);

            harmony.Patch(
                original: typeof(JojaCDMenu).GetMethod("getPriceFromButtonNumber"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(getPriceFromButtonNumber_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(JojaMart), "buyMovieTheater"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(buyMovieTheater_Prefix))
            );
				
            harmony.Patch(
                original: typeof(JojaMart).GetMethod("answerDialogue"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(answerDialogue_Prefix))
            );

            harmony.Patch(
                original: typeof(JojaMart).GetMethod("answerDialogue"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(answerDialogue_Postfix))
            );
				
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            return false;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony method - maintain case from original")]
    public static void answerDialogue_Prefix(Response answer, bool __result, out bool __state)
    {
        try
        {
            if (Game1.player.Money >= Globals.Config.Joja.MembershipCost)
            {
                __state = true;
                Game1.player.Money += 5000;
            }
            else
            {
                __state = false;
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Failed in {nameof(buyMovieTheater_Prefix)}:\n{ex}");
            __state = false;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony method - maintain case from original")]
    public static void answerDialogue_Postfix(Response answer, bool __result, bool __state)
    {
        try
        {
            if (__state)
            {
                Game1.player.Money -= Globals.Config.Joja.MembershipCost;
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Failed in {nameof(buyMovieTheater_Prefix)}:\n{ex}");
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony method - maintain case from original")]
    public static bool getPriceFromButtonNumber_Prefix(int buttonNumber, ref int __result)
    {
        try
        {
            __result = buttonNumber switch
            {
                0 => Globals.Config.Joja.ApplyValues ? Globals.Config.Joja.BusCost : 40000,
                1 => Globals.Config.Joja.ApplyValues ? Globals.Config.Joja.MinecartsCost : 15000,
                2 => Globals.Config.Joja.ApplyValues ? Globals.Config.Joja.BridgeCost : 25000,
                3 => Globals.Config.Joja.ApplyValues ? Globals.Config.Joja.GreenhouseCost : 35000,
                4 => Globals.Config.Joja.ApplyValues ? Globals.Config.Joja.PanningCost : 20000,
                _ => 10000
            };

            return false;
        }
        catch (Exception ex)
        {
            Log.Error($"Failed in {nameof(getPriceFromButtonNumber_Prefix)}:\n{ex}");
            return true; // run original logic
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Harmony method - maintain case from original")]
    public static bool buyMovieTheater_Prefix(int response, bool __result)
    {
        try
        {
            int cost = Globals.Config.Joja.ApplyValues ? Globals.Config.Joja.MovieTheaterCost : 500000;

            if (response == 0)
            {
                if (Game1.player.Money >= cost)
                {
                    Game1.player.Money -= cost;
                    Game1.addMailForTomorrow("ccMovieTheater", noLetter: true, sendToEveryone: true);
                    Game1.addMailForTomorrow("ccMovieTheaterJoja", noLetter: true, sendToEveryone: true);
                    if (Game1.player.team.theaterBuildDate.Value < 0)
                    {
                        Game1.player.team.theaterBuildDate.Set(Game1.Date.TotalDays + 1);
                    }
                    JojaMart.Morris.setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Morris_TheaterBought"));
                    Game1.drawDialogue(JojaMart.Morris);
                }
                else
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325"));
                }
            }
            __result = true;
            return false;
        }
        catch (Exception ex)
        {
            Log.Error($"Failed in {nameof(buyMovieTheater_Prefix)}:\n{ex}");
            return true; // run original logic
        }
    }
}
