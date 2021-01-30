/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using Harmony; // el diavolo
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Linq;
using System.Collections.Generic;
using xTile;
using xTile.Tiles;

namespace LoveOfCooking
{
	public static class HarmonyPatches
	{
		public static void Patch()
		{
			var id = ModEntry.Instance.Helper.ModRegistry.ModID;
			var harmony = HarmonyInstance.Create(id);
			try
			{
				if (harmony.HasAnyPatches(id))
					harmony.UnpatchAll(id);
			}
			catch (Exception e)
			{
				Log.D($"Error occurred while unpatching methods, may not be fatal.\n{e}",
					ModEntry.Instance.Config.DebugMode);
			}

			harmony.Patch(
				original: AccessTools.Method(typeof(Bush), nameof(Bush.inBloom)),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Bush_inBloom_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(Bush), "getEffectiveSize"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Bush_getEffectiveSize_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(Bush), nameof(Bush.isDestroyable)),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Bush_isDestroyable_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(Bush), "shake"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Bush_shake_Prefix)));

			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "getAreaNameFromNumber"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_AreaNameFromNumber_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "getAreaNumberFromName"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_AreaNumberFromName_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "getAreaNumberFromLocation"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_AreaNumberFromLocation_Prefix)));
			
			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "getAreaEnglishDisplayNameFromNumber"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_AreaEnglishDisplayNameFromNumber_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "getAreaDisplayNameFromNumber"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_AreaDisplayNameFromNumber_Prefix)));
			

			// TODO: TEST: Big problem in JunimoNoteMenu.setUpMenu():
			// >> if (!Game1.player.hasOrWillReceiveMail("hasSeenAbandonedJunimoNote") && whichArea == 6)

			// TODO: POLISH: Move as many routines out of harmony as possible

			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "loadArea"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_LoadArea_Prefix)));
			
			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "shouldNoteAppearInArea"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_ShouldNoteAppearInArea_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "isJunimoNoteAtArea"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_IsJunimoNoteAtArea_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "addJunimoNote"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_AddJunimoNote_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "UpdateWhenCurrentLocation"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_UpdateWhenCurrentLocation_Prefix)));

			// these ones are probably alright

			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "setViewportToNextJunimoNoteTarget"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_SetViewportToNextJunimoNoteTarget_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "junimoGoodbyeDance"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_JunimoGoodbyeDance_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "startGoodbyeDance"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_StartGoodbyeDance_Prefix)));
			harmony.Patch(
				original: AccessTools.Method(typeof(CommunityCenter), "getMessageForAreaCompletion"),
				prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CC_MessageForAreaCompletion_Prefix)));

			// TODO: TEST: Community centre cooking bundle completion and all bundle completion
		}

		/// <summary>
		/// Basic implementation of new CommunityCenter area.
		/// </summary>
		public static bool CC_AreaNameFromNumber_Prefix(ref string __result, int areaNumber)
		{
			try
			{
				if (areaNumber != ModEntry.CommunityCentreAreaNumber || ModEntry.IsCommunityCentreComplete() || ModEntry.IsAbandonedJojaMartBundleAvailable())
					return true;
				__result = ModEntry.CommunityCentreAreaName;
				return false;
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_AreaNameFromNumber_Prefix)}:\n{e}");
			}

			return true;
		}

		/// <summary>
		/// Basic implementation of new CommunityCenter area.
		/// </summary>
		public static bool CC_AreaNumberFromName_Prefix(ref int __result, string name)
		{
			try
			{
				if (name != ModEntry.CommunityCentreAreaName)
					return true;
				__result = ModEntry.CommunityCentreAreaNumber;
				return false;
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_AreaNumberFromName_Prefix)}:\n{e}");
			}

			return true;
		}

		/// <summary>
		/// Basic implementation of new CommunityCenter area.
		/// </summary>
		public static bool CC_AreaNumberFromLocation_Prefix(ref int __result, Vector2 tileLocation)
		{
			try
			{
				Log.T($"CC_AreaNumberFromLocation_Prefix(tileLocation={tileLocation.ToString()})");
				if (!new Rectangle(0, 0, 11, 11).Contains(Utility.Vector2ToPoint(tileLocation)) || ModEntry.IsCommunityCentreComplete() || ModEntry.IsAbandonedJojaMartBundleAvailable())
					return true;
				__result = ModEntry.CommunityCentreAreaNumber;
				return false;
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_AreaNumberFromLocation_Prefix)}:\n{e}");
			}

			return true;
		}
		
		/// <summary>
		/// Basic implementation of new CommunityCenter area.
		/// </summary>
		public static bool CC_AreaEnglishDisplayNameFromNumber_Prefix(ref string __result, int areaNumber)
		{
			try
			{
				if (areaNumber != ModEntry.CommunityCentreAreaNumber || ModEntry.IsCommunityCentreComplete() || ModEntry.IsAbandonedJojaMartBundleAvailable())
					return true;
				__result = ModEntry.Instance.Helper.Translation.Get("world.community_centre.kitchen");
				return false;
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_AreaEnglishDisplayNameFromNumber_Prefix)}:\n{e}");
			}

			return true;
		}

		/// <summary>
		/// Basic implementation of new CommunityCenter area.
		/// </summary>
		public static bool CC_AreaDisplayNameFromNumber_Prefix(ref string __result, int areaNumber)
		{
			try
			{
				if (areaNumber != ModEntry.CommunityCentreAreaNumber || ModEntry.IsCommunityCentreComplete() || ModEntry.IsAbandonedJojaMartBundleAvailable())
					return true;
				__result = ModEntry.Instance.Helper.Translation.Get("world.community_centre.kitchen");
				return false;
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_AreaDisplayNameFromNumber_Prefix)}:\n{e}");
			}

			return true;
		}

		/// <summary>
		/// GetAreaBounds() throws FatalEngineExecutionError when patched.
		/// Mimics LoadArea() using a static areaToRefurbish value in place of GetAreaBounds().
		/// </summary>
		public static bool CC_LoadArea_Prefix(CommunityCenter __instance, int area, bool showEffects)
		{
			try
			{
				Log.T($"CC_LoadArea_Prefix(area={area})");
				if (area != ModEntry.CommunityCentreAreaNumber || ModEntry.IsCommunityCentreComplete() || ModEntry.IsAbandonedJojaMartBundleAvailable())
					return true;

				var areaToRefurbish = area != ModEntry.CommunityCentreAreaNumber 
					? ModEntry.Instance.Helper.Reflection.GetMethod(__instance, "getAreaBounds").Invoke<Rectangle>(area)
					: ModEntry.CommunityCentreArea;
				var refurbishedMap = Game1.game1.xTileContent.Load<Map>("Maps\\CommunityCenter_Refurbished");

				//PyTK.Extensions.PyMaps.mergeInto(__instance.Map, refurbishedMap, Vector2.Zero, ModEntry.CommunityCentreArea);
				//__instance.addLightGlows();
				//return false;

				var adjustMapLightPropertiesForLamp = ModEntry.Instance.Helper.Reflection.GetMethod(
					__instance, "adjustMapLightPropertiesForLamp");
				
				for (var x = areaToRefurbish.X; x < areaToRefurbish.Right; x++)
				{
					for (var y = areaToRefurbish.Y; y < areaToRefurbish.Bottom; y++)
					{
						if (refurbishedMap.GetLayer("Back").Tiles[x, y] != null)
						{
							__instance.map.GetLayer("Back").Tiles[x, y].TileIndex
								= refurbishedMap.GetLayer("Back").Tiles[x, y].TileIndex;
						}
						if (refurbishedMap.GetLayer("Buildings").Tiles[x, y] != null)
						{
							__instance.map.GetLayer("Buildings").Tiles[x, y] = new StaticTile(
								__instance.map.GetLayer("Buildings"), __instance.map.TileSheets[0],
								BlendMode.Alpha, refurbishedMap.GetLayer("Buildings").Tiles[x, y].TileIndex);
							adjustMapLightPropertiesForLamp.Invoke(
								refurbishedMap.GetLayer("Buildings").Tiles[x, y].TileIndex, x, y, "Buildings");
							if (Game1.player.getTileX() == x && Game1.player.getTileY() == y)
							{
								Game1.player.Position = new Vector2(2080f, 576f);
							}
						}
						else
						{
							__instance.map.GetLayer("Buildings").Tiles[x, y] = null;
						}
						if (refurbishedMap.GetLayer("Front").Tiles[x, y] != null)
						{
							__instance.map.GetLayer("Front").Tiles[x, y] = new StaticTile(
								__instance.map.GetLayer("Front"), __instance.map.TileSheets[0],
								BlendMode.Alpha, refurbishedMap.GetLayer("Front").Tiles[x, y].TileIndex);
							adjustMapLightPropertiesForLamp.Invoke(
								refurbishedMap.GetLayer("Front").Tiles[x, y].TileIndex, x, y, "Front");
						}
						else
						{
							__instance.map.GetLayer("Front").Tiles[x, y] = null;
						}
						if (refurbishedMap.GetLayer("Paths").Tiles[x, y] != null
						    && refurbishedMap.GetLayer("Paths").Tiles[x, y].TileIndex == 8)
						{
							Game1.currentLightSources.Add(new LightSource(
								4, new Vector2(x * 64, y * 64), 2f));
						}
						if (showEffects && Game1.random.NextDouble() < 0.58
						                && refurbishedMap.GetLayer("Buildings").Tiles[x, y] == null)
						{
							__instance.temporarySprites.Add(new TemporaryAnimatedSprite(
								6, new Vector2(x * 64, y * 64), Color.White)
							{
								layerDepth = 1f,
								interval = 50f,
								motion = new Vector2(Game1.random.Next(17) / 10f, 0f),
								acceleration = new Vector2(-0.005f, 0f),
								delayBeforeAnimationStart = Game1.random.Next(500)
							});
						}
					}
				}
				Log.D("End of LoadAreaPrefix",
					ModEntry.Instance.Config.DebugMode);
				return false;
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_LoadArea_Prefix)}:\n{e}");
			}

			return true;
		}
		
		/// <summary>
		/// GetNotePosition() throws FatalEngineExecutionError when patched.
		/// Mimics IsJunimoNoteAtArea() using a static p value in place of GetNotePosition().
		/// </summary>
		public static bool CC_IsJunimoNoteAtArea_Prefix(CommunityCenter __instance, ref bool __result, int area)
		{
			try
			{
				Log.T($"CC_IsJunimoNoteAtArea_Prefix(area={area})");
				if (area != ModEntry.CommunityCentreAreaNumber || ModEntry.IsCommunityCentreComplete() || ModEntry.IsAbandonedJojaMartBundleAvailable())
					return true;

				var p = ModEntry.CommunityCentreNotePosition;
				__result = __instance.map.GetLayer("Buildings").Tiles[p.X, p.Y] != null;
				return false;
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_IsJunimoNoteAtArea_Prefix)}:\n{e}");
			}

			return true;
		}

		/// <summary>
		/// GetNotePosition() throws FatalEngineExecutionError when patched.
		/// Mimics ShouldNoteAppearInArea() using a static position in place of GetNotePosition().
		/// </summary>
		public static bool CC_ShouldNoteAppearInArea_Prefix(CommunityCenter __instance, ref bool __result, int area)
		{
			try
			{
				if (ModEntry.IsCommunityCentreComplete() && ModEntry.IsAbandonedJojaMartBundleAvailable()
					&& (Game1.netWorldState.Value.BundleData.Keys.Any(key => key.StartsWith(ModEntry.CommunityCentreAreaName))))
				{
					Log.D($"ShouldNoteAppearInArea removing custom bundle data.",
						ModEntry.Instance.Config.DebugMode);
					ModEntry.Instance.SaveAndUnloadBundleData();
				}

				if (area != ModEntry.CommunityCentreAreaNumber)
					return true;
				__result = !ModEntry.Instance.IsCommunityCentreKitchenCompleted()
					&& __instance.numberOfCompleteBundles() > (ModEntry.Instance.Config.DebugMode ? 0 : 2);
				return false;
			}
			catch (ArgumentOutOfRangeException e)
			{
				Log.D($"Error in {nameof(CC_ShouldNoteAppearInArea_Prefix)}, may be non-critical:\n{e}",
					ModEntry.Instance.Config.DebugMode);
				return false;
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_ShouldNoteAppearInArea_Prefix)}:\n{e}");
			}

			return true;
		}
		
		/// <summary>
		/// GetNotePosition() throws FatalEngineExecutionError when patched.
		/// Mimics AddJunimoNote() using a static p value in place of GetNotePosition().
		/// </summary>
		public static bool CC_AddJunimoNote_Prefix(CommunityCenter __instance, int area)
		{
			try
			{
				Log.T($"CC_AddJunimoNote_Prefix(area={area})");

				if (area != ModEntry.CommunityCentreAreaNumber || ModEntry.IsCommunityCentreComplete() || ModEntry.IsAbandonedJojaMartBundleAvailable())
					return true;

				var p = ModEntry.CommunityCentreNotePosition;
			
				var tileFrames = CommunityCenter.getJunimoNoteTileFrames(area, __instance.Map);
				const string layer = "Buildings";
				__instance.Map.GetLayer(layer).Tiles[p.X, p.Y]
					= new AnimatedTile(__instance.Map.GetLayer(layer), tileFrames, 70L);
				Game1.currentLightSources.Add(new LightSource(
					4, new Vector2(p.X * 64, p.Y * 64), 1f));
				__instance.temporarySprites.Add(new TemporaryAnimatedSprite(
					6, new Vector2(p.X * 64, p.Y * 64), Color.White)
				{
					layerDepth = 1f,
					interval = 50f,
					motion = new Vector2(1f, 0f),
					acceleration = new Vector2(-0.005f, 0f)
				});
				__instance.temporarySprites.Add(new TemporaryAnimatedSprite(
					6, new Vector2(p.X * 64 - 12, p.Y * 64 - 12), Color.White)
				{
					scale = 0.75f,
					layerDepth = 1f,
					interval = 50f,
					motion = new Vector2(1f, 0f),
					acceleration = new Vector2(-0.005f, 0f),
					delayBeforeAnimationStart = 50
				});
				__instance.temporarySprites.Add(new TemporaryAnimatedSprite(
					6, new Vector2(p.X * 64 - 12, p.Y * 64 + 12), Color.White)
				{
					layerDepth = 1f,
					interval = 50f,
					motion = new Vector2(1f, 0f),
					acceleration = new Vector2(-0.005f, 0f),
					delayBeforeAnimationStart = 100
				});
				__instance.temporarySprites.Add(new TemporaryAnimatedSprite(
					6, new Vector2(p.X * 64, p.Y * 64), Color.White)
				{
					layerDepth = 1f,
					scale = 0.75f,
					interval = 50f,
					motion = new Vector2(1f, 0f),
					acceleration = new Vector2(-0.005f, 0f),
					delayBeforeAnimationStart = 150
				});
		
				return false;
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_AddJunimoNote_Prefix)}:\n{e}");
			}

			return true;
		}
		
		/// <summary>
		/// GetNotePosition() throws FatalEngineExecutionError when patched.
		/// Mimics SetViewportToNextJunimoNoteTarget() using a static p value in place of GetNotePosition().
		/// </summary>
		public static bool CC_SetViewportToNextJunimoNoteTarget_Prefix(CommunityCenter __instance)
		{
			try
			{
				var viewportTargets = ModEntry.Instance.Helper.Reflection.GetField<List<int>>(
					__instance, "junimoNotesViewportTargets").GetValue();
				if (viewportTargets.Count < 1 || viewportTargets[0] != ModEntry.CommunityCentreAreaNumber || !ModEntry.Instance.IsCommunityCentreKitchenEnabledByHost())
					return true;

				var reachedTarget = ModEntry.Instance.Helper.Reflection.GetMethod(__instance, "afterViewportGetsToJunimoNotePosition");
				var endFunction = ModEntry.Instance.Helper.Reflection.GetMethod(__instance, "setViewportToNextJunimoNoteTarget");

				var p = ModEntry.CommunityCentreNotePosition;
				Game1.moveViewportTo(new Vector2(p.X, p.Y) * 64f, 5f, 2000, () => reachedTarget.Invoke(), () => endFunction.Invoke());
				return false;
			}
			catch (ArgumentException e)
			{
				Log.D($"Error in {nameof(CC_SetViewportToNextJunimoNoteTarget_Prefix)}, may be non-critical:\n{e}",
					ModEntry.Instance.Config.DebugMode);
				return false;
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_SetViewportToNextJunimoNoteTarget_Prefix)}:\n{e}");
			}

			return true;
		}
		
		/// <summary>
		/// Adds an extra Junimo to the goodbye dance, as the number of Junimos added is otherwise hardcoded.
		/// </summary>
		public static bool CC_StartGoodbyeDance_Prefix(CommunityCenter __instance)
		{
			try
			{
				if (!ModEntry.Instance.IsCommunityCentreKitchenEnabledByHost())
					return true;

				var junimo = __instance.getJunimoForArea(ModEntry.CommunityCentreAreaNumber);
				junimo.Position = new Vector2(22f, 12f) * 64f;
				junimo.stayStill();
				junimo.faceDirection(1);
				junimo.fadeBack();
				junimo.IsInvisible = false;
				junimo.setAlpha(1f);
				junimo.sayGoodbye();
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_StartGoodbyeDance_Prefix)}: {e}");
			}

			return true;
		}
		
		/// <summary>
		/// Adds an extra Junimo to the goodbye dance, as the number of Junimos added is otherwise hardcoded.
		/// </summary>
		public static bool CC_JunimoGoodbyeDance_Prefix(CommunityCenter __instance)
		{
			try
			{
				if (!ModEntry.Instance.IsCommunityCentreKitchenEnabledByHost())
					return true;

				__instance.getJunimoForArea(ModEntry.CommunityCentreAreaNumber).Position = new Vector2(22f, 12f) * 64f;
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_JunimoGoodbyeDance_Prefix)}:\n{e}");
			}
			return true;
		}

		public static bool CC_MessageForAreaCompletion_Prefix(CommunityCenter __instance, ref string __result)
		{
			try
			{
				// todo: what the fuck?
				__result = null;
				return string.IsNullOrEmpty(__result);
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_MessageForAreaCompletion_Prefix)}:\n{e}");
			}
			return true;
		}

		public static bool CC_UpdateWhenCurrentLocation_Prefix(CommunityCenter __instance, GameTime time)
		{
			try
			{
				var index = ModEntry.Instance.Helper.Reflection.GetField<int>(__instance, "restoreAreaIndex").GetValue();
				var timerField = ModEntry.Instance.Helper.Reflection.GetField<int>(__instance, "restoreAreaTimer");
				var timer = timerField.GetValue();
				var phaseField = ModEntry.Instance.Helper.Reflection.GetField<int>(__instance, "restoreAreaPhase");
				var phase = phaseField.GetValue();

				if (timer == 0 || !ModEntry.Instance.IsCommunityCentreKitchenEnabledByHost())
				{
					return true;
				}

				if (index != ModEntry.CommunityCentreAreaNumber)
				{
					// Reload the incomplete custom bundle area if the Pantry was just completed, as it overrides the area
					if (phase == 3 && timer == 2000 && Game1.IsMasterGame && Game1.currentLocation is CommunityCenter
						&& index == 0
						&& __instance.areasComplete.Count > ModEntry.CommunityCentreAreaNumber
						&& !__instance.areasComplete[ModEntry.CommunityCentreAreaNumber])
					{
						ModEntry.Instance.CheckAndTryToUnrenovateKitchen();
					}
					return true;
				}

				var messageAlphaField = ModEntry.Instance.Helper.Reflection.GetField<float>(__instance, "messageAlpha");
				var messageAlpha = messageAlphaField.GetValue();
				var buildUpSound = ModEntry.Instance.Helper.Reflection.GetField<ICue>(__instance, "buildUpSound").GetValue();
				var junimoNotesViewportTargets = ModEntry.Instance.Helper.Reflection.GetField<List<int>>(__instance, "junimoNotesViewportTargets").GetValue();

				__instance.missedRewardsChest.Value.updateWhenCurrentLocation(time, __instance);

				if (timer > 0)
				{
					var old = timer;
					timer -= time.ElapsedGameTime.Milliseconds;
					switch (phase)
					{
						case 0:
							if (timer <= 0)
							{
								timer = 3000;
								phase = 1;
								if (Game1.player.currentLocation == __instance)
								{
									Game1.player.faceDirection(2);
									Game1.player.jump();
									Game1.player.jitterStrength = 1f;
									Game1.player.showFrame(94);

									// THIS IS AN IMPORTANT BIT:
									// Add some mail to flag this bundle as having been completed
									Log.D($"Sending mail for Cooking bundle completion ({ModEntry.MailKitchenCompleted})",
										ModEntry.Instance.Config.DebugMode);
									Game1.player.mailForTomorrow.Add(ModEntry.MailKitchenCompleted + "%&NL&%");
								}
							}
							break;
						case 1:
							if (Game1.IsMasterGame && Game1.random.NextDouble() < 0.4)
							{
								var area = new Rectangle(ModEntry.CommunityCentreArea.X, ModEntry.CommunityCentreArea.Y,
									ModEntry.CommunityCentreArea.Width + 12, ModEntry.CommunityCentreArea.Height + 6);
								var v = Utility.getRandomPositionInThisRectangle(area, Game1.random);
								var i = new Junimo(v * 64f, index, temporary: true);
								if (!__instance.isCollidingPosition(i.GetBoundingBox(), Game1.viewport, i))
								{
									__instance.characters.Add(i);
									ModEntry.Instance.Helper.Reflection.GetField<Multiplayer>(
										typeof(Game1), "multiplayer").GetValue().broadcastSprites(__instance,
										new TemporaryAnimatedSprite((Game1.random.NextDouble() < 0.5) ? 5 : 46,
										v * 64f + new Vector2(16f, 16f), Color.White)
										{
											layerDepth = 1f
										});
									__instance.localSound("tinyWhip");
								}
							}
							if (timer <= 0)
							{
								timer = 999999;
								phase = 2;
								if (Game1.player.currentLocation != __instance)
								{
									break;
								}
								Game1.screenGlowOnce(Color.White, hold: true, 0.005f, 1f);
								if (Game1.soundBank != null)
								{
									buildUpSound = Game1.soundBank.GetCue("wind");
									buildUpSound.SetVariable("Volume", 0f);
									buildUpSound.SetVariable("Frequency", 0f);
									buildUpSound.Play();
								}
								Game1.player.jitterStrength = 2f;
								Game1.player.stopShowingFrame();
							}
							Game1.drawLighting = false;
							break;
						case 2:
							if (buildUpSound != null)
							{
								buildUpSound.SetVariable("Volume", Game1.screenGlowAlpha * 150f);
								buildUpSound.SetVariable("Frequency", Game1.screenGlowAlpha * 150f);
							}
							if (Game1.screenGlowAlpha >= Game1.screenGlowMax)
							{
								messageAlpha += 0.008f;
								messageAlpha = Math.Min(messageAlpha, 1f);
							}
							if ((Game1.screenGlowAlpha == Game1.screenGlowMax
								|| Game1.currentLocation != __instance) && timer > 5200)
							{
								timer = 5200;
							}
							if (timer < 5200 && Game1.random.NextDouble() < (5200 - timer) / 10000f)
							{
								__instance.localSound((Game1.random.NextDouble() < 0.5) ? "dustMeep" : "junimoMeep1");
							}
							if (timer > 0)
							{
								break;
							}
							timer = 2000;
							messageAlpha = 0f;
							phase = 3;
							if (Game1.IsMasterGame)
							{
								for (int j = __instance.characters.Count - 1; j >= 0; j--)
								{
									if (__instance.characters[j] is Junimo
										&& (__instance.characters[j] as Junimo).temporaryJunimo.Value)
									{
										__instance.characters.RemoveAt(j);
									}
								}
							}
							if (Game1.player.currentLocation == __instance)
							{
								Game1.screenGlowHold = false;
								__instance.loadArea(index);
								if (buildUpSound != null)
								{
									buildUpSound.Stop(AudioStopOptions.Immediate);
								}
								__instance.localSound("wand");
								Game1.changeMusicTrack("junimoStarSong");
								__instance.localSound("woodyHit");
								Game1.flashAlpha = 1f;
								Game1.player.stopJittering();
								Game1.drawLighting = true;
							}
							break;
						case 3:
							if (old > 1000 && timer <= 1000)
							{
								var junimo = __instance.getJunimoForArea(index);
								if (junimo != null && Game1.IsMasterGame)
								{
									if (!junimo.holdingBundle.Value)
									{
										junimo.Position = Utility.getRandomAdjacentOpenTile(Utility.PointToVector2(ModEntry.CommunityCentreNotePosition), __instance) * 64f;
										var iter = 0;
										while (__instance.isCollidingPosition(junimo.GetBoundingBox(), Game1.viewport, junimo) && iter < 20)
										{
											junimo.Position = Utility.getRandomPositionInThisRectangle(ModEntry.CommunityCentreArea, Game1.random) * 64f;
											iter++;
										}
										if (iter < 20)
										{
											junimo.fadeBack();
										}
									}
									junimo.returnToJunimoHutToFetchStar(__instance);
								}
							}
							if (timer <= 0 && !ModEntry.Instance.Helper.Reflection.GetField<bool>(
								__instance, "_isWatchingJunimoGoodbye").GetValue())
							{
								Game1.freezeControls = false;
							}
							break;
					}
				}
				else if (Game1.activeClickableMenu == null && junimoNotesViewportTargets != null
					&& junimoNotesViewportTargets.Count > 0 && !Game1.isViewportOnCustomPath())
				{
					ModEntry.Instance.Helper.Reflection.GetMethod(__instance, "setViewportToNextJunimoNoteTarget").Invoke();
				}

				timerField.SetValue(timer);
				phaseField.SetValue(phase);

				return false;
			}
			catch (Exception e)
			{
				Log.E($"Error in {nameof(CC_UpdateWhenCurrentLocation_Prefix)}:\n{e}");
			}
			return true;
		}

		public static bool Bush_inBloom_Prefix(Bush __instance, ref bool __result, string season, int dayOfMonth)
		{
			if (!(__instance is CustomBush bush))
				return true;
			__result = CustomBush.InBloomBehaviour(bush, season, dayOfMonth);
			return false;
		}

		public static bool Bush_getEffectiveSize_Prefix(Bush __instance, ref int __result)
		{
			if (!(__instance is CustomBush bush))
				return true;
			__result = CustomBush.GetEffectiveSizeBehaviour(bush);
			return false;
		}

		public static bool Bush_isDestroyable_Prefix(Bush __instance, ref bool __result)
		{
			if (!(__instance is CustomBush bush))
				return true;
			__result = CustomBush.IsDestroyableBehaviour(bush);
			return false;
		}

		public static bool Bush_shake_Prefix(Bush __instance, Vector2 tileLocation)
		{
			if (!(__instance is CustomBush bush))
				return true;
			CustomBush.ShakeBehaviour(bush, tileLocation);
			return false;
		}
	}
}
