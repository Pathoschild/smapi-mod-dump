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
using mouahrarasModuleCollection.ArcadeGames.PayToPlay.Utilities;

namespace mouahrarasModuleCollection.ArcadeGames.PayToPlay.Patches
{
	internal class MineCartPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(MineCart), nameof(MineCart.ShowTitle)),
				postfix: new HarmonyMethod(typeof(MineCartPatch), nameof(ShowTitlePostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(MineCart), nameof(MineCart.draw), new Type[] { typeof(SpriteBatch) }),
				postfix: new HarmonyMethod(typeof(MineCartPatch), nameof(DrawPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(MineCart), nameof(MineCart.UpdateInput)),
				prefix: new HarmonyMethod(typeof(MineCartPatch), nameof(UpdateInputPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(MineCart), nameof(MineCart.QuitGame)),
				postfix: new HarmonyMethod(typeof(MineCartPatch), nameof(QuitGamePostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(MineCart), nameof(MineCart.forceQuit)),
				postfix: new HarmonyMethod(typeof(MineCartPatch), nameof(ForceQuitPostfix))
			);
		}

		private static void ShowTitlePostfix()
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlay)
				return;

			PayToPlayUtility.Reset();
		}

		private static void DrawPostfix(MineCart __instance, SpriteBatch b)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlay)
				return;
			if (__instance.gameState != MineCart.GameStates.Title)
				return;

			float scale = __instance.GetPixelScale() / 5f;
			int screenWidth = (int)((int)typeof(MineCart).GetField("screenWidth", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) * __instance.GetPixelScale());
			int screenHeight = (int)((int)typeof(MineCart).GetField("screenHeight", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) * __instance.GetPixelScale());
			string insertCoinText = ModEntry.Helper.Translation.Get("ArcadeGames.PayToPlay.JunimoKart_InsertCoin");
			string loadingText = ModEntry.Helper.Translation.Get("ArcadeGames.PayToPlay.JunimoKart_Loading");
			string pressAnyButtonText = ModEntry.Helper.Translation.Get("ArcadeGames.PayToPlay.JunimoKart_PressAnyButton");
			string credit0Text = ModEntry.Helper.Translation.Get("ArcadeGames.PayToPlay.JunimoKart_Credit0");
			string credit1Text = ModEntry.Helper.Translation.Get("ArcadeGames.PayToPlay.JunimoKart_Credit1");
			string freeText = ModEntry.Helper.Translation.Get("ArcadeGames.PayToPlay.JunimoKart_Free");
			Vector2 insertCoinDimension = Game1.dialogueFont.MeasureString(insertCoinText) * scale;
			Vector2 loadingDimension = Game1.dialogueFont.MeasureString(loadingText) * scale;
			Vector2 pressAnyButtonDimension = Game1.dialogueFont.MeasureString(pressAnyButtonText) * scale;
			Vector2 credit0Dimension = Game1.dialogueFont.MeasureString(credit0Text) * scale;
			Vector2 credit1Dimension = Game1.dialogueFont.MeasureString(credit1Text) * scale;
			Vector2 freeDimension = Game1.dialogueFont.MeasureString(freeText) * scale;
			int gameMode = (int)typeof(MineCart).GetField("gameMode", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

			b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
			if (Game1.currentLocation is not null && Game1.currentLocation.Name.Equals("Saloon"))
			{
				if (PayToPlayUtility.OnInsertCoinMenu)
				{
					if (gameMode == 3)
						b.DrawString(Game1.dialogueFont, insertCoinText, new Vector2(Game1.viewport.Width / 2 - insertCoinDimension.X / 2, 3 * Game1.viewport.Height / 4 - insertCoinDimension.Y / 2), Utility.GetPrismaticColor() * (Game1.currentGameTime.TotalGameTime.Seconds % 2 == 0 ? 0.5f : 1f), 0f, Vector2.Zero, scale, SpriteEffects.None, 0.199f);
					b.DrawString(Game1.dialogueFont, credit0Text, new Vector2((Game1.viewport.Width - screenWidth) / 2 + __instance.tileSize, (Game1.viewport.Height - screenHeight) / 2 + screenHeight - credit0Dimension.Y - __instance.tileSize), Utility.GetPrismaticColor(), 0f, Vector2.Zero, scale, SpriteEffects.None, 0.199f);
				}
				else
				{
					if (gameMode == 3)
						b.DrawString(Game1.dialogueFont, loadingText, new Vector2(Game1.viewport.Width / 2 - loadingDimension.X / 2, 3 * Game1.viewport.Height / 4 - loadingDimension.Y / 2), Utility.GetPrismaticColor() * (Game1.currentGameTime.TotalGameTime.Seconds % 2 == 0 ? 0.5f : 1f), 0f, Vector2.Zero, scale, SpriteEffects.None, 0.199f);
					b.DrawString(Game1.dialogueFont, credit1Text, new Vector2((Game1.viewport.Width - screenWidth) / 2 + __instance.tileSize, (Game1.viewport.Height - screenHeight) / 2 + screenHeight - credit1Dimension.Y - __instance.tileSize), Utility.GetPrismaticColor(), 0f, Vector2.Zero, scale, SpriteEffects.None, 0.199f);
				}
				Game1.dayTimeMoneyBox.drawMoneyBox(b, Game1.dayTimeMoneyBox.xPositionOnScreen, 0);
			}
			else
			{
				if (PayToPlayUtility.OnInsertCoinMenu)
				{
					if (gameMode == 3)
						b.DrawString(Game1.dialogueFont, pressAnyButtonText, new Vector2(Game1.viewport.Width / 2 - pressAnyButtonDimension.X / 2, 3 * Game1.viewport.Height / 4 - pressAnyButtonDimension.Y / 2), Utility.GetPrismaticColor() * (Game1.currentGameTime.TotalGameTime.Seconds % 2 == 0 ? 0.5f : 1f), 0f, Vector2.Zero, scale, SpriteEffects.None, 0.199f);
				}
				else
				{
					if (gameMode == 3)
						b.DrawString(Game1.dialogueFont, loadingText, new Vector2(Game1.viewport.Width / 2 - loadingDimension.X / 2, 3 * Game1.viewport.Height / 4 - loadingDimension.Y / 2), Utility.GetPrismaticColor() * (Game1.currentGameTime.TotalGameTime.Seconds % 2 == 0 ? 0.5f : 1f), 0f, Vector2.Zero, scale, SpriteEffects.None, 0.199f);
				}
				b.DrawString(Game1.dialogueFont, freeText, new Vector2((Game1.viewport.Width - screenWidth) / 2 + __instance.tileSize, (Game1.viewport.Height - screenHeight) / 2 + screenHeight - freeDimension.Y - __instance.tileSize), Utility.GetPrismaticColor(), 0f, Vector2.Zero, scale, SpriteEffects.None, 0.199f);
			}
			b.End();
		}

		private static bool UpdateInputPrefix(MineCart __instance)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlay)
				return true;
			if (__instance.gameState != MineCart.GameStates.Title || !PayToPlayUtility.OnInsertCoinMenu)
				return true;
			if (__instance.pauseBeforeTitleFadeOutTimer != 0f || (float)typeof(MineCart).GetField("screenDarkness", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) != 0f || __instance.fadeDelta > 0f)
				return true;

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
					if (Game1.player.Money < ModEntry.Config.ArcadeGamesPayToPlayCoinPerJKGame)
					{
						Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
						Game1.playSound("cancel");
					}
					else
					{
						Game1.player.Money -= ModEntry.Config.ArcadeGamesPayToPlayCoinPerJKGame;
						PayToPlayUtility.OnInsertCoinMenu = false;
						return true;
					}
				}
				else
				{
					PayToPlayUtility.OnInsertCoinMenu = false;
					return true;
				}
			}
			PayToPlayUtility.TriedToInsertCoin = justTryToInsertCoin;
			return !justTryToInsertCoin;
		}

		private static void QuitGamePostfix()
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlay)
				return;

			PayToPlayUtility.Reset();
		}

		private static void ForceQuitPostfix(bool __result)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlay)
				return;

			if (__result == true)
				PayToPlayUtility.Reset();
		}
	}
}
