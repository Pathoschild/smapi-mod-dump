/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDV.Shared.Abstractions;
using StardewValley;

namespace DialogueExtension.Tests.MockWrappers
{
  public class MockGameWrapper : DynamicObject, IGameWrapper
  {
    public MockGameWrapper(params object[] args)
    {
      if (args.Length > 0 && args[0] is Game1)
        GetBaseType = (Game1)args[0];
    }

    public Game1 GetBaseType { get; }
    public bool CanTakeScreenshots() => false;

    public bool CanBrowseScreenshots() => false;

    public bool CanZoomScreenshots() => false;

    public void BrowseScreenshots()
    {
    }

    public string takeMapScreenshot(float? in_scale, string screenshot_name, Action onDone) => null;

    public void CleanupReturningToTitle()
    {
    }

    public bool IsActiveNoOverlay { get; }
    public bool useUnscaledLighting { get; set; }
    public bool IsSaving { get; set; }
    public RenderTarget2D screen { get; set; }
    public RenderTarget2D uiScreen { get; set; }
    public void TranslateFields()
    {
    }

    public void exitEvent(object sender, EventArgs e)
    {
    }

    public void refreshWindowSettings()
    {
    }

    public void Window_ClientSizeChanged(object sender, EventArgs e)
    {
    }

    public void SetWindowSize(int w, int h)
    {
    }

    public void Instance_Initialize()
    {
    }

    public void Instance_LoadContent()
    {
    }

    public void SetNewGameOption<T>(string key, T val)
    {
    }

    public T GetNewGameOption<T>(string key) => default;

    public bool IsFirstInstanceAtThisLocation(IGameLocationWrapper location, Func<Game, bool> additional_check = null) => false;

    public bool IsLocalCoopJoinable() => false;

    public void Instance_UnloadContent()
    {
    }

    public void errorUpdateLoop()
    {
    }

    public void CheckGamepadMode()
    {
    }

    public void Instance_Update(GameTime gameTime)
    {
    }

    public void Instance_OnActivated(object sender, EventArgs args)
    {
    }

    public bool HasKeyboardFocus() => false;

    public void ShowScreenScaleMenu()
    {
    }

    public bool ShowLocalCoopJoinMenu() => false;

    public void SetOtherLocationWeatherForTomorrow(Random random)
    {
    }

    public bool ShouldDismountOnWarp(IHorseWrapper mount, IGameLocationWrapper old_location, IGameLocationWrapper new_location) => false;

    public bool parseDebugInput(string debugInput) => false;

    public void RecountWalnuts()
    {
    }

    public void ResetIslandLocations()
    {
    }

    public void ShowTelephoneMenu()
    {
    }

    public void requestDebugInput()
    {
    }

    public void Instance_Draw(GameTime gameTime)
    {
    }

    public bool ShouldDrawOnBuffer() => false;

    public bool checkCharacterTilesForShadowDrawFlag(ICharacterWrapper character) => false;

    public void drawWeather(GameTime time, RenderTarget2D target_screen)
    {
    }

    public void DrawSplitScreenWindow()
    {
    }

    public void drawMouseCursor()
    {
    }

    public void drawBillboard()
    {
    }

    public bool checkBigCraftableBoundariesForFrontLayer() => false;

    public void _PerformRemoveNormalItemFromWorldOvernight(int parent_sheet_index)
    {
    }

    public int year;
    public string currentSeason;
    public int dayOfMonth;
    public Func<int, string> shortDayNameFromDayOfSeason;
    public Player player = new Player();
    public Func<string, bool> isLocationAccessible;


    public class Player
    {
      public string spouse { get; set; }

      public IDictionary<string, IFriendshipWrapper> friendshipData { get; set; } =
        new Dictionary<string, IFriendshipWrapper>();
      public bool HasTownKey { get; set; }
    }

    //game.year = year;
    //game.currentSeason = season.ToString();
    //game.dayOfMonth = day;
    //game.shortDayNameFromDayOfSeason = new Func<int, string>(i => weekday.ToString());
    //game.player.spouse = spouse;
    //game.player.friendshipData = new Dictionary<string, IFriendshipWrapper>();
    //if (hearts > 0)
    //{
    //  var friendship = _factory.CreateInstance<IFriendshipWrapper>();
    //  friendship.Points = hearts;
    //  ((Dictionary<string, IFriendshipWrapper>) game.player.friendshipData).Add(name, friendship);
    //}

    //game.isLocationAccessible = new Func<string, bool>(s => cc);
    //game.player.HasTownKey = key;
  }
}
