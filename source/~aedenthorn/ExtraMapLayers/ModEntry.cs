/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using xTile;
using xTile.Dimensions;
using xTile.Display;
using xTile.Layers;
using xTile.Tiles;
using Rectangle = xTile.Dimensions.Rectangle;

namespace ExtraMapLayers
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        public static IMonitor PMonitor;
        public static IModHelper PHelper;
        private Harmony harmony;
        public static ModEntry context;
        public static ModConfig config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            context = this;
            config = Helper.ReadConfig<ModConfig>();

            if (!config.EnableMod)
                return;

            PMonitor = Monitor;
            PHelper = helper;


            harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(Layer), nameof(Layer.Draw)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Layer_Draw_Postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(Layer), "DrawNormal"),
               transpiler: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Layer_DrawNormal_Transpiler))
            );
            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            Monitor.Log($"device type {Game1.mapDisplayDevice?.GetType()}");

            harmony.Patch(
               original: AccessTools.Method(Game1.mapDisplayDevice.GetType(), "DrawTile"),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.DrawTile_Prefix))
            );
        }

        public static IEnumerable<CodeInstruction> Layer_DrawNormal_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            PMonitor.Log("Transpiling Layer_DrawNormal");
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt && (MethodInfo)codes[i].operand == AccessTools.Method("String:Equals", new Type[]{typeof(string)}) && i > 0 && codes[i - 1].opcode == OpCodes.Ldstr && (string)codes[i - 1].operand == "Front")
                {
                    PMonitor.Log("switching equals to starts with");
                    codes[i].operand = AccessTools.Method("String:StartsWith", new Type[] { typeof(string) }, null);
                }
            }
            return codes.AsEnumerable();
        }
        public static int thisLayerDepth = 0;
        public static void Layer_Draw_Postfix(Layer __instance)
        {
            if (!config.EnableMod || Regex.IsMatch(__instance.Id, "[0-9]$"))
                return;

            foreach (Layer layer in Game1.currentLocation.Map.Layers)
            {
                Match match = Regex.Match(layer.Id, $"^{__instance.Id}([0-9]+)$");
                if (match.Success)
                {
                    thisLayerDepth = int.Parse(match.Groups[1].Value);
                    layer.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
                    thisLayerDepth = 0;
                }
            }
        }
        public static int lastLayerDepth = 0;
        public static void DrawTile_Prefix(ref float layerDepth)
        {
            if (!config.EnableMod || thisLayerDepth == 0)
                return;
            if(lastLayerDepth != thisLayerDepth)
            {
                lastLayerDepth = thisLayerDepth;
            }
            layerDepth += thisLayerDepth / 10000f;
        }
    }
}