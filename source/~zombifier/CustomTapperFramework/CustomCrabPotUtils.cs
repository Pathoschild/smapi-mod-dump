/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using StardewModdingAPI;

using SObject = StardewValley.Object;

namespace Selph.StardewMods.MachineTerrainFramework;

class CrabPotData {
  public Vector2 directionOffset = Vector2.Zero;
  public int ignoreRemovalTimer = 0;
  public Vector2 shake = Vector2.Zero;
}

// Contains common logic for drawing floating water buildings, including water planters.
public static class CustomCrabPotUtils
{
  private static readonly Dictionary<string, Dictionary<Vector2, CrabPotData>> crabPotDataMap = [];

  private static CrabPotData getCrabPotData(GameLocation location, Vector2 tileLocation) {
    if (!crabPotDataMap.ContainsKey(location.Name)) {
      crabPotDataMap[location.Name] = new Dictionary<Vector2, CrabPotData>();
    }
    var locationDataMap = crabPotDataMap[location.Name];
    if (!locationDataMap.ContainsKey(tileLocation)) {
      locationDataMap[tileLocation] = new CrabPotData();
    }
    return locationDataMap[tileLocation];
  }

  internal static CrabPotData getCrabPotData(SObject obj) {
    return getCrabPotData(obj.Location, obj.TileLocation);
  }

  public static List<Vector2> getOverlayTiles(GameLocation location, Vector2 tileLocation)
  {
    List<Vector2> list = new List<Vector2>();
    var directionOffset = getCrabPotData(location, tileLocation).directionOffset;
    if (location != null)
    {
      if (directionOffset.Y < 0f)
      {
        addOverlayTilesIfNecessary(location, (int)tileLocation.X, (int)tileLocation.Y, list);
      }
      addOverlayTilesIfNecessary(location, (int)tileLocation.X, (int)tileLocation.Y + 1, list);
      if (directionOffset.X < 0f)
      {
        addOverlayTilesIfNecessary(location, (int)tileLocation.X - 1, (int)tileLocation.Y + 1, list);
      }
      if (directionOffset.X > 0f)
      {
        addOverlayTilesIfNecessary(location, (int)tileLocation.X + 1, (int)tileLocation.Y + 1, list);
      }
    }
    return list;
  }

  static void addOverlayTilesIfNecessary(GameLocation location, int tile_x, int tile_y, List<Vector2> tiles)
  {
    if (location != null && location == Game1.currentLocation && location.getTileIndexAt(tile_x, tile_y, "Buildings") >= 0 && !location.isWaterTile(tile_x, tile_y + 1))
    {
      tiles.Add(new Vector2(tile_x, tile_y));
    }
  }

  /// <summary>Add any tiles that might overlap with this crab pot incorrectly to the <see cref="F:StardewValley.Game1.crabPotOverlayTiles" /> dictionary.</summary>
  public static void addOverlayTiles(GameLocation location, Vector2 tileLocation)
  {
    if (location == null || location != Game1.currentLocation)
    {
      return;
    }
    foreach (Vector2 overlayTile in getOverlayTiles(location, tileLocation))
    {
      if (!Game1.crabPotOverlayTiles.TryGetValue(overlayTile, out var value))
      {
        value = (Game1.crabPotOverlayTiles[overlayTile] = 0);
      }
      Game1.crabPotOverlayTiles[overlayTile] = value + 1;
    }
  }

  /// <summary>Remove any tiles that might overlap with this crab pot incorrectly from the <see cref="F:StardewValley.Game1.crabPotOverlayTiles" /> dictionary.</summary>
  public static void removeOverlayTiles(GameLocation location, Vector2 tileLocation)
  {
    if (location == null || location != Game1.currentLocation)
    {
      return;
    }
    foreach (Vector2 overlayTile in getOverlayTiles(location, tileLocation))
    {
      if (Game1.crabPotOverlayTiles.TryGetValue(overlayTile, out var value))
      {
        value--;
        if (value <= 0)
        {
          Game1.crabPotOverlayTiles.Remove(overlayTile);
        }
        else
        {
          Game1.crabPotOverlayTiles[overlayTile] = value;
        }
      }
    }
  }

  /// <inheritdoc />
  public static void actionOnPlayerEntry(GameLocation location, Vector2 tileLocation)
  {
    updateOffset(location, tileLocation);
    addOverlayTiles(location, tileLocation);
  }

  public static bool placementAction(SObject obj, GameLocation location, int x, int y, Farmer who = null)
  {
    Vector2 vector = new Vector2(x / 64, y / 64);
    if (who != null)
    {
      obj.owner.Value = who.UniqueMultiplayerID;
    }
    if (!CrabPot.IsValidCrabPotLocationTile(location, (int)vector.X, (int)vector.Y))
    {
      return false;
    }
    obj.CanBeGrabbed = false;
    obj.Type = "interactive";
    obj.Location = location;
    obj.TileLocation = vector;
    location.objects.Add(obj.TileLocation, obj);
    location.playSound("waterSlosh");
    DelayedAction.playSoundAfterDelay("slosh", 150);
    updateOffset(obj.Location, obj.TileLocation);
    addOverlayTiles(obj.Location, obj.TileLocation);

    CrabPotData crabPotData = getCrabPotData(obj);
    crabPotData.ignoreRemovalTimer = 500;

    return true;
  }

  public static void updateOffset(GameLocation location, Vector2 tileLocation)
  {
    Vector2 zero = Vector2.Zero;
    if (checkLocation(location, tileLocation.X - 1f, tileLocation.Y))
    {
      zero += new Vector2(32f, 0f);
    }
    if (checkLocation(location, tileLocation.X + 1f, tileLocation.Y))
    {
      zero += new Vector2(-32f, 0f);
    }
    if (zero.X != 0f && checkLocation(location, tileLocation.X + (float)Math.Sign(zero.X), tileLocation.Y + 1f))
    {
      zero += new Vector2(0f, -42f);
    }
    if (checkLocation(location, tileLocation.X, tileLocation.Y - 1f))
    {
      zero += new Vector2(0f, 32f);
    }
    if (checkLocation(location, tileLocation.X, tileLocation.Y + 1f))
    {
      zero += new Vector2(0f, -42f);
    }
    CrabPotData crabPotData = getCrabPotData(location, tileLocation);
    crabPotData.directionOffset = zero;
  }

  static bool checkLocation(GameLocation location, float tile_x, float tile_y)
  {
    if (!location.isWaterTile((int)tile_x, (int)tile_y) || location.doesTileHaveProperty((int)tile_x, (int)tile_y, "Passable", "Buildings") != null)
    {
      return true;
    }
    return false;
  }

  /// <inheritdoc />
  //protected override Item GetOneNew()
  //{
  //  return new SObject(base.ItemId, 1);
  //}

  /// <inheritdoc />
  //public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who, bool returnFalseIfItemConsumed = false)
  //{
  //  GameLocation location = this.Location;
  //  if (location == null)
  //  {
  //    return false;
  //  }
  //  if (!(dropInItem is Object @object))
  //  {
  //    return false;
  //  }
  //  Farmer farmer = Game1.getFarmer(base.owner.Value);
  //  if (@object.Category == -21 && this.bait.Value == null && (farmer == null || !farmer.professions.Contains(11)))
  //  {
  //    if (!probe)
  //    {
  //      if (who != null)
  //      {
  //        base.owner.Value = who.UniqueMultiplayerID;
  //      }
  //      this.bait.Value = @object.getOne() as Object;
  //      location.playSound("Ship");
  //      this.lidFlapping = true;
  //      this.lidFlapTimer = 60f;
  //    }
  //    return true;
  //  }
  //  return false;
  //}

  /// <inheritdoc />
  /// Returns true if the original function should be skipped
  public static bool checkForAction(SObject obj, Farmer who, bool justCheckingForActivity = false)
  {
    GameLocation location = obj.Location;
    Vector2 tileLocation = obj.TileLocation;
    CrabPotData crabPotData = getCrabPotData(obj);
  //  if (location == null)
  //  {
  //    return false;
  //  }
  //  if (this.tileIndexToShow == 714)
  //  {
  //    if (justCheckingForActivity)
  //    {
  //      return true;
  //    }
  //    int numberCaught = 1;
  //    if (Utility.CreateDaySaveRandom(Game1.uniqueIDForThisGame, Game1.stats.DaysPlayed * 77, base.TileLocation.X * 777f + base.TileLocation.Y).NextDouble() < 0.25 && Game1.player.stats.Get("Book_Crabbing") != 0)
  //    {
  //      numberCaught = 2;
  //    }
  //    Object value = base.heldObject.Value;
  //    if (value != null)
  //    {
  //      value.Stack = numberCaught;
  //      base.heldObject.Value = null;
  //      if (who.IsLocalPlayer && !who.addItemToInventoryBool(value))
  //      {
  //        base.heldObject.Value = value;
  //        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
  //        return false;
  //      }
  //      if (DataLoader.Fish(Game1.content).TryGetValue(value.ItemId, out var value2))
  //      {
  //        string[] array = value2.Split('/');
  //        int minValue = ((array.Length <= 5) ? 1 : Convert.ToInt32(array[5]));
  //        int num = ((array.Length > 5) ? Convert.ToInt32(array[6]) : 10);
  //        who.caughtFish(value.QualifiedItemId, Game1.random.Next(minValue, num + 1), from_fish_pond: false, numberCaught);
  //      }
  //      who.gainExperience(1, 5);
  //    }
  //    base.readyForHarvest.Value = false;
  //    this.tileIndexToShow = 710;
  //    this.lidFlapping = true;
  //    this.lidFlapTimer = 60f;
  //    this.bait.Value = null;
  //    who.animateOnce(279 + who.FacingDirection);
  //    location.playSound("fishingRodBend");
  //    DelayedAction.playSoundAfterDelay("coin", 500);
  //    crabPotData.shake = Vector2.Zero;
  //    crabPotData.shakeTimer = 0f;
  //    this.ignoreRemovalTimer = 750;
  //    return true;
  //  }
    bool machineNotWorking = (obj is not IndoorPot &&
        (obj.heldObject.Value == null || !obj.readyForHarvest.Value));
    bool waterPlanterHasNoCrops = (obj is IndoorPot waterPlanter &&
           waterPlanter.hoeDirt.Value.crop == null &&
           waterPlanter.bush.Value == null);
    if ((machineNotWorking || waterPlanterHasNoCrops) &&
        crabPotData.ignoreRemovalTimer <= 0)
    {
      if (justCheckingForActivity)
      {
        return true;
      }
      if (Game1.didPlayerJustClickAtAll(ignoreNonMouseHeldInput: true))
      {
        if (Game1.player.addItemToInventoryBool(obj.getOne()))
        {
          if (who.isMoving())
          {
            Game1.haltAfterCheck = false;
          }
          Game1.playSound("coin");
          location.objects.Remove(tileLocation);
          removeOverlayTiles(location, tileLocation);
          return true;
        }
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
      }
    }
    return false;
  }

  public static void performRemoveAction(GameLocation location, Vector2 tileLocation)
  {
    removeOverlayTiles(location, tileLocation);
  }

  //public override void DayUpdate()
  //{
  //  GameLocation location = this.Location;
  //  bool flag = Game1.getFarmer(base.owner.Value) != null && Game1.getFarmer(base.owner.Value).professions.Contains(11);
  //  bool flag2 = Game1.getFarmer(base.owner.Value) != null && Game1.getFarmer(base.owner.Value).professions.Contains(10);
  //  if (base.owner.Value == 0L && Game1.player.professions.Contains(11))
  //  {
  //    flag2 = true;
  //  }
  //  if (!(this.bait.Value != null || flag) || base.heldObject.Value != null)
  //  {
  //    return;
  //  }
  //  this.tileIndexToShow = 714;
  //  base.readyForHarvest.Value = true;
  //  Random random = Utility.CreateDaySaveRandom(base.TileLocation.X * 1000f, base.TileLocation.Y * 255f, this.directionOffset.X * 1000f + this.directionOffset.Y);
  //  Dictionary<string, string> dictionary = DataLoader.Fish(Game1.content);
  //  List<string> list = new List<string>();
  //  if (!location.TryGetFishAreaForTile(base.TileLocation.Value, out var _, out var data))
  //  {
  //    data = null;
  //  }
  //  double num = (flag2 ? 0.0 : (((double?)data?.CrabPotJunkChance) ?? 0.2));
  //  int initialStack = 1;
  //  int num2 = 0;
  //  string text = null;
  //  if (this.bait.Value != null && this.bait.Value.QualifiedItemId == "(O)DeluxeBait")
  //  {
  //    num2 = 1;
  //    num /= 2.0;
  //  }
  //  else if (this.bait.Value != null && this.bait.Value.QualifiedItemId == "(O)774")
  //  {
  //    num /= 2.0;
  //    if (random.NextBool(0.25))
  //    {
  //      initialStack = 2;
  //    }
  //  }
  //  else if (this.bait.Value != null && this.bait.Value.Name.Contains("Bait") && this.bait.Value.preservedParentSheetIndex != null && this.bait.Value.preserve.Value.HasValue)
  //  {
  //    text = this.bait.Value.preservedParentSheetIndex.Value;
  //    num /= 2.0;
  //  }
  //  if (!random.NextBool(num))
  //  {
  //    IList<string> crabPotFishForTile = location.GetCrabPotFishForTile(base.TileLocation.Value);
  //    foreach (KeyValuePair<string, string> item in dictionary)
  //    {
  //      if (!item.Value.Contains("trap"))
  //      {
  //        continue;
  //      }
  //      string[] array = item.Value.Split('/');
  //      string[] array2 = ArgUtility.SplitBySpace(array[4]);
  //      bool flag3 = false;
  //      string[] array3 = array2;
  //      foreach (string text2 in array3)
  //      {
  //        foreach (string item2 in crabPotFishForTile)
  //        {
  //          if (text2 == item2)
  //          {
  //            flag3 = true;
  //            break;
  //          }
  //        }
  //      }
  //      if (!flag3)
  //      {
  //        continue;
  //      }
  //      if (flag2)
  //      {
  //        list.Add(item.Key);
  //        continue;
  //      }
  //      double num3 = Convert.ToDouble(array[2]);
  //      if (text != null && text == item.Key)
  //      {
  //        num3 *= (double)((num3 < 0.1) ? 4 : ((num3 < 0.2) ? 3 : 2));
  //      }
  //      if (!(random.NextDouble() < num3))
  //      {
  //        continue;
  //      }
  //      base.heldObject.Value = new Object(item.Key, initialStack, isRecipe: false, -1, num2);
  //      break;
  //    }
  //  }
  //  if (base.heldObject.Value == null)
  //  {
  //    if (flag2 && list.Count > 0)
  //    {
  //      base.heldObject.Value = ItemRegistry.Create<Object>("(O)" + random.ChooseFrom(list));
  //    }
  //    else
  //    {
  //      base.heldObject.Value = ItemRegistry.Create<Object>("(O)" + random.Next(168, 173));
  //    }
  //  }
  //}

  public static void updateWhenCurrentLocation(SObject obj, GameTime time)
  {
  //  if (this.lidFlapping)
  //  {
  //    this.lidFlapTimer -= time.ElapsedGameTime.Milliseconds;
  //    if (this.lidFlapTimer <= 0f)
  //    {
  //      this.tileIndexToShow += ((!this.lidClosing) ? 1 : (-1));
  //      if (this.tileIndexToShow >= 713 && !this.lidClosing)
  //      {
  //        this.lidClosing = true;
  //        this.tileIndexToShow--;
  //      }
  //      else if (this.tileIndexToShow <= 709 && this.lidClosing)
  //      {
  //        this.lidClosing = false;
  //        this.tileIndexToShow++;
  //        this.lidFlapping = false;
  //        if (this.bait.Value != null)
  //        {
  //          this.tileIndexToShow = 713;
  //        }
  //      }
  //      this.lidFlapTimer = 60f;
  //    }
  //  }
  //  if ((bool)base.readyForHarvest && base.heldObject.Value != null)
  //  {
  //    crabPotData.shakeTimer -= time.ElapsedGameTime.Milliseconds;
  //    if (crabPotData.shakeTimer < 0f)
  //    {
  //      crabPotData.shakeTimer = Game1.random.Next(2800, 3200);
  //    }
  //  }
  //  if (crabPotData.shakeTimer > 2000f)
  //  {
  //    crabPotData.shake.X = Game1.random.Next(-1, 2);
  //  }
  //  else
  //  {
  //    crabPotData.shake.X = 0f;
  //  }
    CrabPotData crabPotData = getCrabPotData(obj);
    if (crabPotData.ignoreRemovalTimer > 0)
    {
      crabPotData.ignoreRemovalTimer -= (int)time.ElapsedGameTime.TotalMilliseconds;
    }
  }

  public static void draw(SObject obj, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    GameLocation location = obj.Location;
    if (location == null)
    {
      return;
    }
    CrabPotData crabPotData = getCrabPotData(obj);
    float yBob = (float)(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 500.0 + (double)(x * 64)) * 8.0 + 8.0);
    if (yBob <= 0.001f)
    {
      location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 150f, 8, 0, crabPotData.directionOffset + new Vector2(x * 64 + 4, y * 64 + 32), flicker: false, Game1.random.NextBool(), 0.001f, 0.01f, Color.White, 0.75f, 0.003f, 0f, 0f));
    }

    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(obj.QualifiedItemId);
    Texture2D texture2D = dataOrErrorItem.GetTexture();
    //int spriteIndex = dataOrErrorItem.SpriteIndex;
    int offset = 0;
    if (obj.showNextIndex.Value) {
      offset = 1;
    }
    int machineAnimationFrame = ModEntry.Helper.Reflection.GetField<int>(obj, "_machineAnimationFrame").GetValue();
    object machineAnimation = ModEntry.Helper.Reflection.GetField<object>(obj, "_machineAnimation").GetValue();
    if (machineAnimationFrame >= 0 && machineAnimation != null)
    {
      offset = machineAnimationFrame;
    }
    int spriteIndex = obj.ParentSheetIndex;

    spriteBatch.Draw(
        texture2D,
        Game1.GlobalToLocal(Game1.viewport,
          crabPotData.directionOffset + new Vector2(x * 64, y * 64 + (int)yBob - 64)) + crabPotData.shake,
        SObject.getSourceRectForBigCraftable(texture2D, spriteIndex + offset),
        Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64) + crabPotData.directionOffset.Y + (float)(x % 4)) / 10000f);

    if (location.waterTiles != null && x < location.waterTiles.waterTiles.GetLength(0) && y < location.waterTiles.waterTiles.GetLength(1) && location.waterTiles.waterTiles[x, y].isWater)
    {
      if (location.waterTiles.waterTiles[x, y].isVisible)
      {
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, crabPotData.directionOffset + new Vector2(x * 64 + 4, y * 64 + 48)) + crabPotData.shake, new Microsoft.Xna.Framework.Rectangle(location.waterAnimationIndex * 64, 2112 + (((x + y) % 2 != 0) ? ((!location.waterTileFlip) ? 128 : 0) : (location.waterTileFlip ? 128 : 0)), 56, 16 + (int)yBob), location.waterColor.Value, 0f, Vector2.Zero, 1f, SpriteEffects.None, ((float)(y * 64) + crabPotData.directionOffset.Y + (float)(x % 4)) / 9999f);
      }
      else
      {
        Color a = new Color(135, 135, 135, 215);
        a = Utility.MultiplyColor(a, location.waterColor.Value);
        spriteBatch.Draw(Game1.staminaRect, Game1.GlobalToLocal(Game1.viewport, crabPotData.directionOffset + new Vector2(x * 64 + 4, y * 64 + 48)) + crabPotData.shake, null, a, 0f, Vector2.Zero, new Vector2(56f, 16 + (int)yBob), SpriteEffects.None, ((float)(y * 64) + crabPotData.directionOffset.Y + (float)(x % 4)) / 9999f);
      }
    }
    if ((bool)obj.readyForHarvest.Value && obj.heldObject.Value != null)
    {
      float num = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, crabPotData.directionOffset + new Vector2(x * 64 - 8, (float)(y * 64 - 96 - 16) + num)), new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((y + 1) * 64) / 10000f + 1E-06f + obj.TileLocation.X / 10000f);
      ParsedItemData heldObjectData = ItemRegistry.GetDataOrErrorItem(obj.heldObject.Value.QualifiedItemId);
      spriteBatch.Draw(heldObjectData.GetTexture(), Game1.GlobalToLocal(Game1.viewport, crabPotData.directionOffset + new Vector2(x * 64 + 32, (float)(y * 64 - 64 - 8) + num)), heldObjectData.GetSourceRect(), Color.White * 0.75f, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float)((y + 1) * 64) / 10000f + 1E-05f + obj.TileLocation.X / 10000f);
    }
  }
  
  public static void resetRemovalTimer(SObject obj) {
    CrabPotData crabPotData = getCrabPotData(obj);
    crabPotData.ignoreRemovalTimer = 750;
  }

  public static float getXOffset(GameLocation location, Vector2 tileLocation) {
    CrabPotData crabPotData = getCrabPotData(location, tileLocation);
    return crabPotData.directionOffset.X;
  }
}
