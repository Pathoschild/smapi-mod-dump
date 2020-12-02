/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LockedSignsMod
{
  public class ModEntry : Mod
  {
    public static new IModHelper Helper;
    public static LCPConfig Config;
    private static bool _isUnlockKeyPressed;

    public override void Entry(IModHelper helper)
    {
      Helper = helper;
      this.ReadConfig();
      this.SubscribeToEvents();
      this.ApplyPatches();
    }

    private void ReadConfig()
    {
      Config = (LCPConfig) Helper.ReadConfig<LCPConfig>();
      if (!(Config.UnlockKey == "control"))
        return;
      Config.UnlockKey = "leftcontrol,rightcontrol";
      Helper.WriteConfig<LCPConfig>(Config);
    }

    private void SubscribeToEvents()
    {
      Helper.Events.Input.ButtonReleased += new EventHandler<ButtonReleasedEventArgs>(this.OnButtonReleased);
      Helper.Events.Input.ButtonPressed += new EventHandler<ButtonPressedEventArgs>(this.OnButtonPressed);
    }

    private void ApplyPatches() => HarmonyInstance.Create("MindMeltMax.LSM").Patch((MethodBase) AccessTools.Method(typeof (Sign), "checkForAction"), new HarmonyMethod(typeof (ModEntry), "Sign_CheckForAction"));

    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
      if (!this.IsButtonUnlockKey(e.Button))
        return;
      _isUnlockKeyPressed = true;
    }

    private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
    {
      if (!this.IsButtonUnlockKey(e.Button))
        return;
      _isUnlockKeyPressed = false;
    }

    private bool IsButtonUnlockKey(SButton button)
    {
      string buttonAsString = button.ToString().ToLower();
      return ((IEnumerable<string>) Config.UnlockKey.ToLower().Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).Any<string>((Func<string, bool>) (Item => buttonAsString.Equals(Item.Trim())));
    }

    private static bool Sign_CheckForAction(
      ref Sign __instance,
      Farmer who,
      bool justCheckingForActivity = false) => _isUnlockKeyPressed || __instance.heldObject.Value != null;

        public ModEntry() : base()
        {
            return;
        }
  }
}
