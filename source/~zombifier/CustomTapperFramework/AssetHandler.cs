/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Pathoschild.Stardew.Automate;

using SObject = StardewValley.Object;

namespace CustomTapperFramework;

public class AssetHandler {
  private string dataPath;
  public Dictionary<string, TapperModel> data {get; private set;}

  public AssetHandler() {
    // "selph.CustomTapperFramework/Data"
    dataPath = $"{ModEntry.UniqueId}/Data";
  }

  public void OnAssetRequested(object sender, AssetRequestedEventArgs e) {
    if (e.NameWithoutLocale.IsEquivalentTo(this.dataPath)) {
      var dict = new Dictionary<string, TapperModel>();
      // Populate with base game tappers
      dict["(BC)105"] = new TapperModel();
      dict["(BC)105"].AlsoUseBaseGameRules = true;
      dict["(BC)264"] = new TapperModel();
      dict["(BC)264"].AlsoUseBaseGameRules = true;
      e.LoadFrom(() => dict, AssetLoadPriority.Low);
    }
  }

  public void OnAssetReady(object sender, AssetReadyEventArgs e) {
    if (e.NameWithoutLocale.IsEquivalentTo(this.dataPath)) {
      this.data = Game1.content.Load<Dictionary<string, TapperModel>>(this.dataPath);
      // Just in case
      this.data["(BC)105"].AlsoUseBaseGameRules = true;
      this.data["(BC)264"].AlsoUseBaseGameRules = true;
      ModEntry.StaticMonitor.Log("Loaded custom tapper data with " + data.Count + " entries.", LogLevel.Info);
    }
  }

  public void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
    this.data = Game1.content.Load<Dictionary<string, TapperModel>>(this.dataPath);
    ModEntry.StaticMonitor.Log("Loaded custom tapper data with " + data.Count + " entries.", LogLevel.Info);
    IAutomateAPI automate = ModEntry.Helper.ModRegistry.GetApi<IAutomateAPI>("Pathoschild.Automate");
    if (automate != null) {
      automate.AddFactory(new ResourceClumpConnectorFactory());
    }
  }

  public void OnAssetsInvalidated(object sender, AssetsInvalidatedEventArgs e) {
    foreach (var name in e.NamesWithoutLocale) {
      if (name.IsEquivalentTo(this.dataPath)) {
        this.data = Game1.content.Load<Dictionary<string, TapperModel>>(this.dataPath);
        this.data["(BC)105"].AlsoUseBaseGameRules = true;
        this.data["(BC)264"].AlsoUseBaseGameRules = true;
        ModEntry.StaticMonitor.Log("Reloaded custom tapper data with " + data.Count + " entries.", LogLevel.Info);
      }
    }
  }

}
