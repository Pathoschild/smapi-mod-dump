using DsStardewLib.SMAPI;
using DsStardewLib.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;

namespace FishingAutomaton
{
  /// <summary>The mod entry point.</summary>
  public class ModEntry : Mod
  {
    private DsModHelper<ModConfig> modHelper = new DsModHelper<ModConfig>();
    private HarmonyWrapper hWrapper = new HarmonyWrapper();

    private Logger log;
    private ModConfig config;
    
    private Lib.FishForMe automaton;

    /// <summary>
    /// Entry point for the mod.  Sets up logging, Harmony, and configuration, and preps the mod to fish.
    /// </summary>
    /// <param name="helper"></param>
    public override void Entry(IModHelper helper)
    {
      modHelper.Init(helper, this.Monitor);
      log = modHelper.Log;
      config = modHelper.Config;

      log.Silly("Created log and config for mod entry.  Loading Harmony.");
      hWrapper.InitHarmony(helper, config, log);
      
      log.Silly("Loading event handlers");
      GameEvents.UpdateTick += new EventHandler(OnUpdateTick);
      GameEvents.HalfSecondTick += new EventHandler(OnHalfSecondTick);

      automaton = new Lib.FishForMe(helper, config);
      log.Trace("Finished init, ready for operation");
    }

    /// <summary>
    /// Event handler for the update that happens appx 60 times a second.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnUpdateTick(object sender, EventArgs e)
    {
      if (!Context.IsWorldReady || Game1.player == null || !(Game1.player.CurrentTool is FishingRod))
        return;

      automaton.OnUpdate();
    }

    /// <summary>
    /// Does the mod need to do anything on half second?
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnHalfSecondTick(object sender, EventArgs e)
    {
      // Nothing at the moment, but send off to the automaton
      automaton.OnHalfSecondUpdate();
    }
  }
}