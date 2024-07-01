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
using StardewValley.TerrainFeatures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using StardewModdingAPI;

using SObject = StardewValley.Object;

namespace Selph.StardewMods.MachineTerrainFramework;

// Contains utils for handling logic specific to water pot. The rest lives in CustomCrabPotUtils.
public static class WaterIndoorPotUtils {
  public static readonly string WaterPlanterItemId = $"{ModEntry.UniqueId}.WaterPlanter";
  public static readonly string WaterPlanterQualifiedItemId = $"(BC){WaterPlanterItemId}";
  public static readonly string WaterPotItemId = $"{ModEntry.UniqueId}.WaterPot";
  public static readonly string WaterPotQualifiedItemId = $"(BC){WaterPotItemId}";
  
  public static readonly string HoeDirtIsWaterModDataKey = $"{ModEntry.UniqueId}.IsWater";
  public static readonly string HoeDirtIsWaterPlanterModDataKey = $"{ModEntry.UniqueId}.IsWaterPlanter";

  public static readonly string CropIsWaterCustomFieldsKey = $"{ModEntry.UniqueId}.IsAquaticCrop";
  public static readonly string CropIsAmphibiousCustomFieldsKey = $"{ModEntry.UniqueId}.IsSemiAquaticCrop";

  public static void transformIndoorPotToItem(IndoorPot indoorPot, string itemId) {
    indoorPot.ItemId = itemId;
    if (Game1.bigCraftableData.TryGetValue(itemId, out var value))
    {
      indoorPot.name = value.Name ?? ItemRegistry.GetDataOrErrorItem($"(BC){itemId}").InternalName;
      indoorPot.Price = value.Price;
      indoorPot.Type = "Crafting";
      indoorPot.Category = -9;
      indoorPot.setOutdoors.Value = value.CanBePlacedOutdoors;
      indoorPot.setIndoors.Value = value.CanBePlacedIndoors;
      indoorPot.Fragility = value.Fragility;
      indoorPot.isLamp.Value = value.IsLamp;
    }
    indoorPot.ResetParentSheetIndex();
  }

  public static bool isWaterPlanter(SObject obj) {
    return 
      obj.QualifiedItemId == WaterIndoorPotUtils.WaterPlanterQualifiedItemId ||
      obj.QualifiedItemId == WaterIndoorPotUtils.WaterPotQualifiedItemId;
  }

  public static void draw(IndoorPot obj, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    GameLocation location = obj.Location;
    if (location == null)
    {
      return;
    }
    CrabPotData crabPotData = CustomCrabPotUtils.getCrabPotData(obj);
    float yBob = (float)(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 500.0 + (double)(x * 64)) * 8.0 + 8.0);
    float yBobCrops = (float)(Math.Sin((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + 500) / 500.0 + (double)(x * 64)) * 8.0 + 8.0);
    if (yBob <= 0.001f)
    {
      location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 150f, 8, 0, crabPotData.directionOffset + new Vector2(x * 64 + 4, y * 64 + 32), flicker: false, Game1.random.NextBool(), 0.001f, 0.01f, Color.White, 0.75f, 0.003f, 0f, 0f));
    }

    Vector2 vector = obj.getScale();
    vector *= 4f;
    Vector2 vector2 = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64 + (int)yBob));
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(obj.QualifiedItemId);
    Texture2D texture2D = dataOrErrorItem.GetTexture();
    int spriteIndex = dataOrErrorItem.SpriteIndex;

    spriteBatch.Draw(
        texture2D,
        Game1.GlobalToLocal(Game1.viewport,
          crabPotData.directionOffset + new Vector2(x * 64, y * 64 + (int)yBob - 64)) + crabPotData.shake,
        SObject.getSourceRectForBigCraftable(texture2D, spriteIndex),
        Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64) + crabPotData.directionOffset.Y + (float)(x % 4)) / 10000f);

    // Old draw code
    //Microsoft.Xna.Framework.Rectangle destinationRectangle =
    //  new Microsoft.Xna.Framework.Rectangle(
    //      (int)(vector2.X - vector.X / 2f) + ((obj.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0),
    //      (int)(vector2.Y - vector.Y / 2f) + ((obj.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0),
    //      (int)(64f + vector.X),
    //      (int)(128f + vector.Y / 2f));
    //spriteBatch.Draw(
    //    dataOrErrorItem.GetTexture(),
    //    destinationRectangle,
    //    dataOrErrorItem.GetSourceRect(obj.showNextIndex.Value ? 1 : 0),
    //    Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f);
    if (obj.hoeDirt.Value.HasFertilizer())
    {
      Microsoft.Xna.Framework.Rectangle fertilizerSourceRect = obj.hoeDirt.Value.GetFertilizerSourceRect();
      fertilizerSourceRect.Width = 13;
      fertilizerSourceRect.Height = 13;
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, crabPotData.directionOffset + new Vector2(obj.tileLocation.X * 64f + 4f, obj.tileLocation.Y * 64f + 4f + (int)yBobCrops)), fertilizerSourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (obj.tileLocation.Y + 0.65f) * 64f / 10000f + (float)x * 1E-05f);
    }
    obj.hoeDirt.Value.crop?.drawWithOffset(spriteBatch, obj.tileLocation.Value, /*(obj.hoeDirt.Value.isWatered() && (int)obj.hoeDirt.Value.crop.currentPhase.Value == 0 && !obj.hoeDirt.Value.crop.raisedSeeds.Value) ? (new Color(180, 100, 200) * 1f) :*/ Color.White, obj.hoeDirt.Value.getShakeRotation(), crabPotData.directionOffset + new Vector2(32f, 24f + (int)yBobCrops));
    obj.heldObject.Value?.draw(spriteBatch, x * 64, y * 64 - 48, (obj.tileLocation.Y + 0.66f) * 64f / 10000f + (float)x * 1E-05f, 1f);
    obj.bush.Value?.draw(spriteBatch, crabPotData.directionOffset.Y + yBobCrops);
  }

  public static void canPlant(HoeDirt hoeDirt, string itemId, ref bool result) {
    itemId = Crop.ResolveSeedId(itemId, hoeDirt.Location);
    if (Crop.TryGetData(itemId, out var data)) {
      foreach (var what in hoeDirt.modData.Pairs) {
      }
      bool isWater = hoeDirt.modData.ContainsKey(HoeDirtIsWaterModDataKey);
      if (isWater &&
          (!data.CustomFields?.ContainsKey(CropIsAmphibiousCustomFieldsKey) ?? true) &&
          (!data.CustomFields?.ContainsKey(CropIsWaterCustomFieldsKey) ?? true)) {
        result = false;
      }
      if (!isWater &&
          (data.CustomFields?.ContainsKey(CropIsWaterCustomFieldsKey) ?? false)) {
        result = false;
      }
    }
  }
}
