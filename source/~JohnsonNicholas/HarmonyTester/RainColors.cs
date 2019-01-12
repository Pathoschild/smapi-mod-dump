using Harmony;
using System;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Color = Microsoft.Xna.Framework.Color;
using StardewModdingAPI.Events;
using StardewValley;

namespace HarmonyTester
{
    public class RainColors : Mod
    {
        private static int RainColor = 0;
        private static int SnowColor = 0;
        private static int BackingColor = 0;

        private bool SequenceRC = true;
        private bool SequenceSC = true;
        private bool SequenceBC = true;

        private static readonly List<Color> OurColors = new List<Color>() { Color.DarkRed, Color.OrangeRed, Color.DarkOrange, Color.Orange, Color.LightGoldenrodYellow, Color.Yellow, Color.GreenYellow, Color.Green, Color.LightSeaGreen, Color.Blue, Color.BlueViolet, Color.Indigo, Color.Violet};
        private static readonly int ColorCount = 13;
        public static Sprites.Icons OurIcons { get; set; }

        public static IMonitor Logger;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Logger = Monitor;
            OurIcons = new Sprites.Icons(Helper.Content);

            HarmonyInstance.DEBUG = true;
            var harmony = HarmonyInstance.Create("koihimenakamura.harmonytester");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
            //patch SGame::DarkImpl
            Type t = AccessTools.TypeByName("StardewModdingAPI.Framework.SGame");
            MethodInfo SGameDrawImpl = AccessTools.Method(t, "DrawImpl");
            HarmonyMethod DrawTrans = new HarmonyMethod(AccessTools.Method(typeof(SGamePatches),"Transpiler"));
            harmony.Patch(SGameDrawImpl, null, null, DrawTrans);

            //patch GameLocation::drawAboveAlwaysFrontLayer
            MethodInfo GameLocationDAAFL = AccessTools.Method(typeof(StardewValley.GameLocation), "drawAboveAlwaysFrontLayer");
            HarmonyMethod DAAFLTranspiler = new HarmonyMethod(AccessTools.Method(typeof(GameLocationPatches),"Transpiler"));
            harmony.Patch(GameLocationDAAFL, null, null, DAAFLTranspiler);

            //patch ShippingMenu::draw
            MethodInfo ShippingMenuDraw = AccessTools.Method(typeof(StardewValley.Menus.ShippingMenu), "draw");
            HarmonyMethod MenuTranspiler = new HarmonyMethod(AccessTools.Method(typeof(ShippingMenuPatches),"Transpiler"));
            harmony.Patch(ShippingMenuDraw, null, null, MenuTranspiler);

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        public static Color GetRainColor()
        {
            return OurColors[RainColor];
        }

        public static Color GetBackRainColor()
        {
            return OurColors[BackingColor];
        }

        public static Color GetSnowColor()
        {
            return OurColors[SnowColor];
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (!(Game1.currentLocation is null) && !Game1.currentLocation.IsOutdoors)
                return;

            if (BackingColor == ColorCount - 1)
                SequenceBC = false;
            if (RainColor == ColorCount - 1)
                SequenceRC = false;
            if (SnowColor == ColorCount - 1)
                SequenceSC = false;

            if (!SequenceBC && (BackingColor == 0))
                SequenceBC = true;
            if (!SequenceRC && (RainColor == 0))
                SequenceRC = true;
            if (!SequenceSC && (SnowColor == 0))
                SequenceSC = true;

            if (e.IsMultipleOf(15))
            {
                if (SequenceBC)
                    RainColors.BackingColor++;
                else
                    RainColors.BackingColor--;
            }

            if (e.IsMultipleOf(8))
            {
                if (SequenceSC)
                    RainColors.SnowColor++;
                else
                    RainColors.SnowColor--;

                if (SequenceRC)
                    RainColors.RainColor++;
                else
                    RainColors.RainColor--;
            }
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Game1.isRaining = false;
            Game1.isSnowing = false;
        }
    }

    public static class SGamePatches
    {
        public static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            bool OpNotFound = true, OpNotFoundB = true;

            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldelema && codes[i].operand.ToString().Contains("RainDrop"))
                {
                    var startIndex = i + 1;

                    for (int j = startIndex; j < codes.Count; j++)
                    {
                        if (codes[j].opcode == OpCodes.Call && OpNotFound)
                        {
                            if (codes[j].operand.ToString().Contains("get_White()"))
                            {
                                codes[j].operand = AccessTools.Method(typeof(RainColors), "GetRainColor", new Type[] { });
                                OpNotFound = false;
                            }
                        }
                    }
                }

                if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("get_Blue") && OpNotFoundB)
                {
                    codes[i].operand = AccessTools.Method(typeof(RainColors), "GetBackRainColor", new Type[] { });
                    OpNotFoundB = false;
                }
            }

            return codes.AsEnumerable();
        }
    }

    public static class ShippingMenuPatches
    {
        public static IEnumerable<CodeInstruction> Transpiler(MethodBase original,
            IEnumerable<CodeInstruction> instructions)
        {
            bool StopLoop = false;
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Bne_Un)
                {
                    codes[i].opcode = OpCodes.Ble_Un_S;
                    for (int j = i; j < codes.Count; j++)
                    {
                        if (codes[j].opcode == OpCodes.Ldsfld && codes[j].operand.ToString().Contains("mouseCursors"))
                        {
                            codes[j].operand = AccessTools.Field(typeof(Sprites.Icons), "MoonSource");
                            var insertPoint = j + 8;
                            codes[insertPoint].operand =
                                RainColors.OurIcons.GetNightMoonSprite(MoonPhase.WaxingGibbeous).X;
                            insertPoint++;
                            codes[insertPoint].operand =
                                RainColors.OurIcons.GetNightMoonSprite(MoonPhase.WaxingGibbeous).Y;
                            insertPoint++;
                            codes[insertPoint].operand =
                                RainColors.OurIcons.GetNightMoonSprite(MoonPhase.WaxingGibbeous).Width;
                            insertPoint++;
                            codes[insertPoint].operand =
                                RainColors.OurIcons.GetNightMoonSprite(MoonPhase.WaxingGibbeous).Height;
                            StopLoop = true;
                        }

                        if (StopLoop)
                            break;
                    }
                }
                if (StopLoop)
                    break;
            }
            return codes.AsEnumerable();
        }
    }

    public static class GameLocationPatches
    {
        public static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("get_White"))
                {
                    codes[i].operand = AccessTools.Method(typeof(RainColors), "GetSnowColor", new Type[] { });
                }
            }
            return codes.AsEnumerable();
        }
    }
}
