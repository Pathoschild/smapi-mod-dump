/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/vgperson/CommunityCenterHelper
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;

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
        public static bool debugTreatRecipesAsKnown = false;
        public static bool debugShowUnknownIDs = false;
        public static bool debugAddBundleTestCommand = false;
        
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
                
                if (debugAddBundleTestCommand)
                    helper.ConsoleCommands.Add("testbundlehints", "Tests item hints on a list of JSON Community Center bundle definitions, outputting results.", this.debugTestBundleHints);
                
                Harmony harmonyInstance = new Harmony(this.ModManifest.UniqueID);
                
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
        /// <param name="sourceLiteralName">The source method given as a string, if type cannot be directly accessed.</param>
        void patchPostfix(Harmony harmonyInstance, Type sourceClass, string sourceName, Type patchClass, string patchName, Type[] sourceParameters = null, string sourceLiteralName = "")
        {
            try
            {
                MethodBase sourceMethod;
                if (sourceLiteralName != "")
                    sourceMethod = AccessTools.Method(sourceLiteralName, sourceParameters);
                else
                    sourceMethod = AccessTools.Method(sourceClass, sourceName, sourceParameters);
                
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
            catch (Exception ex)
            {
                Log("Error patching postfix method to " + sourceClass.Name + "." + sourceName + "." + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
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
                        string hintText = ItemHints.getHintText(ingredient.id, ingredient.quality, ingredient.category);
                        if (hintText != "")
                        {
                            ingredientHoverTitle[i] = __instance.ingredientList[i].hoverText;
                            ingredientHoverText[i] = hintText;
                            __instance.ingredientList[i].hoverText = "";
                            
                            // Override item name for generic Dried and Smoked items.
                            if (ingredient.id == ItemID.IT_DriedFruit)
                                ingredientHoverTitle[i] = Game1.content.LoadString("Strings\\Objects:DriedFruit_CollectionsTabName");
                            else if (ingredient.id == ItemID.IT_DriedMushrooms)
                                ingredientHoverTitle[i] = Game1.content.LoadString("Strings\\Objects:DriedMushrooms_CollectionsTabName");
                            else if (ingredient.id == ItemID.IT_SmokedFish)
                                ingredientHoverTitle[i] = Game1.content.LoadString("Strings\\Objects:SmokedFish_CollectionsTabName");
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
        
        /// <summary>Debug method to test item hint function on a list of JSON bundle definitions from clipboard.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void debugTestBundleHints(string command, string[] args)
        {
            bool listItemsInEachBundle = false; // Additional debug to print bundle contents as readable item names
            
            string clipboardText = "";
            if (DesktopClipboard.GetText(ref clipboardText))
            {
                bool oldUnknownIDs = debugShowUnknownIDs;
                debugShowUnknownIDs = false; // Let function return blank string if no hint found
                
                System.IO.StringWriter str = new System.IO.StringWriter(new System.Text.StringBuilder());
                System.Collections.Generic.List<string> printedItemIDs = new System.Collections.Generic.List<string>();
                
                foreach (string line in clipboardText.Split("\n"))
                {
                    if (line.StartsWith("//"))
                    {
                        str.WriteLine(line + "\n");
                        continue;
                    }
                    
                    System.Text.RegularExpressions.Regex bundleDefinition = new System.Text.RegularExpressions.Regex("\"[^\"]*\":[ ]*\"([^\"]*)\"");
                    System.Text.RegularExpressions.Match match = bundleDefinition.Match(line);
                    if (match.Success)
                    {
                        str.WriteLine("<" + line.Replace("\r", "") + ">\n");
                        
                        System.IO.StringWriter allItemsInBundle = null;
                        if (listItemsInEachBundle)
                            allItemsInBundle = new System.IO.StringWriter(new System.Text.StringBuilder());
                        
                        string definitionText = match.Groups[1].Value;
                        if (definitionText.Contains("{{")) // Simplify JSON asset references to just {{}}
                            definitionText = System.Text.RegularExpressions.Regex.Replace(definitionText, "\\{\\{[^\\}]*\\}\\}", "{{}}");
                        
                        if (definitionText.Split("/").Length >= 3) // Original format: data string split by /s using item IDs
                        {
                            string[] definitionSplit = definitionText.Split('/');
                            string ingredientText = definitionSplit.Length > 2? definitionSplit[2] : "";
                            string[] ingredientList = ingredientText.Split(' ');
                        
                            for (int i = 0; i < ingredientList.Length; i += 3)
                            {
                                string itemID = ingredientList[i];
                                if (itemID.Equals("{{}}")) // Skip over old JSON asset references
                                    continue;
                                
                                string itemName = ItemHints.getItemName(itemID);
                                if (listItemsInEachBundle)
                                    allItemsInBundle.Write((i > 0? ", " : "") + itemName);
                                
                                int itemQuality = 0;
                                if (i + 2 < ingredientList.Length)
                                    int.TryParse(ingredientList[i + 2], out itemQuality);
                                
                                if (printedItemIDs.Contains(itemID)) // Already printed hint for this item
                                    continue;
                                printedItemIDs.Add(itemID);
                                
                                string hintText = ItemHints.getHintText(itemID, itemQuality, itemID.StartsWith("-")? int.Parse(itemID) : 0);
                                if (hintText != "")
                                    str.WriteLine(itemName + " [" + itemID + "]\n" + hintText + "\n");
                                else
                                    str.WriteLine("ERROR: No hint for " + itemName + " [" + itemID + "]\n"
                                                + definitionText + "\n");
                            }
                        }
                        else // RandomBundles format: item quantities and names separated by commas
                        {
                            string[] ingredientList = definitionText.Split(", ");
                            System.Collections.Generic.List<string> fullIngredientList = new System.Collections.Generic.List<string>();
                            
                            for (int i = 0; i < ingredientList.Length; i++)
                            {
                                string itemEntry = ingredientList[i];
                                if (itemEntry.StartsWith("[")) // Multiple possible items
                                {
                                    itemEntry = itemEntry.Substring(1, itemEntry.Length - 1);
                                    string[] possibleItems = itemEntry.Split('|');
                                    foreach (string item in possibleItems)
                                        fullIngredientList.Add(item);
                                }
                                else // Single item
                                    fullIngredientList.Add(itemEntry);
                            }
                            
                            for (int i = 0; i < fullIngredientList.Count; i++)
                            {
                                string itemEntry = ingredientList[i];
                                string itemName = itemEntry.Substring(itemEntry.IndexOf(" ") + 1); // Remove quantity at start
                                
                                int itemQuality = 0;
                                if (itemName.StartsWith("SQ "))
                                {
                                    itemQuality = 1;
                                    itemName = itemEntry.Substring(itemEntry.IndexOf(" ") + 1);
                                }
                                else if (itemName.StartsWith("GQ "))
                                {
                                    itemQuality = 2;
                                    itemName = itemEntry.Substring(itemEntry.IndexOf(" ") + 1);
                                }
                                else if (itemName.StartsWith("IQ "))
                                {
                                    itemQuality = 3;
                                    itemName = itemEntry.Substring(itemEntry.IndexOf(" ") + 1);
                                }
                                
                                string itemID = "";
                                if (itemName == "EggCategory")
                                    itemID = StardewValley.Object.EggCategory.ToString();
                                else if (itemName == "MilkCategory")
                                    itemID = StardewValley.Object.MilkCategory.ToString();
                                else
                                    ItemHints.findItemIDByName(itemName, out itemID);
                                if (itemID == "")
                                {
                                    str.WriteLine("ERROR: No item found matching name " + itemName + "\n");
                                    continue;
                                }
                                if (listItemsInEachBundle)
                                    allItemsInBundle.Write((i > 0? ", " : "") + itemName);
                                
                                if (printedItemIDs.Contains(itemID)) // Already printed hint for this item
                                    continue;
                                printedItemIDs.Add(itemID);
                                
                                string hintText = ItemHints.getHintText(itemID, itemQuality, itemID.StartsWith("-")? int.Parse(itemID) : 0);
                                if (hintText != "")
                                    str.WriteLine(itemName + " [" + itemID + "]\n" + hintText + "\n");
                                else
                                    str.WriteLine("ERROR: No hint for " + itemName + " [" + itemID + "]\n"
                                                + definitionText + "\n");
                            }
                        }
                        
                        if (listItemsInEachBundle)
                            str.WriteLine("<Items in Bundle>\n" + line + "\n" + allItemsInBundle.ToString() + "\n");
                    }
                }
                
                Log(str.ToString());
                debugShowUnknownIDs = oldUnknownIDs;
            }
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
