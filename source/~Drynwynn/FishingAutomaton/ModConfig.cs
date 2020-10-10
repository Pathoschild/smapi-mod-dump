/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Drynwynn/StardewValleyMods
**
*************************************************/

using DsStardewLib.Config;
using StardewModdingAPI;

namespace FishingAutomaton
{
  public class ModConfig : HarmonyConfig
  {
    // Test if notes go in the config file.
    public bool alwaysPerfect { get; set; } = false;
    public SButton alwaysPerfectButton { get; set; } = SButton.None;

    /// <summary>
    /// If true, fishing will always spawn a treasure.
    /// </summary>
    public bool alwaysTreasure { get; set; } = false;
    public SButton alwaysTreasureButton { get; set; } = SButton.None;

    /// <summary>
    /// Will automatically cast the fishing rod if it's equipped and they player can do so.
    /// Highly recommend setting autoCastButton so this value can be toggled easily.
    /// </summary>
    public bool autoCast { get; set; } = false;
    public SButton autoCastButton { get; set; } = SButton.N;

    /// <summary>
    /// Auto finish will clear the caught fish popup and take any treasure that was caught.
    /// </summary>
    /// <remarks>
    /// Any treasure will be lost if your inventory is full.  Also clearing the caught fish
    /// popup requires the use of Harmony due to how the game is coded, so autofinish will only
    /// process the treasure if HarmonyLoad is false.
    /// </remarks>
    public bool autoFinish { get; set; } = true;
    public SButton autoFinishButton { get; set; } = SButton.None;

    /// <summary>
    /// Will automatically hook the fish/trash (and start the minigame if fish) when HIT occurs.
    /// </summary>
    public bool autoHook { get; set; } = true;
    public SButton autoHookButton { get; set; } = SButton.None;

    /// <summary>
    /// If true, the fishing minigame will be circumvented and the fish will be automatically caught.
    /// </summary>
    /// <remarks>
    /// If this if true, the fish will still be automatically caught even if catchFish is
    /// set to false
    /// </remarks>
    public bool boringFishing { get; set; } = false;
    public SButton boringFishingButton { get; set; } = SButton.None;

    /// <summary>
    /// Play the fishing minigame for you if true.  This option tries to emulate a player holding the button
    /// down as if they were actually playing.  Therefore your fishing level and tackle still come in to
    /// play.  The mod will find it hard to catch a lava eel at low level with the bamboo rod.
    /// </summary>
    /// </remarks>Note that boringFishing is a separate option.</remarks>
    public bool catchFish { get; set; } = true;
    public SButton catchFishButton { get; set; } = SButton.None;

    /// <summary>
    /// If treasure is caught during the minigame, mod will automatically add it to inventory if
    /// this is true.  See pauseTreasureOnError for more.
    /// </summary>
    public bool catchTreasure { get; set; } = true;
    public SButton catchTreasureButton { get; set; } = SButton.None;

    public bool HarmonyDebug { get; set; } = false;

    /// <summary>
    /// Harmony is required to dismiss the caught fish popup and to play the minigame.  If this is
    /// set to false, neither will operate.
    /// </summary>
    public bool HarmonyLoad { get; set; } = true;

    /// <summary>
    /// If the power bar during casting should always generate "Max" power
    /// </summary>
    public bool maxCastPower { get; set; } = true;
    public SButton maxCastPowerButton { get; set; } = SButton.None;

    public bool noSeaweed { get; set; } = false;
    public bool noTrash { get; set; } = false;

    /// <summary>
    /// If there is an error while getting treasure such as a full inventory, pause the acquisition of auto
    /// treasure collection until resolved.  If this is false, any treasure left in the chest when your inventory
    /// is full (or other error) will be lost.  Does nothing if catchTreasure if false.
    /// </summary>
    public bool pauseTreasureOnError { get; set; } = true;

    /// <summary>
    /// Once the bobber is in the water, if true the fish or trash will immediately cause a HIT
    /// </summary>
    public bool quickHook { get; set; } = true;
    public SButton quickHookButton { get; set; } = SButton.None;

    public float fishingBarMaxVelWithFish = 1.5F;
    public float fishingBarMaxVelAtBottom = 4.0F;
    public float fishingBarMaxVelWithoutFish = 7.0F;
  }
}
