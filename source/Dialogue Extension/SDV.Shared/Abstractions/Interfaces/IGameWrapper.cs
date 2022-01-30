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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public interface IGameWrapper : IWrappedType<Game1>
  {
    bool CanTakeScreenshots();
    bool CanBrowseScreenshots();
    bool CanZoomScreenshots();
    void BrowseScreenshots();
    string takeMapScreenshot(float? in_scale, string screenshot_name, Action onDone);
    void CleanupReturningToTitle();
    bool IsActiveNoOverlay { get; }
    bool useUnscaledLighting { get; set; }
    bool IsSaving { get; set; }
    RenderTarget2D screen { get; set; }
    RenderTarget2D uiScreen { get; set; }
    void TranslateFields();
    void exitEvent(object sender, EventArgs e);
    void refreshWindowSettings();
    void Window_ClientSizeChanged(object sender, EventArgs e);
    void SetWindowSize(int w, int h);
    void Instance_Initialize();
    void Instance_LoadContent();
    void SetNewGameOption<T>(string key, T val);
    T GetNewGameOption<T>(string key);

    bool IsFirstInstanceAtThisLocation(
      IGameLocationWrapper location,
      Func<Game, bool> additional_check = null);

    bool IsLocalCoopJoinable();
    void Instance_UnloadContent();
    void errorUpdateLoop();
    void CheckGamepadMode();
    void Instance_Update(GameTime gameTime);
    void Instance_OnActivated(object sender, EventArgs args);
    bool HasKeyboardFocus();
    void ShowScreenScaleMenu();
    bool ShowLocalCoopJoinMenu();
    void SetOtherLocationWeatherForTomorrow(Random random);

    bool ShouldDismountOnWarp(
      IHorseWrapper mount,
      IGameLocationWrapper old_location,
      IGameLocationWrapper new_location);

    bool parseDebugInput(string debugInput);
    void RecountWalnuts();
    void ResetIslandLocations();
    void ShowTelephoneMenu();
    void requestDebugInput();
    void Instance_Draw(GameTime gameTime);
    bool ShouldDrawOnBuffer();
    bool checkCharacterTilesForShadowDrawFlag(ICharacterWrapper character);
    void drawWeather(GameTime time, RenderTarget2D target_screen);
    void DrawSplitScreenWindow();
    void drawMouseCursor();
    void drawBillboard();
    bool checkBigCraftableBoundariesForFrontLayer();
    void _PerformRemoveNormalItemFromWorldOvernight(int parent_sheet_index);
  }
}