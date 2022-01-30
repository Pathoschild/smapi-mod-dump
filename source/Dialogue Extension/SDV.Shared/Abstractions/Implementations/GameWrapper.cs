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
using SDV.Shared.Abstractions.Utility;
using StardewValley;
using StardewValley.Characters;

namespace SDV.Shared.Abstractions
{
  public class GameWrapper : DynamicStaticWrapper<Game1>, IGameWrapper
  {
    public GameWrapper() {}
    public GameWrapper(Game1 game) => GetBaseType = game;

    public bool CanTakeScreenshots() => GetBaseType.CanTakeScreenshots();

    public bool CanBrowseScreenshots() => GetBaseType.CanBrowseScreenshots();

    public bool CanZoomScreenshots() => GetBaseType.CanZoomScreenshots();

    public void BrowseScreenshots() => GetBaseType.BrowseScreenshots();

    public string takeMapScreenshot(float? in_scale, string screenshot_name, Action onDone) =>
      GetBaseType.takeMapScreenshot(in_scale, screenshot_name, onDone);

    public void CleanupReturningToTitle() => GetBaseType.CleanupReturningToTitle();

    public bool IsActiveNoOverlay => GetBaseType.IsActiveNoOverlay;

    public bool useUnscaledLighting
    {
      get => GetBaseType.useUnscaledLighting;
      set => GetBaseType.useUnscaledLighting = value;
    }

    public bool IsSaving
    {
      get => GetBaseType.IsSaving;
      set => GetBaseType.IsSaving = value;
    }

    public RenderTarget2D screen
    {
      get => GetBaseType.screen;
      set => GetBaseType.screen = value;
    }

    public RenderTarget2D uiScreen
    {
      get => GetBaseType.uiScreen;
      set => GetBaseType.uiScreen = value;
    }

    public void TranslateFields() => GetBaseType.TranslateFields();

    public void exitEvent(object sender, EventArgs e) => GetBaseType.exitEvent(sender, e);

    public void refreshWindowSettings() => GetBaseType.refreshWindowSettings();
    public void Window_ClientSizeChanged(object sender, EventArgs e) => GetBaseType.Window_ClientSizeChanged(sender, e);

    public void SetWindowSize(int w, int h) => GetBaseType.SetWindowSize(w, h);

    public void Instance_Initialize() => GetBaseType.Instance_Initialize();

    public void Instance_LoadContent() => GetBaseType.Instance_LoadContent();

    public void SetNewGameOption<T>(string key, T val) => GetBaseType.SetNewGameOption(key, val);

    public T GetNewGameOption<T>(string key) => GetBaseType.GetNewGameOption<T>(key);

    public bool IsFirstInstanceAtThisLocation(IGameLocationWrapper location,
      Func<Game, bool> additional_check = null) =>
      GetBaseType.IsFirstInstanceAtThisLocation(((IWrappedType<GameLocation>)location).GetBaseType);

    public bool IsLocalCoopJoinable() => GetBaseType.IsLocalCoopJoinable();

    public void Instance_UnloadContent() => GetBaseType.Instance_UnloadContent();

    public void errorUpdateLoop() => GetBaseType.errorUpdateLoop();

    public void CheckGamepadMode() => GetBaseType.CheckGamepadMode();

    public void Instance_Update(GameTime gameTime) => GetBaseType.Instance_Update(gameTime);

    public void Instance_OnActivated(object sender, EventArgs args) => GetBaseType.Instance_OnActivated(sender, args);

    public bool HasKeyboardFocus() => GetBaseType.HasKeyboardFocus();

    public void ShowScreenScaleMenu() => GetBaseType.ShowScreenScaleMenu();

    public bool ShowLocalCoopJoinMenu() => GetBaseType.ShowLocalCoopJoinMenu();

    public void SetOtherLocationWeatherForTomorrow(Random random) =>
      GetBaseType.SetOtherLocationWeatherForTomorrow(random);

    public bool ShouldDismountOnWarp(IHorseWrapper mount, IGameLocationWrapper old_location, IGameLocationWrapper new_location) =>
      GetBaseType.ShouldDismountOnWarp(((IWrappedType<Horse>)mount).GetBaseType, 
        ((IWrappedType<GameLocation>)old_location).GetBaseType, ((IWrappedType<GameLocation>)new_location).GetBaseType);

    public bool parseDebugInput(string debugInput) => GetBaseType.parseDebugInput(debugInput);

    public void RecountWalnuts() => GetBaseType.RecountWalnuts();

    public void ResetIslandLocations() => GetBaseType.ResetIslandLocations();

    public void ShowTelephoneMenu() => GetBaseType.ShowTelephoneMenu();

    public void requestDebugInput() => GetBaseType.requestDebugInput();

    public void Instance_Draw(GameTime gameTime) => GetBaseType.Instance_Draw(gameTime);

    public bool ShouldDrawOnBuffer() => GetBaseType.ShouldDrawOnBuffer();

    public bool checkCharacterTilesForShadowDrawFlag(ICharacterWrapper character) =>
      GetBaseType.checkCharacterTilesForShadowDrawFlag(character.GetBaseType);

    public void drawWeather(GameTime time, RenderTarget2D target_screen) =>
      GetBaseType.drawWeather(time, target_screen);

    public void DrawSplitScreenWindow() => GetBaseType.DrawSplitScreenWindow();

    public void drawMouseCursor() => GetBaseType.drawMouseCursor();

    public void drawBillboard() => GetBaseType.drawBillboard();

    public bool checkBigCraftableBoundariesForFrontLayer() => GetBaseType.checkBigCraftableBoundariesForFrontLayer();

    public void _PerformRemoveNormalItemFromWorldOvernight(int parent_sheet_index) =>
      GetBaseType._PerformRemoveNormalItemFromWorldOvernight(parent_sheet_index);

    public Game1 GetBaseType { get; }
  }
}