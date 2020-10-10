using System;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace RangedTools
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private static IMonitor myMonitor;
        private static ModConfig Config;
        
        public static bool specialClick = false;
        public static Vector2 specialClickLocation = Vector2.Zero;
        
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
                Config = this.Helper.ReadConfig<ModConfig>();
                
                helper.Events.Input.ButtonPressed += this.OnButtonPressed;
                
                HarmonyInstance harmonyInstance = HarmonyInstance.Create(this.ModManifest.UniqueID);
                
                patchPrefix(harmonyInstance, typeof(Farmer), nameof(Farmer.useTool),
                            typeof(ModEntry), nameof(ModEntry.Prefix_useTool));
                
                patchPrefix(harmonyInstance, typeof(Utility), nameof(Utility.isWithinTileWithLeeway),
                            typeof(ModEntry), nameof(ModEntry.Prefix_isWithinTileWithLeeway));
            }
            catch (Exception e)
            {
                Log("Error in mod setup: " + e.Message + Environment.NewLine + e.StackTrace);
            }
        }
        
        /// <summary>Attempts to patch the given source method with the given prefix method.</summary>
        /// <param name="harmonyInstance">The Harmony instance to patch with.</param>
        /// <param name="sourceClass">The class the source method is part of.</param>
        /// <param name="sourceName">The name of the source method.</param>
        /// <param name="patchClass">The class the patch method is part of.</param>
        /// <param name="patchName">The name of the patch method.</param>
        void patchPrefix(HarmonyInstance harmonyInstance, System.Type sourceClass, string sourceName, System.Type patchClass, string patchName)
        {
            try
            {
                MethodBase sourceMethod = AccessTools.Method(sourceClass, sourceName);
                HarmonyMethod prefixPatch = new HarmonyMethod(patchClass, patchName);
                
                if (sourceMethod != null && prefixPatch != null)
                    harmonyInstance.Patch(sourceMethod, prefixPatch);
                else
                {
                    if (sourceMethod == null)
                        Log("Warning: Source method (" + sourceClass.ToString() + "::" + sourceName + ") not found.");
                    if (prefixPatch == null)
                        Log("Warning: Patch method (" + patchClass.ToString() + "::" + patchName + ") not found.");
                }
            }
            catch (Exception e)
            {
                Log("Error in code patching: " + e.Message + Environment.NewLine + e.StackTrace);
            }
        }
        
        /******************
         ** Input Method **
         ******************/
        
        /// <summary>Checks whether to set ToolLocation override when Tool Button is pressed.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!Context.IsWorldReady || Game1.player == null) // If no save is loaded yet or player doesn't exist
                    return;
                
                if (!isAppropriateTimeToUseTool()) // Tools not allowed in various circumstances
                    return;
                
                bool withClick = e.Button.ToString().Contains("Mouse");
                
                // Tool Button was pressed; required to be mouse button if that setting is enabled.
                if (e.Button.IsUseToolButton() && (withClick || !Config.CustomRangeOnClickOnly))
                {
                    Farmer player = Game1.player;
                    Vector2 mousePosition = new Vector2(e.Cursor.AbsolutePixels.X, e.Cursor.AbsolutePixels.Y);
                    
                    if (player.CurrentTool != null && !player.UsingTool) // Have a tool selected, not in the middle of using it
                    {
                        specialClick = false; // Reset override flag
                        
                        Tool currentTool = player.CurrentTool;
                        
                        // If setting is enabled, face all mouse clicks when a tool/weapon is equipped.
                        if (withClick && shouldToolTurnToFace(currentTool))
                            player.faceGeneralDirection(mousePosition);
                        
                        if (isToolOverridable(currentTool)) // Only override ToolLocation for applicable tools
                        {
                            int range = getCustomRange(currentTool);
                            
                            // Click position is within tool's range, or range setting is negative (infinite).
                            if (range < 0 || Utility.withinRadiusOfPlayer((int)mousePosition.X, (int)mousePosition.Y, range, player))
                            {
                                bool usableOnPlayerTile = getPlayerTileSetting(Game1.player.CurrentTool);
                                
                                // If not allowed to use on player tile, ensure click is not within radius 0 of player.
                                if (usableOnPlayerTile || !Utility.withinRadiusOfPlayer((int)mousePosition.X, (int)mousePosition.Y, 0, player))
                                {
                                    // Set this location as an override to be used a bit later, when the tool is actually used.
                                    specialClick = true;
                                    specialClickLocation = mousePosition;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error in button press: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        
        /// <summary>Returns whether tools can be used currently. Does not check whether player is in the middle of using tool.</summary>
        private bool isAppropriateTimeToUseTool()
        {
            return !Game1.fadeToBlack
                && !Game1.dialogueUp
                && !Game1.eventUp
                && !Game1.menuUp
                && Game1.currentMinigame == null
                && !Game1.player.isRidingHorse()
                && (Game1.CurrentEvent == null || Game1.CurrentEvent.canPlayerUseTool());
        }
        
        /// <summary>Returns whether the given tool is a type that supports ToolLocation override.</summary>
        /// <param name="tool">The Tool being checked.</param>
        private bool isToolOverridable(Tool tool)
        {
            return tool is Axe
                || tool is Pickaxe
                || tool is Hoe
                || tool is WateringCan;
        }
        
        /// <summary>Returns whether the given tool is a type that should face mouse clicks (i.e. sprite won't glitch).</summary>
        /// <param name="tool">The Tool being checked.</param>
        private bool shouldToolTurnToFace(Tool tool)
        {
            return (tool is Axe && Config.ToolAlwaysFaceClick)
                || (tool is Pickaxe && Config.ToolAlwaysFaceClick)
                || (tool is Hoe && Config.ToolAlwaysFaceClick)
                || (tool is WateringCan && Config.ToolAlwaysFaceClick)
                || (tool is MeleeWeapon && Config.WeaponAlwaysFaceClick);
        }
        
        /// <summary>Returns custom range setting for overridable tools (1 for any others).</summary>
        /// <param name="tool">The Tool being checked.</param>
        private int getCustomRange(Tool tool)
        {
            return tool is Axe? Config.AxeRange
                 : tool is Pickaxe? Config.PickaxeRange
                 : tool is Hoe? Config.HoeRange
                 : tool is WateringCan? Config.WateringCanRange
                 : 1;
        }
       
        /// <summary>Returns "usable on player tile" setting for overridable tools (1 for any others).</summary>
        /// <param name="tool">The Tool being checked.</param>
        private bool getPlayerTileSetting(Tool tool)
        {
            return tool is Axe? Config.AxeUsableOnPlayerTile
                 : tool is Pickaxe? Config.PickaxeUsableOnPlayerTile
                 : tool is Hoe? Config.HoeUsableOnPlayerTile
                 : true;
        }
        
        /********************
         ** Method Patches **
         ********************/
        
        /// <summary>Prefix to Farmer.useTool that overrides GetToolLocation with click location when specialClick is set.</summary>
        /// <param name="who">The Farmer using the tool.</param>
        public static bool Prefix_useTool(Farmer who)
        {
            try
            {
                if (specialClick) // Override set by earlier click
                {
                    if (who.toolOverrideFunction == null)
                    {
                        if (who.CurrentTool == null)
                            return true; // Go to original function (where it should just terminate due to tool being null, but still)
                        float stamina = who.stamina;
                        specialClick = false;
                        who.CurrentTool.DoFunction(who.currentLocation, (int)ModEntry.specialClickLocation.X, (int)ModEntry.specialClickLocation.Y, 1, who);
                        
                        // Usual post-DoFunction checks from original
                        who.lastClick = Vector2.Zero;
                        who.checkForExhaustion(stamina);
                        Game1.toolHold = 0.0f;
                        return false; // Don't do original function anymore
                    }
                }
                return true; // Go to original function
            }
            catch (Exception e)
            {
                Log("Error in useTool: " + e.Message + Environment.NewLine + e.StackTrace);
                return true; // Go to original function
            }
        }
        
        /// <summary>Rewrite of Utility.isWithinTileWithLeeway that returns true if within object/seed custom range.</summary>
        /// <param name="x">The X location.</param>
        /// <param name="y">The Y location.</param>
        /// <param name="item">The item being placed.</param>
        /// <param name="f">The Farmer placing it.</param>
        /// <param name="__result">The returned result.</param>
        public static bool Prefix_isWithinTileWithLeeway(int x, int y, Item item, Farmer f, ref bool __result)
        {
            try
            {
                bool bigCraftable = (item as StardewValley.Object).bigCraftable;
                
                // Base game relies on short range to prevent placing Crab Pots in unreachable places, so always use default range.
                if (!bigCraftable && item.parentSheetIndex == 710) // Crab Pot
                    return true; // Go to original function
                
                // Though original behavior shows green when placing Tapper as long as highlighted tile is in range,
                // this becomes particularly confusing at longer range settings, so check that there is in fact an empty tree.
                if (bigCraftable && item.parentSheetIndex == 105) // Tapper
                {
                    Vector2 tile = new Vector2(x / 64, y / 64);
                    if (!f.currentLocation.terrainFeatures.ContainsKey(tile) // No special terrain at tile
                     || !(f.currentLocation.terrainFeatures[tile] is Tree) // Terrain at tile is not a tree
                     || f.currentLocation.objects.ContainsKey(tile)) // Tree tile is already occupied
                    {
                        __result = false;
                        return false; // Don't do original function anymore
                    }
                }
                
                int range = item.category == StardewValley.Object.SeedsCategory
                         || item.category == StardewValley.Object.fertilizerCategory? Config.SeedRange
                                                                                    : Config.ObjectPlaceRange;
                
                if (range < 0 || Utility.withinRadiusOfPlayer(x, y, range, f))
                {
                    __result = true;
                    return false; // Don't do original function anymore
                }
                return true; // Go to original function
            }
            catch (Exception e)
            {
                Log("Error in isWithinTileWithLeeway: " + e.Message + Environment.NewLine + e.StackTrace);
                return true; // Go to original function
            }
        }
        
        /*******************
         ** Debug Methods **
         *******************/
        
        /// <summary>Prints a message to the SMAPI console.</summary>
        /// <param name="message">The log message.</param>
        public static void Log(string message)
        {
            if (myMonitor != null)
                myMonitor.Log(message, LogLevel.Debug);
        }
    }
}