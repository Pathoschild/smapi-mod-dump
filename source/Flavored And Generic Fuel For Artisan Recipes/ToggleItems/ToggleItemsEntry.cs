/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Inventories;
using StardewValley.GameData.Machines;
using StardewValley.GameData.BigCraftables;
using StardewValley.TokenizableStrings;
using System.Collections.Generic;

namespace ToggleItems {
  internal sealed class ModEntry : Mod {
    internal static new IModHelper helper { get;
    set;
  }
  internal static IMonitor monitor { get; set; }

  private static IDictionary<string, IList<string>> qualifiedIdMap = new Dictionary<string, IList<string>>();

  private ToggleItemsConfig config;

  private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {
    qualifiedIdMap = ToggleItemsContentPackLoader.LoadContentPack(helper, monitor);
  }

  private bool TryGetNextForm(string currentId, IList<string> idList, out string nextId) {
    nextId = null;
    int currentIndex = idList.IndexOf(currentId);
    if (currentIndex < 0) {
      return false;
    }
    int i = currentIndex;
    do {
      if (i == idList.Count - 1) {
        i = 0;
      } else {
        i += 1;
      }
      if (i == currentIndex) {
        break;
      }
      if (StardewValley.ItemRegistry.Exists(idList[i])) {
        nextId = idList[i];
        return true;
      }
    } while (i != currentIndex);
    return false;
  }
  

  private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e) {
    if (config.switchKey.JustPressed()) {
      StardewValley.Item currentItem = Game1.player.CurrentItem;
      if (currentItem != null &&
          qualifiedIdMap.TryGetValue(currentItem.QualifiedItemId, out var idList) &&
          TryGetNextForm(currentItem.QualifiedItemId, idList, out var nextId)) {
        StardewValley.Item newItem =
            ItemRegistry.Create(nextId, currentItem.Stack, currentItem.Quality, true);
        if (newItem != null) {
          Game1.player.removeItemFromInventory(currentItem);
          Game1.player.addItemToInventory(newItem, Game1.player.CurrentToolIndex);
          Game1.player.showCarrying();
        }
      }
    }
  }

  private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
    // get Generic Mod Config Menu's API (if it's installed)
    var configMenu =
        helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
    if (configMenu is null)
      return;

    configMenu.Register(mod: this.ModManifest, reset: () => this.config = new ToggleItemsConfig(),
                        save: () => helper.WriteConfig(this.config));

    configMenu.AddKeybindList(mod: this.ModManifest, name: () => "Toggle Keybind",
                              getValue: () => this.config.switchKey,
                              setValue: value => this.config.switchKey = value);
  }

  public override void Entry(IModHelper modHelper) {
    helper = modHelper;
    monitor = this.Monitor;

    this.config = helper.ReadConfig<ToggleItemsConfig>();

    helper.Events.GameLoop.GameLaunched += OnGameLaunched;
    helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
    helper.Events.Input.ButtonsChanged += OnButtonsChanged;
  }
}
}
