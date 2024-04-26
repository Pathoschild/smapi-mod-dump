/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamescodesthings/smapi-better-sprinklers
**
*************************************************/

using System;
using System.Collections.Generic;
using BetterSprinklersPlus.Framework;
using BetterSprinklersPlus.Framework.Helpers;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace BetterSprinklersPlus
{
  /// <summary>
  /// Better Sprinklers Plus
  /// </summary>
  // ReSharper disable once UnusedType.Global
  public class BetterSprinklersPlus : Mod
  {
    /// <summary>
    /// Is F3 mode on?
    /// </summary>
    private bool _showInfoOverlay;

    /// <summary>
    /// Stores recency of sprinkler activation.
    /// </summary>
    private Dictionary<string, Tuple<SObject, DateTime>> _sprinklerActivations = new();

    /// <summary>
    /// The mod entry point, called after the mod is first loaded.
    /// </summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
      Logger.init(Monitor);
      SetUpEvents();
    }

    /// <summary>
    /// Sets up events
    /// </summary>
    private void SetUpEvents()
    {
      Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
      Helper.Events.GameLoop.DayStarted += OnDayStarted;
      Helper.Events.Display.RenderedWorld += OnRenderedWorld;
      Helper.Events.Input.ButtonPressed += OnButtonPressed;
      Helper.Events.Input.ButtonReleased += OnButtonReleased;
    }

    /// <summary>
    /// Get an API that other mods can access.
    /// This is always called after <see cref="Entry" />.
    /// </summary>
    public override object GetApi()
    {
      return new BetterSprinklersPlusApi();
    }

    /// <summary>
    /// Handle game launch
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
      BetterSprinklersPlusConfig.Init(Helper, ModManifest);
    }

    /// <summary>
    /// Raised after drawing the world.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
    {
        RenderHighlight();
    }

    /// <summary>
    /// Raised after the game begins a new day (including when the player loads a save).
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnDayStarted(object sender, DayStartedEventArgs e)
    {
      RunSprinklers();
    }

    /// <summary>
    /// Raised after the player presses a button on the keyboard, controller, or mouse.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
      if (Game1.activeClickableMenu != null || Game1.CurrentEvent != null)
        return;

      if (e.Button == BetterSprinklersPlusConfig.Active.ShowSprinklerEditKey)
      {
        Logger.Verbose($"{e.Button} pressed, opening sprinkler edit");
        ShowSprinklerEditMenu();
        return;
      }

      if (e.Button == BetterSprinklersPlusConfig.Active.ShowOverlayKey)
      {
        Logger.Verbose($"{e.Button} pressed, showing overlay");
        ToggleOverlay();
      }


    }

    private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
    {
      if (e.Button == BetterSprinklersPlusConfig.Active.ActivateKey || e.Button == SButton.ControllerA)
      {
        TryActivateSprinkler(e.Button);
      }
    }

    private void TryActivateSprinkler(SButton button)
    {
      if (Game1.activeClickableMenu != null || Game1.CurrentEvent != null)
        return;

      var cursorPos = Helper.Input.GetCursorPosition();
      var tile = cursorPos.GrabTile;

      if (!Game1.currentLocation.objects.TryGetValue(tile, out StardewValley.Object obj)) return;
      // Check if the object under the cursor is a sprinkler
      // if it is, water and deduct cost appropriately
      if (!SprinklerHelper.SprinklerObjectIds.Contains(obj.ParentSheetIndex)) return;

      // Save sprinkler activation to dict using location + type as key.
      var key = obj.TileLocation.ToString() + "_" + obj.QualifiedItemId;

      // Check if sprinkler was activated in last 'SprinklerCooldown' seconds, if so, don't activate again.
      if (_sprinklerActivations.ContainsKey(key))
      {
        var dictObj = _sprinklerActivations[key].Item1;
        var dictDateTime = _sprinklerActivations[key].Item2;
        if (DateTime.Now <= dictDateTime.AddSeconds(BetterSprinklersPlusConfig.Active.SprinklerCooldown) && dictObj == obj)
        {
          Game1.addHUDMessage(new HUDMessage("Can't run sprinkler, on cooldown", 3));
          return;
        }
        else
        {
          _sprinklerActivations[key] = Tuple.Create(obj, DateTime.Now);
        }
      }
      else
      {
        _sprinklerActivations.Add(key, Tuple.Create(obj, DateTime.Now));
      }

      // Suppress the default action if we are handling it
      Helper.Input.Suppress(button);

      if (BetterSprinklersPlusConfig.Active.BalancedMode == (int)BetterSprinklersPlusConfig.BalancedModeOptions.Off)
      {
        Logger.Verbose($"Sprinkler at {tile.X}x{tile.Y} activated");
        ActivateSprinkler(Game1.currentLocation, tile, obj);
        Game1.addHUDMessage(new HUDMessage("Sprinkler Activated", 2));
        return;
      }

      var type = obj.ParentSheetIndex;
      var hasPressureNozzle = obj.HasPressureNozzle();
      var cost = (int)Math.Round(type.CalculateCostForSprinkler(hasPressureNozzle));

      if (BetterSprinklersPlusConfig.Active.CannotAfford == (int)BetterSprinklersPlusConfig.CannotAffordOptions.DoNotWater && Game1.player.Money < cost)
      {
        Logger.Warn($"Player tried to activate sprinkler but it was too expensive ({cost}G) > {Game1.player.Money}");
        Game1.addHUDMessage(new HUDMessage($"Can't run sprinkler, it will cost too much ({cost}G)", 3));
        return;
      }

      Logger.Verbose($"Sprinkler at {tile.X}x{tile.Y} activated ({cost}G)");
      ActivateSprinkler(Game1.currentLocation, tile, obj);
      DeductCost(cost);
      Game1.addHUDMessage(new HUDMessage($"Sprinkler Activated ({cost}G)", 2));
    }

    /// <summary>
    /// Show the sprinkler edit menu
    /// </summary>
    private void ShowSprinklerEditMenu()
    {
      Logger.Verbose($"ShowSprinklerEditMenu()");
      Game1.activeClickableMenu = new CoverageEditMenu(Helper);
    }

    /// <summary>
    /// Toggles the F3 overlay
    /// </summary>
    private void ToggleOverlay()
    {
      _showInfoOverlay = !_showInfoOverlay;
    }

    /// <summary>Run all sprinklers.</summary>
    private void RunSprinklers()
    {
      Logger.Info("Running sprinklers");
      // Start by Unwatering all tiles in every sprinkler's radius
      UnwaterAll();

      if (BetterSprinklersPlusConfig.Active.BalancedMode == (int)BetterSprinklersPlusConfig.BalancedModeOptions.Off)
      {
        Logger.Verbose("Balanced mode is off, just water");
        if (BetterSprinklersPlusConfig.Active.BalancedModeCostMessage)
        {
          Game1.addHUDMessage(new HUDMessage("Your sprinklers have run.", 2));
        }

        WaterAll();
        return;
      }

      Logger.Verbose("Balanced Mode is on, calculating cost");
      var cost = CalculateCost();
      var affordable = Game1.player.Money;
      if (cost > affordable && BetterSprinklersPlusConfig.Active.CannotAfford == (int)BetterSprinklersPlusConfig.CannotAffordOptions.DoNotWater)
      {
        Logger.Verbose(
          $"We can only afford {affordable}G, but watering would cost {cost}G");
        Logger.Verbose("Do not water is set, unwatering.");
        if (BetterSprinklersPlusConfig.Active.BalancedModeCostMessage || BetterSprinklersPlusConfig.Active.BalancedModeCannotAffordWarning)
        {
          Game1.addHUDMessage(new HUDMessage($"You could not afford to run your sprinklers today ({cost}G).", 3));
        }

        return;
      }

      // Otherwise, water everything and deduct cost
      WaterAll();
      DeductCost(cost);
      Logger.Verbose($"Sprinklers have run ({cost}G).");
      if (BetterSprinklersPlusConfig.Active.BalancedModeCostMessage && cost > 0)
      {
        Game1.addHUDMessage(new HUDMessage($"Your sprinklers have run ({cost}G).", 2));
      }
    }

    /// <summary>
    /// Water all tiles
    /// </summary>
    private void WaterAll()
    {
      foreach (var location in LocationHelper.GetAllBuildableLocations())
      {
        if (Game1.IsRainingHere(location) && !location.IsGreenhouse)
        {
          Logger.Verbose($"It's raining here, no need to water");
          continue;
        }

        foreach (var (tile, sprinkler) in location.AllSprinklers())
        {
          ActivateSprinkler(location, tile, sprinkler);
          // Save sprinkler activation to dict using location + type as key.
          var key = sprinkler.TileLocation.ToString() + "_" + sprinkler.QualifiedItemId;
          if (_sprinklerActivations.ContainsKey(key))
          {
            _sprinklerActivations[key] = Tuple.Create(sprinkler, DateTime.Now);
          }
          else
          {
            _sprinklerActivations.Add(key, Tuple.Create(sprinkler, DateTime.Now));
          }
        }
      }
    }

    private void ActivateSprinkler(GameLocation location, Vector2 tile, SObject sprinkler)
    {
      var type = sprinkler.ParentSheetIndex;
      BetterSprinklersPlusConfig.Active.SprinklerShapes.TryGetValue(type, out var grid);
      if (grid == null) return;

      sprinkler.ApplySprinklerAnimation();

      sprinkler.ForAllTiles(tile, t =>
      {
        if (t.IsCovered)
        {
          WaterTile(location, t.ToVector2());
        }
      });
    }

    /// <summary>
    /// Unwater all tiles
    /// </summary>
    private void UnwaterAll()
    {
      foreach (var location in LocationHelper.GetAllBuildableLocations())
      {
        if (Game1.IsRainingHere(location) && !location.IsGreenhouse)
        {
          Logger.Verbose($"It's raining here, not unwatering");
          continue;
        }

        foreach (var (tile, sprinkler) in location.AllSprinklers())
        {
          sprinkler.ForDefaultTiles(tile, t => UnwaterTile(location, t.ToVector2()));
        }
      }
    }

    /// <summary>
    /// Sets a tile to watered
    /// </summary>
    /// <param name="location">The location the tile is in</param>
    /// <param name="tile">The tile location</param>
    private void WaterTile(GameLocation location, Vector2 tile)
    {
      SetTileWateredValue(location, tile, HoeDirt.watered);
    }

    /// <summary>
    /// Sets a tile to dry
    /// </summary>
    /// <param name="location">The location the tile is in</param>
    /// <param name="tile">The tile location</param>
    private void UnwaterTile(GameLocation location, Vector2 tile)
    {
      SetTileWateredValue(location, tile, HoeDirt.dry);
    }

    /// <summary>
    /// Sets the dirt tile state
    /// </summary>
    /// <param name="location">The location the tile is in</param>
    /// <param name="tile">The tile location</param>
    /// <param name="value">The value to set it to</param>
    private void SetTileWateredValue(GameLocation location, Vector2 tile, int value)
    {
      if (!location.terrainFeatures.TryGetValue(tile, out var terrainFeature))
      {
        // Logger.Verbose($"could not get feature at: {tile.X}x{tile.Y}");
        return;
      }

      if (value != HoeDirt.dry && value != HoeDirt.watered)
      {
        Logger.Warn("Careful, setting the dirt state to something other than watered/dry is untested");
        return;
      }

      if (!terrainFeature.IsDirt()) return;

      var dirt = (HoeDirt)terrainFeature;
      dirt.state.Value = value;
    }

    private int CalculateCost()
    {
      Logger.Verbose($"CalculateCost()");
      var cost = 0f;
      foreach (var location in LocationHelper.GetAllBuildableLocations())
      {
        if (Game1.IsRainingHere(location) && !location.IsGreenhouse)
        {
          Logger.Verbose($"It's raining here, we are not running the sprinklers, so cost = 0");
          continue;
        }

        foreach (var (tile, sprinkler) in location.AllSprinklers())
        {
          var type = sprinkler.ParentSheetIndex;
          var hasPressureNozzle = sprinkler.HasPressureNozzle();
          var costForSprinkler = type.CalculateCostForSprinkler(hasPressureNozzle);
          Logger.Verbose($"Sprinkler at {tile.X}x{tile.Y}, type: {SprinklerHelper.SprinklerTypes[type]}, hasPressureNozzle: {hasPressureNozzle} cost: {costForSprinkler}G");
          cost += costForSprinkler;
        }
      }

      var rounded = (int)Math.Round(cost);
      Logger.Verbose($"CalculateCost(): {rounded}G");
      return rounded;
    }

    /// <summary>
    /// Deducts the cost from player money
    /// </summary>
    /// <param name="cost">The cost to deduct</param>
    private static void DeductCost(int cost)
    {
      if (Game1.player.Money - cost >= 0)
      {
        Game1.player.Money -= cost;
      }
      else
      {
        Game1.player.Money = 0;
      }
    }

    /// <summary>
    /// Highlight coverage for sprinklers and scarecrows held or under the mouse.
    /// </summary>
    private void RenderHighlight()
    {
      if (!ShouldRender())
      {
        return;
      }

      var x = (Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize;
      var y = (Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize;

      var mouseTile = new Vector2(x, y);

      HighlightCoverageForHeldObject();
      HighlightCoverageForObjectUnderCursor(mouseTile);
    }

    /// <summary>
    /// Tells us whether or not we should render highlights to the game world
    /// </summary>
    /// <returns>True if we should render</returns>
    private static bool ShouldRender()
    {
      return Context.IsWorldReady && Game1.activeClickableMenu == null && Game1.CurrentEvent == null;
    }

    /// <summary>
    /// Highlight the coverage for the held object
    /// </summary>
    private void HighlightCoverageForHeldObject()
    {
      if (!BetterSprinklersPlusConfig.Active.OverlayEnabledOnPlace) return;

      var heldObject = Game1.player.ActiveObject;
      if (heldObject == null) return;

      // accounts for controller mode
      var tileToHighlightFrom = PlacementHelper.GetPlacementPosition();
      if (tileToHighlightFrom == null) return;

      if (SprinklerHelper.IsSprinkler(heldObject))
      {
        RenderSprinklerHighlight(heldObject.ParentSheetIndex, (Vector2)tileToHighlightFrom);
        return;
      }

      if (ScarecrowHelper.IsScarecrow(heldObject))
      {
        RenderScarecrowHighlight((Vector2)tileToHighlightFrom);
      }
    }

    /// <summary>
    /// Highlight coverage for the object under the cursor
    /// </summary>
    /// <param name="mouseTile">The mouse tile</param>
    private void HighlightCoverageForObjectUnderCursor(Vector2 mouseTile)
    {
      if (!_showInfoOverlay) return;

      if (!Game1.currentLocation.objects.TryGetValue(mouseTile, out var objUnderMouse)) return;

      if (SprinklerHelper.IsSprinkler(objUnderMouse)){
        RenderSprinklerHighlight(objUnderMouse.ParentSheetIndex, mouseTile);
      }
      else if (ScarecrowHelper.IsScarecrow(objUnderMouse))
      {
        RenderScarecrowHighlight(mouseTile);
      }
    }

    /// <summary>Highlight coverage for a sprinkler.</summary>
    /// <param name="sprinklerId">The sprinkler ID.</param>
    /// <param name="tile">The sprinkler tile.</param>
    private void RenderSprinklerHighlight(int sprinklerId, Vector2 tile)
    {
      GridHelper.RenderHighlight(Helper, tile, BetterSprinklersPlusConfig.Active.SprinklerShapes[sprinklerId]);
    }

    /// <summary>Highlight coverage for a scarecrow.</summary>
    /// <param name="tile">The scarecrow tile.</param>
    private void RenderScarecrowHighlight(Vector2 tile)
    {
      var scarecrowGrid = ScarecrowHelper.GetScarecrowGrid();
      GridHelper.RenderHighlight(Helper, tile, scarecrowGrid);
    }
  }
}
