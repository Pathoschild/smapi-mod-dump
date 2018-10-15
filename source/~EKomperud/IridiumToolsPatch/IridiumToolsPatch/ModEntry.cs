using System;
using Harmony;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;
using StardewModdingAPI;
using StardewValley;

namespace IridiumToolsPatch
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public static ModConfig config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();

            HarmonyInstance harmony = HarmonyInstance.Create("Redwood.PatchedIridiumTools");
            Type[] types = new Type[] {typeof(Vector2), typeof(int), typeof(Farmer)};
            MethodInfo originalToolsMethod = typeof(Tool).GetMethod("tilesAffected", BindingFlags.Instance | BindingFlags.NonPublic, null, types, null);
            MethodInfo iridiumToolsPatch = typeof(PatchedIridiumTilesAffected).GetMethod("Postfix");
            harmony.Patch(originalToolsMethod, null, new HarmonyMethod(iridiumToolsPatch));
        }
    }

    class PatchedIridiumTilesAffected
    {
        static public void Postfix(ref List<Vector2> __result, Vector2 tileLocation, int power, Farmer who)
        {
            if (power == 5)
            {
                __result.Clear();
                Vector2 direction;
                Vector2 orthogonal;

                switch (who.FacingDirection)
                {
                    case 0:
                        direction = new Vector2(0, -1); orthogonal = new Vector2(1, 0);
                        break;
                    case 1:
                        direction = new Vector2(1, 0); orthogonal = new Vector2(0, 1);
                        break;
                    case 2:
                        direction = new Vector2(0, 1); orthogonal = new Vector2(-1, 0);
                        break;
                    case 3:
                        direction = new Vector2(-1, 0); orthogonal = new Vector2(0, -1);
                        break;
                    default:
                        direction = Vector2.Zero; orthogonal = Vector2.Zero;
                        break;
                }

                int length = ModEntry.config.length;
                int radius = ModEntry.config.radius;

                for (int x = 0; x < length; x++)
                {
                    __result.Add(direction * x + tileLocation);
                    for (int y = 1; y <= radius; y++)
                    {
                        __result.Add((direction * x) + (orthogonal * y) + tileLocation);
                        __result.Add((direction * x) + (orthogonal * -y) + tileLocation);
                    }
                }
            }
        }
    }
}
