/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

// Decompiled with JetBrains decompiler
// Type: BetterFarmAnimalVariety.Framework.Events.LoadMod
// Assembly: BetterFarmAnimalVariety, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5915D6B1-6174-4632-A28A-C1734D2C6C57
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\BetterFarmAnimalVariety.dll

using System;
using System.Collections.Generic;
using BetterFarmAnimalVariety.Framework.Commands;
using BetterFarmAnimalVariety.Framework.Editors;
using BetterFarmAnimalVariety.Framework.Helpers;
using BetterFarmAnimalVariety.Framework.Loaders;
using BetterFarmAnimalVariety.Framework.Patches.AnimalHouse;
using BetterFarmAnimalVariety.Framework.Patches.FarmAnimal;
using BetterFarmAnimalVariety.Framework.Patches.PurchaseAnimalsMenu;
using BetterFarmAnimalVariety.Framework.Patches.Utility;
using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using CoopDayUpdate = BetterFarmAnimalVariety.Framework.Patches.Coop.DayUpdate;
using BarnDayUpdate = BetterFarmAnimalVariety.Framework.Patches.Barn.DayUpdate;
using Object = StardewValley.Object;

namespace BetterFarmAnimalVariety.Framework.Events
{
  internal class LoadMod
  {
    public static void OnEntry(ModEntry mod)
    {
      SetUpConfig(mod);
      SetUpHarmonyPatches();
      SetUpAssetLoaders(mod);
      SetUpAssetEditors(mod);
      SetUpConsoleCommands(mod);
    }

    private static void SetUpConfig(ModEntry mod)
    {
      var targetFormat = mod.ModManifest.Version.MajorVersion.ToString();
      ModConfig config;
      try
      {
        config = Mod.ReadConfig<ModConfig>();
        if (config.Format == null)
          config.Format = targetFormat;
        else
          config.AssertValidFormat(targetFormat);
      }
      catch
      {
        MigrateDeprecatedConfig.OnEntry(mod, targetFormat, out config);
      }

      config.Write(mod.Helper);
      if (!config.IsEnabled)
        throw new ApplicationException("Mod is disabled. To enable, set IsEnabled to true in config.json.");
    }

    private static void SetUpHarmonyPatches()
    {
      var harmonyInstance = HarmonyInstance.Create("Paritee.BetterFarmAnimalVariety");
      ModEntry.Instance.Monitor.Log("Applying Harmony patches...");
      harmonyInstance.Patch(AccessTools.Method(typeof(AnimalHouse), "addNewHatchedAnimal"),
        new HarmonyMethod(typeof(AddNewHatchedAnimal), "Prefix"));
      harmonyInstance.Patch(AccessTools.Method(typeof(AnimalHouse), "resetSharedState"),
        postfix: new HarmonyMethod(typeof(ResetSharedState), "Postfix"));
      harmonyInstance.Patch(AccessTools.Method(typeof(Coop), "dayUpdate"),
        new HarmonyMethod(typeof(CoopDayUpdate), "Prefix"));
      harmonyInstance.Patch(AccessTools.Method(typeof(Barn), "dayUpdate"),
        new HarmonyMethod(typeof(BarnDayUpdate), "Prefix"));
      harmonyInstance.Patch(AccessTools.Method(typeof(FarmAnimal), "behaviors"),
        new HarmonyMethod(typeof(Behaviors), "Prefix"));
      harmonyInstance.Patch(AccessTools.Method(typeof(FarmAnimal), "dayUpdate"),
        new HarmonyMethod(typeof(DayUpdate), "Prefix"),
        new HarmonyMethod(typeof(DayUpdate), "Postfix"));
      harmonyInstance.Patch(AccessTools.Method(typeof(FarmAnimal), "findTruffle"),
        new HarmonyMethod(typeof(FindTruffle), "Prefix"));
      harmonyInstance.Patch(AccessTools.Method(typeof(FarmAnimal), "reload"),
        new HarmonyMethod(typeof(Reload), "Prefix"));
      harmonyInstance.Patch(AccessTools.Method(typeof(Object), "DayUpdate"),
        new HarmonyMethod(typeof(Patches.Object.DayUpdate), "Prefix"));
      harmonyInstance.Patch(AccessTools.Constructor(typeof(PurchaseAnimalsMenu), new Type[1]
      {
        typeof(List<Object>)
      }), new HarmonyMethod(typeof(Constructor), "Prefix"), new HarmonyMethod(typeof(Constructor), "Postfix"));
      harmonyInstance.Patch(AccessTools.Method(typeof(PurchaseAnimalsMenu), "draw", new Type[1]
      {
        typeof(SpriteBatch)
      }), new HarmonyMethod(typeof(Draw), "Prefix"));
      harmonyInstance.Patch(AccessTools.Method(typeof(PurchaseAnimalsMenu), "getAnimalDescription"),
        new HarmonyMethod(typeof(GetAnimalDescription), "Prefix"));
      harmonyInstance.Patch(AccessTools.Method(typeof(PurchaseAnimalsMenu), "getAnimalTitle"),
        new HarmonyMethod(typeof(GetAnimalTitle), "Prefix"));
      harmonyInstance.Patch(AccessTools.Method(typeof(PurchaseAnimalsMenu), "receiveLeftClick"),
        new HarmonyMethod(typeof(ReceiveLeftClick), "Prefix"));
      harmonyInstance.Patch(AccessTools.Method(typeof(Utility), "getPurchaseAnimalStock"),
        new HarmonyMethod(typeof(GetPurchaseAnimalStock), "Prefix"));
      ModEntry.Instance.Monitor.Log("... done patching.");
    }

    private static void SetUpConsoleCommands(ModEntry mod)
    {
      foreach (var command in new List<Command>
      {
        new List(mod.Helper, mod.Monitor)
      })
        mod.Helper.ConsoleCommands.Add(command.Name, command.Description, command.Callback);
    }

    private static void SetUpAssetEditors(ModEntry mod)
    {
      mod.Helper.Content.AssetEditors.Add(new AnimalBirth(mod.Helper));
      mod.Helper.Content.AssetEditors.Add(new FarmAnimalData(mod.Helper, mod.Monitor));
    }

    private static void SetUpAssetLoaders(ModEntry mod)
    {
      mod.Helper.Content.AssetLoaders.Add(new FarmAnimalSprites());
    }
  }
}