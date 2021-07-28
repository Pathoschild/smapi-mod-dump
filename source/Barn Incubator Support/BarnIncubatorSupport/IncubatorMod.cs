/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System;
using Harmony;
using StardewModdingAPI;
using Object = StardewValley.Object;



namespace BarnIncubatorSupport
{
  public class IncubatorMod : Mod
  {
    public override void Entry(IModHelper helper)
    {
      try
      {
        CreateHarmonyPatch();
      }
      catch (Exception ex)
      {
        Monitor.Log(ex.Message, LogLevel.Error);
      }
    }

    private void CreateHarmonyPatch()
    {
      var harmonyInstance = HarmonyInstance.Create("elbe.BarnIncubatorSupport");
      Monitor.Log("Applying Harmony patches...");
      harmonyInstance.Patch(AccessTools.Method(typeof(Object), "performObjectDropInAction"),
        new HarmonyMethod(typeof(performObjectDropInAction), "Prefix"));
    }
  }
}