/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FarmAnimals;
using StardewValley.Pathfinding;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace BetterTruffles; 

internal partial class Mod {
  public class Object_draw_Patch {
    public static void Postfix(Object __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f) {
      if (!Config.Enabled) return;
      if (!Config.ShowBubbles) return;
      if (__instance.QualifiedItemId != "(O)430" && !__instance.HasContextTag("temisthem_bettertruffles")) return;

      var item = ItemRegistry.GetDataOrErrorItem(__instance.QualifiedItemId);
      var tileLocation = __instance.TileLocation;
      
      float base_sort = (tileLocation.Y + 1) * 64 / 10000f + tileLocation.X / 50000f;
      float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
      float movePercent = (100 - Config.SizePercent) / 100f;

      spriteBatch.Draw(Game1.mouseCursors, 
        Game1.GlobalToLocal(Game1.viewport, new Vector2(
          tileLocation.X * 64 - 8 + movePercent * 40 + Config.OffsetX, tileLocation.Y * 64 - 96 - 16 + yOffset + 
          movePercent * 96 + Config.OffsetY)),
        new Rectangle(141, 465, 20, 24), 
        Color.White * (Config.OpacityPercent / 100f), 
        0f, 
        Vector2.Zero, 
        4f * (Config.SizePercent / 100f), 
        SpriteEffects.None, 
        Config.RenderOnTop ? 0.99f : base_sort + 1E-06f);

      spriteBatch.Draw(
        item.GetTexture(),
        Game1.GlobalToLocal(Game1.viewport, new Vector2(
          tileLocation.X * 64 + 32 + Config.OffsetX, tileLocation.Y * 64 - 64 - 8 + yOffset + movePercent * 56 +
           Config.OffsetY)),
        item.GetSourceRect(),
        Color.White * (Config.OpacityPercent / 100f),
        0f, 
        new Vector2(8f, 8f), 
        4f * (Config.SizePercent / 100f),
        SpriteEffects.None, 
        Config.RenderOnTop ? 0.991f : base_sort + 1E-05f
      );
    }
  }  
  public class FarmAnimal_behaviors_Patch {
    public static bool Prefix(ref bool __result, FarmAnimal __instance, GameTime time, GameLocation location) {
      if (!Config.Enabled) return true;
      if (!Config.PigsDigInGrass) return true;
      
      if (!Game1.IsMasterGame) {
        __result = false;
        return false;
      }
      Building home = __instance.home;
      if (home == null) {
        __result = false;
        return false;
      }
      
      IReflectedField<float> nextFollowTargetScan = ModHelper.Reflection.GetField<float>(__instance, "_nextFollowTargetScan");
      IReflectedField<FarmAnimal> followTarget = ModHelper.Reflection.GetField<FarmAnimal>(__instance, "_followTarget");
      IReflectedField<Point?> followTargetPosition = ModHelper.Reflection.GetField<Point?>(__instance, "_followTargetPosition");
      
      if (__instance.isBaby() && __instance.CanFollowAdult()) {

        nextFollowTargetScan.SetValue(nextFollowTargetScan.GetValue() - (float) time.ElapsedGameTime.TotalSeconds);
        if ((double) nextFollowTargetScan.GetValue() < 0.0)
        {
          nextFollowTargetScan.SetValue(Utility.RandomFloat(1f, 3f));
          if (__instance.controller != null || !location.IsOutdoors)
          {
            followTarget.SetValue((FarmAnimal) null);
            followTargetPosition.SetValue(new Point?());
          }
          else if (followTarget.GetValue() == null)
          {
            if (location.IsOutdoors)
            {
              foreach (FarmAnimal animal in location.animals.Values)
              {
                if (!animal.isBaby() && animal.type.Value == __instance.type.Value && FarmAnimal.GetFollowRange(animal, 4).Contains(__instance.StandingPixel))
                {
                  followTarget.SetValue(animal);
                  __instance.GetNewFollowPosition();
                  __result = false;
                  return false;
                }
              }
            }
          }
          else
          {
            if (!FarmAnimal.GetFollowRange(followTarget.GetValue()).Contains(followTargetPosition.GetValue().Value))
              __instance.GetNewFollowPosition();
            __result = false;
            return false;
          }
        }
      }
      if ((bool) __instance.isEating.Value)
      {
        if (home != null && home.getRectForAnimalDoor().Intersects(__instance.GetBoundingBox()))
        {
          FarmAnimal.behaviorAfterFindingGrassPatch((Character) __instance, location);
          __instance.isEating.Value = false;
          __instance.Halt();
          __result = false;
          return false;
        }
        FarmAnimalData animalData = __instance.GetAnimalData();
        int startFrame = 16;
        if (!__instance.Sprite.textureUsesFlippedRightForLeft)
          startFrame += 4;
        bool? uniqueAnimationFrames = animalData?.UseDoubleUniqueAnimationFrames;
        if (uniqueAnimationFrames.HasValue && uniqueAnimationFrames.GetValueOrDefault())
          startFrame += 4;
        if (__instance.Sprite.Animate(time, startFrame, 4, 100f))
        {
          __instance.isEating.Value = false;
          __instance.Sprite.loop = true;
          __instance.Sprite.currentFrame = 0;
          __instance.faceDirection(2);
        }

        __result = true;
        return false;
      }

      if (__instance.controller != null) {
        __result = true;
        return false;
      }
      if (!__instance.isSwimming.Value && location.IsOutdoors && (int) __instance.fullness.Value < 195 && Game1.random.NextDouble() < 0.002 && FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
      {
        ++FarmAnimal.NumPathfindingThisTick;
        __instance.controller = new PathFindController((Character) __instance, location, new PathFindController.isAtEnd(FarmAnimal.grassEndPointFunction), -1, new PathFindController.endBehavior(FarmAnimal.behaviorAfterFindingGrassPatch), 200, Point.Zero);
        followTarget.SetValue((FarmAnimal) null);
        followTargetPosition.SetValue(new Point?());
      }
      if (Game1.timeOfDay >= 1700 && location.IsOutdoors && __instance.controller == null && Game1.random.NextDouble() < 0.002 && (bool) home.animalDoorOpen.Value)
      {
        if (!location.farmers.Any())
        {
          GameLocation indoors = home.GetIndoors();
          location.animals.Remove(__instance.myID.Value);
          indoors.animals.Add(__instance.myID.Value, __instance);
          __instance.setRandomPosition(indoors);
          __instance.faceDirection(Game1.random.Next(4));
          __instance.controller = (PathFindController) null;
          __result = true;
          return false;
        }
        if (FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
        {
          ++FarmAnimal.NumPathfindingThisTick;
          __instance.controller = new PathFindController((Character) __instance, location, new PathFindController.isAtEnd(PathFindController.isAtEndPoint), 0, (PathFindController.endBehavior) null, 200, new Point((int) home.tileX + home.animalDoor.X, (int) home.tileY + home.animalDoor.Y));
          followTarget.SetValue((FarmAnimal) null);
          followTargetPosition.SetValue(new Point?());
        }
      }
      if (location.IsOutdoors && !location.IsRainingHere() && !location.IsWinterHere() && __instance.currentProduce.Value != null && __instance.isAdult())
      {
        FarmAnimalHarvestType? harvestType = __instance.GetHarvestType();
        FarmAnimalHarvestType animalHarvestType = FarmAnimalHarvestType.DigUp;
        if (harvestType.GetValueOrDefault() == animalHarvestType & harvestType.HasValue && Game1.random.NextDouble() < 0.0002)
        {
          Object produce = ItemRegistry.Create<Object>(__instance.currentProduce.Value);
          Microsoft.Xna.Framework.Rectangle boundingBox = __instance.GetBoundingBox();
          for (int corner = 0; corner < 4; ++corner)
          {
            Vector2 cornersOfThisRectangle = Utility.getCornersOfThisRectangle(ref boundingBox, corner);
            Vector2 key = new Vector2((float) (int) ((double) cornersOfThisRectangle.X / 64.0), (float) (int) ((double) cornersOfThisRectangle.Y / 64.0));
            
            if (location.objects.ContainsKey(key)) {
              __result = false;
              return false;
            }

            location.terrainFeatures.TryGetValue(key, out var terrainFeature);
            if (terrainFeature != null && terrainFeature is not Grass) {
              __result = false;
              return false;
            }
          }
          if (Game1.player.currentLocation.Equals(location))
          {
            DelayedAction.playSoundAfterDelay("dirtyHit", 450);
            DelayedAction.playSoundAfterDelay("dirtyHit", 900);
            DelayedAction.playSoundAfterDelay("dirtyHit", 1350);
          }
          if (location.Equals(Game1.currentLocation))
          {
            switch (__instance.FacingDirection)
            {
              case 0:
                __instance.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame(9, 250),
                  new FarmerSprite.AnimationFrame(11, 250),
                  new FarmerSprite.AnimationFrame(9, 250),
                  new FarmerSprite.AnimationFrame(11, 250),
                  new FarmerSprite.AnimationFrame(9, 250),
                  new FarmerSprite.AnimationFrame(11, 250, false, false, (AnimatedSprite.endOfAnimationBehavior) (_ => __instance.DigUpProduce(location, produce)))
                });
                break;
              case 1:
                __instance.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame(5, 250),
                  new FarmerSprite.AnimationFrame(7, 250),
                  new FarmerSprite.AnimationFrame(5, 250),
                  new FarmerSprite.AnimationFrame(7, 250),
                  new FarmerSprite.AnimationFrame(5, 250),
                  new FarmerSprite.AnimationFrame(7, 250, false, false, (AnimatedSprite.endOfAnimationBehavior) (_ => __instance.DigUpProduce(location, produce)))
                });
                break;
              case 2:
                __instance.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame(1, 250),
                  new FarmerSprite.AnimationFrame(3, 250),
                  new FarmerSprite.AnimationFrame(1, 250),
                  new FarmerSprite.AnimationFrame(3, 250),
                  new FarmerSprite.AnimationFrame(1, 250),
                  new FarmerSprite.AnimationFrame(3, 250, false, false, (AnimatedSprite.endOfAnimationBehavior) (_ => __instance.DigUpProduce(location, produce)))
                });
                break;
              case 3:
                __instance.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame(5, 250, false, true),
                  new FarmerSprite.AnimationFrame(7, 250, false, true),
                  new FarmerSprite.AnimationFrame(5, 250, false, true),
                  new FarmerSprite.AnimationFrame(7, 250, false, true),
                  new FarmerSprite.AnimationFrame(5, 250, false, true),
                  new FarmerSprite.AnimationFrame(7, 250, false, true, (AnimatedSprite.endOfAnimationBehavior) (_ => __instance.DigUpProduce(location, produce)))
                });
                break;
            }
            __instance.Sprite.loop = false;
          }
          else
            __instance.DigUpProduce(location, produce);
        }
      }

      __result = false;
      return false;
    }
  }
   
}