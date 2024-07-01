/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jolly-Alpaca/PrismaticDinosaur
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewModdingAPI;

namespace PrismaticDinosaur.Patches
{
    internal class MonsterPatcher
    {
        private static IMonitor Monitor;

        /// <summary>
        /// Apply the harmony patches to Monster.cs
        /// </summary>
        /// <param name="monitor">The Monitor instance for the PrismaticDinosaurMonster module</param>
        /// <param name="harmony">The Harmony instance for the PrismaticDinosaurMonster module</param>
        internal static void Apply(IMonitor monitor, Harmony harmony)
        {
            Monitor = monitor;
            harmony.Patch(
                original: AccessTools.Constructor(typeof(Monster), new Type[] {typeof(string), typeof(Vector2)}),
                prefix: new HarmonyMethod(typeof(MonsterPatcher), nameof(MonsterConstructor_Prefix))
            );
        }

        /// <summary>
        /// Prefix to patch the Monster constructor in MonsterPatcher.cs
        /// Adds logic to add a 10% chance of spawning a Prismatic Pepper Rex instead of a regular Pepper Rex
        /// </summary>
        /// <remarks>This does not prevent the constructor from running. It only changes the name passed to the constructor.</remarks>
        /// <param name="__instance">The Monster instance being instanciated</param>
        /// <param name="name">The name of the monster being passed to the constructor</param>
        /// <param name="position">The location to spawn the Monster instance</param>
        /// <returns>Always returns true</returns>
        internal static bool MonsterConstructor_Prefix(Monster __instance, ref string name, Vector2 position)
        {
            try
            {
                // 10% change to spawn a prismatic pepper rex instead of a regular pepper rex
                Random dinoRandom = new Random();
                if (name == "Pepper Rex" && dinoRandom.NextDouble() < 0.1)
                {
                    // Change the name passed to the Monster constructor to the Prismatic Pepper Rex name
                    name = "JollyLlama.PrismaticDinosaur.Prismatic Pepper Rex";
                }
                return true;   
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(MonsterConstructor_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}