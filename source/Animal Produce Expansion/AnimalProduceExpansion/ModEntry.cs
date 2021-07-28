/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/AnimalProduceExpansion
**
*************************************************/

using AnimalProduceExpansion.Data;
using AnimalProduceExpansion.Patches;
using StardewModdingAPI;

namespace AnimalProduceExpansion
{
  // ReSharper disable once UnusedMember.Global
  public class ModEntry : Mod
  {
    public override void Entry(IModHelper helper)
    {
      Utility.SetLogger(Monitor);
      Utility.SetConfigManager(new ConfigManager.ConfigManager(helper.ReadConfig<Config>()));
      HarmonyPatcher.RegisterPatches();
      _ = new RegisterGameEvents(helper);
    }
  }
}