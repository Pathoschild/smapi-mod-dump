using DsStardewLib.SMAPI;
using DsStardewLib.Utils;
using StardewModdingAPI;

namespace NoAddedFlyingMineMonsters
{
  /// <summary>
  /// Mod to remove random flying monsters that spawn in the mines.
  /// </summary>
  public class ModEntry : Mod
  {
    private DsModHelper<ModConfig> modHelper = new DsModHelper<ModConfig>();
    private HarmonyWrapper hWrapper = new HarmonyWrapper();

    private ModConfig config;
    private Logger log;

    /// <summary>
    /// Entry point for the mod.  Uses helpers to set up logging, config, and harmony.
    /// </summary>
    /// <param name="helper">Provided by SMAPI</param>
    public override void Entry(IModHelper helper)
    {
      modHelper.Init(helper, this.Monitor);
      log = modHelper.Log;
      config = modHelper.Config;

      log.Silly("Loaded ModHelper, now attempted to load Harmony");
      
      hWrapper.InitHarmony(helper, config, log);

      log.Trace("Finished init, ready for operation");
    }
  }
}
