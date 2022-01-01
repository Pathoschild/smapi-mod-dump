using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;
using StardewValley;
using HarmonyLib;

namespace RejuvenatingForest
{
    internal abstract class HarmonyPatches
    {
        // Used by HarmonyPatcher.cs to identify all classes that patch existing code
        // Prerequisite: Class names must contain "_Patch"
    }
}
