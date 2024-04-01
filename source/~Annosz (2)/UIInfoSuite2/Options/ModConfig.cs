/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UIInfoSuite2
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace UIInfoSuite2.Options;

internal class ModConfig
{
  public bool ShowOptionsTabInMenu { get; set; } = true;
  public string ApplyDefaultSettingsFromThisSave { get; set; } = "JohnDoe_123456789";
  public KeybindList OpenCalendarKeybind { get; set; } = KeybindList.ForSingle(SButton.B);
  public KeybindList OpenQuestBoardKeybind { get; set; } = KeybindList.ForSingle(SButton.H);
}
