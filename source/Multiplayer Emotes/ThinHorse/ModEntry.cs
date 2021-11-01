/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using ThinHorse.Framework.Patches;

namespace ThinHorse {

  public class ModEntry : Mod {

    public static IMonitor ModMonitor { get; private set; }
    public static IModHelper ModHelper { get; private set; }

    public override void Entry(IModHelper helper) {

      ModMonitor = Monitor;
      ModHelper = Helper;

      ModPatchManager patchManager = new(helper, new List<IClassPatch>{
        HorsePatch.GetBoundingBoxPatch.CreatePatch(helper.Reflection),
      });
      patchManager.ApplyPatch();

#if DEBUG
      helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
      helper.Events.Input.ButtonPressed += this.DebugActionsKeyBinds;
#endif
    }

#if DEBUG
    /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {
      if (!Context.IsMainPlayer) return;

      // Initialize and setup debug world.
      SetupDebugWorld();
    }

    private void DebugActionsKeyBinds(object sender, ButtonPressedEventArgs e) {
      switch (e.Button) {
        case SButton.NumPad1:
          var tile = Helper.Input.GetCursorPosition().Tile;
          Game1.game1.parseDebugInput($"horse {tile.X} {tile.Y}");
          break;
        case SButton.NumPad2:
          Game1.player?.mount?.dismount();
          Game1.game1.parseDebugInput("killallhorses");
          break;
        case SButton.NumPad3:
          Game1.game1.parseDebugInput("pausetime");
          break;
      }
    }

    private void SetupDebugWorld() {
      Game1.game1.parseDebugInput("nosave");

      //Game1.game1.parseDebugInput("zoomlevel 40");
      Game1.game1.parseDebugInput("zoomlevel 110");

      if (Context.IsMainPlayer) {

        // Pause time and set it to 09:00
        Game1.game1.parseDebugInput("pausetime");
        Game1.game1.parseDebugInput("invincible");
        Game1.game1.parseDebugInput("time 0900");

        Game1.game1.parseDebugInput("warp Farm 64 15");
        // Game1.game1.parseDebugInput("setUpFarm");
      }

    }
#endif

  }

}
