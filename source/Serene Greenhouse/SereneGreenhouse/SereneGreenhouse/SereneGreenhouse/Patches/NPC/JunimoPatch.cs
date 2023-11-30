/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SereneGreenhouse
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System.Reflection;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Linq;
using StardewValley.Characters;
using Netcode;

namespace SereneGreenhouse.Patches.NPC
{
    [HarmonyPatch]
    public class JunimoPatch
    {
        private static IMonitor monitor = ModEntry.monitor;

        internal static ConstructorInfo TargetMethod()
        {
            return AccessTools.Constructor(typeof(Junimo), new System.Type[] { typeof(Vector2), typeof(int), typeof(bool) });
        }

        internal static void Postfix(ref NetColor ___color)
        {
            if (Game1.currentLocation.Name != "Greenhouse")
            {
                return;
            }

            // Set a random color for the Junimo
            if (Game1.random.NextDouble() < 0.01)
            {
                switch (Game1.random.Next(8))
                {
                    case 0:
                        ___color.Value = Color.Red;
                        break;
                    case 1:
                        ___color.Value = Color.Goldenrod;
                        break;
                    case 2:
                        ___color.Value = Color.Yellow;
                        break;
                    case 3:
                        ___color.Value = Color.Lime;
                        break;
                    case 4:
                        ___color.Value = new Color(0, 255, 180);
                        break;
                    case 5:
                        ___color.Value = new Color(0, 100, 255);
                        break;
                    case 6:
                        ___color.Value = Color.MediumPurple;
                        break;
                    case 7:
                        ___color.Value = Color.Salmon;
                        break;
                }
                if (Game1.random.NextDouble() < 0.01)
                {
                    ___color.Value = Color.White;
                }
            }
            else
            {
                switch (Game1.random.Next(8))
                {
                    case 0:
                        ___color.Value = Color.LimeGreen;
                        break;
                    case 1:
                        ___color.Value = Color.Orange;
                        break;
                    case 2:
                        ___color.Value = Color.LightGreen;
                        break;
                    case 3:
                        ___color.Value = Color.Tan;
                        break;
                    case 4:
                        ___color.Value = Color.GreenYellow;
                        break;
                    case 5:
                        ___color.Value = Color.LawnGreen;
                        break;
                    case 6:
                        ___color.Value = Color.PaleGreen;
                        break;
                    case 7:
                        ___color.Value = Color.Turquoise;
                        break;
                }
            }
        }
    }
}
