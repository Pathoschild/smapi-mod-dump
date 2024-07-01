/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using System;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Minigames;
using mouahrarasModuleCollection.ArcadeGames.PayToPlay.Managers;
using mouahrarasModuleCollection.ArcadeGames.PayToPlay.Utilities;

namespace mouahrarasModuleCollection.ArcadeGames.PayToPlay.Patches
{
	internal class AbigailGamePatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.draw), new Type[] { typeof(SpriteBatch) }),
				postfix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(DrawPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.tick), new Type[] { typeof(GameTime) }),
				prefix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(TickPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.tick), new Type[] { typeof(GameTime) }),
				postfix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(TickPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.receiveKeyPress), new Type[] { typeof(Keys) }),
				postfix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(ReceiveKeyPressPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.forceQuit)),
				postfix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(ForceQuitPostfix))
			);
		}

		private static void DrawPostfix(SpriteBatch b)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlay)
				return;
			if (AbigailGame.playingWithAbigail)
				return;
			if (!AbigailGame.onStartMenu)
				return;

			Rectangle insertCoinSourceRectangle = new(0, 0, 324, 27);
			Rectangle pressAnyButtonSourceRectangle = new(0, 30, 324, 27);
			Rectangle loadingSourceRectangle = new(0, 60, 324, 27);
			Rectangle credit0SourceRectangle = new(0, 90, 324, 27);
			Rectangle credit1SourceRectangle = new(0, 120, 324, 27);
			Rectangle freeSourceRectangle = new(0, 150, 324, 27);

			b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
			if (Game1.currentLocation is not null && Game1.currentLocation.Name.Equals("Saloon"))
			{
				if (PayToPlayUtility.OnInsertCoinMenu)
				{
					b.Draw(AssetManager.JourneyOfThePrairieKing, new Vector2(Game1.viewport.Width / 2 - insertCoinSourceRectangle.Width / 2, Game1.viewport.Height / 2 - insertCoinSourceRectangle.Height / 2 + 8 * AbigailGame.baseTileSize), insertCoinSourceRectangle, Color.White * (Game1.currentGameTime.TotalGameTime.Seconds % 2 == 0 ? 0.5f : 1f), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
					b.Draw(AssetManager.JourneyOfThePrairieKing, new Vector2(AbigailGame.topLeftScreenCoordinate.X + AbigailGame.baseTileSize, AbigailGame.topLeftScreenCoordinate.Y + 384 * 2 - credit0SourceRectangle.Height - AbigailGame.baseTileSize), credit0SourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
				}
				else
				{
					b.Draw(AssetManager.JourneyOfThePrairieKing, new Vector2(Game1.viewport.Width / 2 - loadingSourceRectangle.Width / 2, Game1.viewport.Height / 2 - loadingSourceRectangle.Height / 2 + 8 * AbigailGame.baseTileSize), loadingSourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
					b.Draw(AssetManager.JourneyOfThePrairieKing, new Vector2(AbigailGame.topLeftScreenCoordinate.X + AbigailGame.baseTileSize, AbigailGame.topLeftScreenCoordinate.Y + 384 * 2 - credit1SourceRectangle.Height - AbigailGame.baseTileSize), credit1SourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
				}
				if (Game1.player.jotpkProgress.Value == null)
					Game1.dayTimeMoneyBox.drawMoneyBox(b, Game1.dayTimeMoneyBox.xPositionOnScreen, 0);
			}
			else
			{
				if (PayToPlayUtility.OnInsertCoinMenu)
				{
					b.Draw(AssetManager.JourneyOfThePrairieKing, new Vector2(Game1.viewport.Width / 2 - pressAnyButtonSourceRectangle.Width / 2, Game1.viewport.Height / 2 - pressAnyButtonSourceRectangle.Height / 2 + 8 * AbigailGame.baseTileSize), pressAnyButtonSourceRectangle, Color.White * (Game1.currentGameTime.TotalGameTime.Seconds % 2 == 0 ? 0.5f : 1f), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
				}
				else
				{
					b.Draw(AssetManager.JourneyOfThePrairieKing, new Vector2(Game1.viewport.Width / 2 - loadingSourceRectangle.Width / 2, Game1.viewport.Height / 2 - loadingSourceRectangle.Height / 2 + 8 * AbigailGame.baseTileSize), loadingSourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
				}
				b.Draw(AssetManager.JourneyOfThePrairieKing, new Vector2(AbigailGame.topLeftScreenCoordinate.X + AbigailGame.baseTileSize, AbigailGame.topLeftScreenCoordinate.Y + 384 * 2 - freeSourceRectangle.Height - AbigailGame.baseTileSize), freeSourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
			b.End();
		}

		private static bool TickPrefix(AbigailGame __instance)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlay)
				return true;
			if (AbigailGame.playingWithAbigail)
				return true;
			RestartIfRequired(__instance);
			if (Game1.player.jotpkProgress.Value != null)
			{
				PayToPlayUtility.OnInsertCoinMenu = false;
				return true;
			}
			if (!AbigailGame.onStartMenu || !PayToPlayUtility.OnInsertCoinMenu)
				return true;

			AbigailGame.startTimer = int.MaxValue;
			InsertCoinMenuMusic();
			if (Game1.IsChatting || Game1.textEntry != null)
				return true;
			InsertCoinMenuInputs();
			return true;
		}

		private static void RestartIfRequired(AbigailGame __instance)
		{
			if ((AbigailGame.gameOver || __instance.gamerestartTimer > 0) && !AbigailGame.endCutscene)
			{
				__instance.unload();
				Game1.currentMinigame = new AbigailGame();
				PayToPlayUtility.Reset();
			}
		}

		private static void InsertCoinMenuMusic()
		{
			if (Game1.soundBank != null)
			{
				ICue overworldSong = (ICue)typeof(AbigailGame).GetField("overworldSong", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

				if (overworldSong == null || !overworldSong.IsPlaying)
				{
					overworldSong = Game1.soundBank.GetCue("Cowboy_OVERWORLD");
					overworldSong.Play();
					Game1.musicPlayerVolume = Game1.options.musicVolumeLevel;
					Game1.musicCategory.SetVolume(Game1.musicPlayerVolume);
					typeof(AbigailGame).GetField("overworldSong", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, overworldSong);
				}
			}
		}

		private static void InsertCoinMenuInputs()
		{
			bool justTryToInsertCoin = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed
				|| Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.useToolButton)
				|| Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.actionButton)
				|| Game1.input.GetKeyboardState().IsKeyDown(Keys.Space)
				|| Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftShift)
				|| Game1.input.GetGamePadState().IsButtonDown(Buttons.A)
				|| Game1.input.GetGamePadState().IsButtonDown(Buttons.B);

			if (justTryToInsertCoin && !PayToPlayUtility.TriedToInsertCoin)
			{
				if (Game1.currentLocation is not null && Game1.currentLocation.Name.Equals("Saloon"))
				{
					if (Game1.player.Money < ModEntry.Config.ArcadeGamesPayToPlayCoinPerJotPKGame)
					{
						Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
						Game1.playSound("cancel");
					}
					else
					{
						Game1.player.Money -= ModEntry.Config.ArcadeGamesPayToPlayCoinPerJotPKGame;
						Game1.playSound("Pickup_Coin15");
						AbigailGame.startTimer = 1500;
						PayToPlayUtility.OnInsertCoinMenu = false;
					}
				}
				else
				{
					Game1.playSound("Pickup_Coin15");
					AbigailGame.startTimer = 1500;
					PayToPlayUtility.OnInsertCoinMenu = false;
				}
			}
			PayToPlayUtility.TriedToInsertCoin = justTryToInsertCoin;
		}

		private static void TickPostfix(bool __result)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlay)
				return;
			if (AbigailGame.playingWithAbigail)
				return;

			if (__result == true)
				PayToPlayUtility.Reset();
		}

		private static void ReceiveKeyPressPostfix(AbigailGame __instance, Keys k)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlay)
				return;
			if (AbigailGame.playingWithAbigail)
				return;

			if (Game1.input.GetGamePadState().IsButtonDown(Buttons.Back) || k.Equals(Keys.Escape))
				__instance.quit = true;
		}

		private static void ForceQuitPostfix(bool __result)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlay)
				return;
			if (AbigailGame.playingWithAbigail)
				return;

			if (__result == true)
				PayToPlayUtility.Reset();
		}
	}
}
