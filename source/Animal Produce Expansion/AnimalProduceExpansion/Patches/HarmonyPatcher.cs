/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/AnimalProduceExpansion
**
*************************************************/

using System;
using System.Reflection;
using Harmony;

namespace AnimalProduceExpansion.Patches
{
  internal class HarmonyPatcher
  {
    internal static void RegisterPatches()
    {
      try
      {
        var harmonyInstance = HarmonyInstance.Create("elbe.AnimalShopConditions");
        harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
      }
      catch (Exception ex)
      {
        Utility.LogError(ex.Message, ex);
      }
    }
  }
}