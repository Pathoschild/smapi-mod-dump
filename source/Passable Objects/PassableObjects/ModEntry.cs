/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/PassableObjects
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using System.Reflection.Emit;
using StardewValley.Buildings;
using System.IO;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace PassableObjects
{
    using SVObject = StardewValley.Object;

    public class ModEntry : Mod
    {
        public static ModEntry mod;

        public static bool NoClip { get; private set; }

        private readonly static Dictionary<Type, Type> typeToSearch = new Dictionary<Type, Type> {
            { typeof(GameLocation) , typeof(GameLocationPatch)},
            { typeof(DecoratableLocation) , typeof(DecoratableLocationPatch)},
            { typeof(FarmHouse) , typeof(FarmHousePatch)},
            { typeof(MineShaft) , typeof(MineShaftPatch)},
            { typeof(Woods) , typeof(WoodsPatch)},
            { typeof(Mountain) , typeof(MountainPatch)},
            { typeof(Railroad) , typeof(RailroadPatch)},
            { typeof(Beach) , typeof(BeachPatch)}
        };
        private readonly static Dictionary<Type, string> targets = new Dictionary<Type, string>
        {
            { typeof(GameLocationPatch), "isCollidingPosition"},
            { typeof(DecoratableLocationPatch), "isCollidingPosition"},
            { typeof(FarmHousePatch), "isCollidingPosition"},
            { typeof(MineShaftPatch), "isCollidingPosition" },
            { typeof(WoodsPatch), "isCollidingPosition" },
            { typeof(MountainPatch), "isCollidingPosition" },
            { typeof(RailroadPatch), "isCollidingPosition" },
            { typeof(BeachPatch), "isCollidingPosition" }
        };

        public override void Entry(IModHelper helper)
        {
            mod = this;

            helper.ConsoleCommands.Add("noclip", "toggles noclip mode", OnNoClipCommand);

            var harmony = HarmonyInstance.Create("punyo.passableobjects");
            foreach (KeyValuePair<Type, Type> types in typeToSearch)
            {
                if(!targets.ContainsKey(types.Value))
                {
                    continue;
                }
                Type type = types.Key;
                Type typePatcher = typeToSearch[types.Key];
                string targetMethod = targets[typePatcher];

                MethodInfo patcher = null;
                MethodInfo[] methods = null;
                try
                {
                    patcher = typePatcher.GetMethod("Prefix", BindingFlags.Public | BindingFlags.Static);
                    methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(i => i.Name == targetMethod).ToArray();
                }
                catch(Exception e)
                {
                    Monitor.Log(e.ToString(), LogLevel.Error);
                    continue;
                }
                if (patcher == null)
                {
                    Monitor.Log("Patcher is null. What's wrong?", LogLevel.Error);
                    continue;
                }
                foreach (MethodInfo info in methods)
                {
                    if(harmony.GetPatchedMethods().Contains(info))
                    {
                        continue;
                    }
                    if (info != null && patcher != null)
                    {
                        //Patch method
                        harmony.Patch(info, new HarmonyMethod(patcher), null, null);
                    }
                }
            }
        }
        public void ToggleNoClip()
        {
            NoClip = !NoClip;
        }
        public void OnNoClipCommand(string name, string[] args)
        {
            ToggleNoClip();
            string noclip = NoClip ? "on" : "off";
            Monitor.Log($"noclip is now {noclip}.", LogLevel.Info);
        }

        public static bool ProcessNoClipping(ref bool __result)
        {
            if (!NoClip)
            {
                // Calls default code and return
                return true;
            }
            __result = false;
            return false; // Cancel calling default code
        }
    }

    public class GameLocationPatch
    {
        public static bool Prefix(GameLocation __instance, ref bool __result)
        {
            return ModEntry.ProcessNoClipping(ref __result);
        }
    }

    public class DecoratableLocationPatch
    {
        public static bool Prefix(DecoratableLocation __instance, ref bool __result)
        {
            return ModEntry.ProcessNoClipping(ref __result);
        }
    }

    public class FarmHousePatch
    {
        public static bool Prefix(FarmHouse __instance, ref bool __result)
        {
            return ModEntry.ProcessNoClipping(ref __result);
        }
    }

    public class MineShaftPatch
    {
        public static bool Prefix(MineShaft __instance, ref bool __result)
        {
            return ModEntry.ProcessNoClipping(ref __result);
        }
    }

    public class WoodsPatch
    {
        public static bool Prefix(Woods __instance, ref bool __result)
        {
            return ModEntry.ProcessNoClipping(ref __result);
        }
    }

    public class MountainPatch
    {
        public static bool Prefix(Mountain __instance, ref bool __result)
        {
            return ModEntry.ProcessNoClipping(ref __result);
        }
    }

    public class RailroadPatch
    {
        public static bool Prefix(Railroad __instance, ref bool __result)
        {
            return ModEntry.ProcessNoClipping(ref __result);
        }
    }

    public class BeachPatch
    {
        public static bool Prefix(Beach __instance, ref bool __result)
        {
            return ModEntry.ProcessNoClipping(ref __result);
        }
    }
}
