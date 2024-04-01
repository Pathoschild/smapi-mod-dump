/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RuiNtD/SVRichPresence
**
*************************************************/

using StardewModdingAPI;

namespace SVRichPresence
{
  internal class ModConfig
  {
    public SButton ReloadConfigButton = SButton.F5;
    public bool ShowGlobalPlayTime = false;
    public bool AddGetModButton = true;
    public MenuPresence MenuPresence = new();
    public GamePresence GamePresence = new();
  }

  internal class MenuPresence
  {
    public string State = "In Menus";
    public string Details = "";
    public string LargeImageText = "{ Activity }";
    public string SmallImageText = "";
    public bool ForceSmallImage = false;
  }

  internal class GamePresence : MenuPresence
  {
    public bool ShowSeason = true;
    public bool ShowFarmType = true;
    public bool ShowWeather = true;
    public bool ShowPlayTime = true;

    public GamePresence()
    {
      State = "{ GameInfo }";
      Details = "{Farm} | {Money}";
      SmallImageText = "{ Date }";
    }
  }
}
