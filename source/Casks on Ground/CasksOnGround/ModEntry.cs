/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/CasksOnGround
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using Harmony;
using System.Reflection;

namespace CasksOnGround
{
    using SVObject = Object;
    using Player = Farmer;

    public class ModEntry : Mod
    {
        private static IReflectionHelper _reflection;
        public static Multiplayer Multiplayer => _reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

        public static IMonitor Mon { get; private set; }

        public override void Entry(IModHelper helper)
        {
            _reflection = helper.Reflection;
            Mon = Monitor;
            HarmonyInstance harmony = HarmonyInstance.Create("punyo.CasksOnGround");
            MethodInfo methodBase = typeof(Cask).GetMethod("performObjectDropInAction", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo methodPatcher = typeof(CaskPatcher).GetMethod("Prefix", BindingFlags.Public | BindingFlags.Static);
            
            if(methodBase == null)
            {
                Monitor.Log("Original method null, what's wrong?");
                return;
            }
            if(methodPatcher == null)
            {
                Monitor.Log("Patcher null, what's wrong?");
                return;
            }
            harmony.Patch(methodBase, new HarmonyMethod(methodPatcher), null);
            Monitor.Log($"Patched {methodBase.DeclaringType?.FullName}.{methodBase.Name} by {methodPatcher.DeclaringType?.FullName}.{methodPatcher.Name}");
        }
    }

    public class CaskPatcher
    {
        // ReSharper disable once UnusedMember.Global
        public static bool Prefix(Cask __instance, ref Item dropIn, ref bool probe, ref Player who, ref bool __result)
        {
            __result = PerformObjectDropInAction(__instance, dropIn, probe, who);
            return false;
        }

        public static bool PerformObjectDropInAction(Cask cask, Item dropIn, bool probe, Player who)
        {
            if (dropIn is SVObject o && o.bigCraftable.Value)
            {
                return false;
            }
            if (cask.heldObject.Value != null)
            {
                return false;
            }
            if (cask.Quality >= 4)
            {
                return false;
            }
            
            bool goodItem = false;
            float multiplier = 1f;
            switch (dropIn.ParentSheetIndex)
            {
                case 426:
                    goodItem = true;
                    multiplier = 4f;
                    break;
                case 424:
                    goodItem = true;
                    multiplier = 4f;
                    break;
                case 348:
                    goodItem = true;
                    multiplier = 1f;
                    break;
                case 459:
                    goodItem = true;
                    multiplier = 2f;
                    break;
                case 303:
                    goodItem = true;
                    multiplier = 1.66f;
                    break;
                case 346:
                    goodItem = true;
                    multiplier = 2f;
                    break;
            }

            if (!goodItem)
                return false;
            if (probe)
                return true;
            cask.heldObject.Value = (SVObject)dropIn.getOne();
            cask.agingRate.Value = multiplier;
            cask.MinutesUntilReady = 999999;
            switch (cask.heldObject.Value.Quality)
            {
                case 1:
                    cask.daysToMature.Value = 42f;
                    break;
                case 2:
                    cask.daysToMature.Value = 28f;
                    break;
                case 4:
                    cask.daysToMature.Value = 0f;
                    cask.MinutesUntilReady = 1;
                    break;
                default:
                    cask.daysToMature.Value = 56f;
                    break;
            }
            who.currentLocation.playSound("Ship");
            who.currentLocation.playSound("bubbles");
            ModEntry.Multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, cask.TileLocation * 64f + new Vector2(0f, -128f), false, false, (cask.TileLocation.Y + 1f) * 64f / 10000f + 0.0001f, 0f, Color.Yellow * 0.75f, 1f, 0f, 0f, 0f)
            {
                alphaFade = 0.005f
            });
            return true;
        }
    }
}
