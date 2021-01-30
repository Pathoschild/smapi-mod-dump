/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/vgperson/CommunityCenterHelper
**
*************************************************/

using System;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace CommunityCenterHelper
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private static IMonitor myMonitor;
        private static IInputHelper input;
        
        private static string[] ingredientHoverTitle;
        private static string[] ingredientHoverText;
        
        public static bool debugClearCompletedBundles = false;
        public static bool debugUnlockMissingBundle = false;
        public static bool debugUnlockCooking = false;
        public static bool debugShowUnknownIDs = false;
        
        /***************************
         ** Mod Injection Methods **
         ***************************/
        
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            try
            {
                myMonitor = this.Monitor;
                input = helper.Input;
                
                ItemHints.str = helper.Translation;
                ItemHints.modRegistry = helper.ModRegistry;
                ItemHints.Config = helper.ReadConfig<ModConfig>();
                
                HarmonyInstance harmonyInstance = HarmonyInstance.Create(this.ModManifest.UniqueID);
                
                patchPostfix(harmonyInstance, typeof(JunimoNoteMenu), "setUpBundleSpecificPage",
                                              typeof(ModEntry), nameof(ModEntry.Postfix_setUpBundleSpecificPage));
                
                patchPostfix(harmonyInstance, typeof(JunimoNoteMenu), "gameWindowSizeChanged",
                                              typeof(ModEntry), nameof(ModEntry.Postfix_gameWindowSizeChanged));
                
                patchPostfix(harmonyInstance, typeof(JunimoNoteMenu), "draw",
                                              typeof(ModEntry), nameof(ModEntry.Postfix_draw),
                                              new Type[] { typeof(SpriteBatch) });
            }
            catch (Exception e)
            {
                Log("Error in mod setup: " + e.Message + Environment.NewLine + e.StackTrace);
            }
        }
        
        /// <summary>Attempts to patch the given source method with the given postfix method.</summary>
        /// <param name="harmonyInstance">The Harmony instance to patch with.</param>
        /// <param name="sourceClass">The class the source method is part of.</param>
        /// <param name="sourceName">The name of the source method.</param>
        /// <param name="patchClass">The class the patch method is part of.</param>
        /// <param name="patchName">The name of the patch method.</param>
        /// <param name="sourceParameters">The source method's parameter list, when needed for disambiguation.</param>
        void patchPostfix(HarmonyInstance harmonyInstance, Type sourceClass, string sourceName, Type patchClass, string patchName, Type[] sourceParameters = null)
        {
            try
            {
                MethodBase sourceMethod = AccessTools.Method(sourceClass, sourceName, sourceParameters);
                HarmonyMethod postfixPatch = new HarmonyMethod(patchClass, patchName);
                
                if (sourceMethod != null && postfixPatch != null)
                    harmonyInstance.Patch(sourceMethod, postfix: postfixPatch);
                else
                {
                    if (sourceMethod == null)
                        Log("Warning: Source method (" + sourceClass.ToString() + "::" + sourceName + ") not found or ambiguous.");
                    if (postfixPatch == null)
                        Log("Warning: Patch method (" + patchClass.ToString() + "::" + patchName + ") not found.");
                }
            }
            catch (Exception e)
            {
                Log("Error in code patching: " + e.Message + Environment.NewLine + e.StackTrace);
            }
        }
        
        /********************
         ** Method Patches **
         ********************/
        
        /// <summary>After setUpBundleSpecificPage runs, define hover text for ingredients and blank out the originals.</summary>
        /// <param name="__instance">The instance of the bundle menu.</param>
        /// <param name="b">The bundle object.</param>
        public static void Postfix_setUpBundleSpecificPage(JunimoNoteMenu __instance, Bundle b)
        {
            try
            {
                if (__instance.ingredientList == null)
                    return;
                
                if (debugClearCompletedBundles)
                    debugTempClearBundles(__instance);
                if (debugUnlockMissingBundle)
                    debugUnlockMissing();
                
                ingredientHoverTitle = new string[__instance.ingredientList.Count];
                ingredientHoverText = new string[__instance.ingredientList.Count];
                
                for (int i = 0; i < __instance.ingredientList.Count; i++)
                {
                    try
                    {
                        BundleIngredientDescription ingredient = b.ingredients[i];
                        string hintText = ItemHints.getHintText(ingredient.index, ingredient.quality);
                        if (hintText != "")
                        {
                            ingredientHoverTitle[i] = __instance.ingredientList[i].hoverText;
                            ingredientHoverText[i] = hintText;
                            __instance.ingredientList[i].hoverText = "";
                        }
                        else
                        {
                            ingredientHoverTitle[i] = "";
                            ingredientHoverText[i] = "";
                        }
                    }
                    catch (Exception e)
                    {
                        Log("Error defining hint text: " + e.Message + Environment.NewLine + e.StackTrace);
                    }
                }
            }
            catch (Exception e)
            {
                Log("Error in setUpBundleSpecificPage: " + e.Message + Environment.NewLine + e.StackTrace);
            }
        }
        
        /// <summary>gameWindowSizeChanged resets hover text for ingredients, so blank it again after it runs.</summary>
        /// <param name="__instance">The instance of the bundle menu.</param>
        public static void Postfix_gameWindowSizeChanged(JunimoNoteMenu __instance)
        {
            try
            {
                if (__instance.ingredientList == null)
                    return;
                
                for (int i = 0; i < __instance.ingredientList.Count; i++)
                {
                    if (i < ingredientHoverTitle.Length && ingredientHoverTitle[i] != "")
                        __instance.ingredientList[i].hoverText = "";
                }
            }
            catch (Exception e)
            {
                Log("Error in gameWindowSizeChanged: " + e.Message + Environment.NewLine + e.StackTrace);
            }
        }
        
        /// <summary>Draw tooltip for ingredients if cursor is hovering over them.</summary>
        /// <param name="__instance">The instance of the bundle menu.</param>
        /// <param name="b">The sprite batch.</param>
        public static void Postfix_draw(JunimoNoteMenu __instance, SpriteBatch b)
        {
            try
            {
                if (__instance.ingredientList == null || ingredientHoverText == null || ingredientHoverTitle == null)
                    return;
                
                Vector2 mousePosition = Utility.ModifyCoordinatesForUIScale(input.GetCursorPosition().ScreenPixels);
                int x = (int)mousePosition.X, y = (int)mousePosition.Y;
                
                for (int i = 0; i < __instance.ingredientList.Count; i++)
                {
                    if (__instance.ingredientList[i].bounds.Contains(x, y))
                    {
                        if (i >= ingredientHoverText.Length || i >= ingredientHoverTitle.Length)
                            continue;
                        
                        IClickableMenu.drawToolTip(b, ingredientHoverText[i], ingredientHoverTitle[i], null);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log("Error in draw: " + e.Message + Environment.NewLine + e.StackTrace);
            }
        }
        
        /*******************
         ** Debug Methods **
         *******************/
        
        /// <summary>Debug method to temporarily uncomplete all bundles (for the current menu instance).</summary>
        /// <param name="menu">Instance of the golden scroll menu.</param>
        public static void debugTempClearBundles(JunimoNoteMenu menu)
        {
            for (int i = 0; i < menu.bundles.Count; i++)
                menu.bundles[i].complete = false;
        }
        
        /// <summary>Debug method to unlock the Missing Bundle.</summary>
        public static void debugUnlockMissing()
        {
            if (!Game1.MasterPlayer.mailReceived.Contains("abandonedJojaMartAccessible"))
                Game1.MasterPlayer.mailReceived.Add("abandonedJojaMartAccessible");
        }
        
        /// <summary>Prints a message to the SMAPI console.</summary>
        /// <param name="message">The log message.</param>
        public static void Log(string message)
        {
            if (myMonitor != null)
                myMonitor.Log(message, LogLevel.Debug);
        }
    }
}
