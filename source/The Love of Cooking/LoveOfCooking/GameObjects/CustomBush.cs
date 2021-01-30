/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;

namespace LoveOfCooking
{
	public class CustomBush : Bush
	{
		public enum BushVariety
		{
			Nettle,
			Redberry
		}

		protected Texture2D CustomTexture => ModEntry.SpriteSheet;
		protected Rectangle SourceRectangle;
		protected IReflectedField<float> AlphaField, ShakeField, MaxShakeField;
		
		public BushVariety Variety;
		public int DaysToMature;
		public int DaysBetweenProduceWhenEmpty;
		public int DaysBetweenAdditionalProduce;
		public int HeldItemId = -1;
		public int HeldItemQuantity;
		public bool DestroyWhenHarvested;

		public const int NettlesDamage = 4;
		public const string NettleBuffSource = ModEntry.ObjectPrefix + "NettleBuff";

		public CustomBush(Vector2 tile, GameLocation location, BushVariety variety)
			: base(tile, variety == BushVariety.Redberry ? 2 : 3, location)
		{
			currentTileLocation = tile;
			currentLocation = location;
			Variety = variety;
			Init();
		}

		protected void Init()
		{
			AlphaField = ModEntry.Instance.Helper.Reflection.GetField<float>(this, "alpha");
			ShakeField = ModEntry.Instance.Helper.Reflection.GetField<float>(this, "shakeRotation");
			MaxShakeField = ModEntry.Instance.Helper.Reflection.GetField<float>(this, "maxShake");

			drawShadow.Set(false);
			flipped.Set(Game1.random.NextDouble() < 0.5);
			if (currentLocation.IsGreenhouse)
				greenhouseBush.Value = true;
			
			if (Variety == BushVariety.Nettle)
			{
				DaysToMature = -1;
				DaysBetweenProduceWhenEmpty = -1;
				DaysBetweenAdditionalProduce = -1;
				HeldItemId = ModEntry.JsonAssets.GetObjectId("Nettles");
				HeldItemQuantity = 1;
				DestroyWhenHarvested = true;
			}
			else if (Variety == BushVariety.Redberry)
			{
				DaysToMature = 17;
				DaysBetweenProduceWhenEmpty = 4;
				DaysBetweenAdditionalProduce = 2;
				HeldItemId = ModEntry.JsonAssets.GetObjectId("Redberries");
				HeldItemQuantity = 0;
				DestroyWhenHarvested = false;
			}
		}

		public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
		{
			if (Variety == BushVariety.Redberry
			    && tileSheetOffset == 0
			    && Game1.random.NextDouble() < 0.2
			    && inBloom(Game1.currentSeason, Game1.dayOfMonth))
				tileSheetOffset.Value = 1;
			else if (!Game1.currentSeason.Equals("summer") && !inBloom(Game1.currentSeason, Game1.dayOfMonth))
				tileSheetOffset.Value = 0;
			
			SetUpSourceRectangle();
			if (Math.Abs(6f - tileLocation.X) > 0.001 || Math.Abs(7f - tileLocation.X) > 0.001 || environment.Name != "Sunroom")
				health = 0f;
		}

		public override bool seasonUpdate(bool onLoad)
		{
			if (!Game1.IsMultiplayer || Game1.IsServer)
			{
				if (Variety == BushVariety.Redberry && Game1.currentSeason.Equals("summer") && Game1.random.NextDouble() < 0.5)
					tileSheetOffset.Value = 1;
				else
					tileSheetOffset.Value = 0;
				loadSprite();
			}
			return false;
		}

		public override bool isActionable()
		{
			return true;
		}
		
		public override bool performUseAction(Vector2 tileLocation, GameLocation location)
		{
			if (Math.Abs(0f - MaxShakeField.GetValue()) < 0.001f
			    && (greenhouseBush || Variety == BushVariety.Redberry || !Game1.currentSeason.Equals("winter")))
				location.localSound("leafrustle");
			
			ModEntry.Instance.Helper.Reflection.GetMethod(this, "shake").Invoke(tileLocation, false);
			return true;
		}
		
		public override Rectangle getBoundingBox(Vector2 tileLocation)
		{
			if (Variety == BushVariety.Nettle)
				return new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);
			if (Variety == BushVariety.Redberry)
				return new Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, 128, 64);
			return Rectangle.Empty;
		}

		public override void loadSprite()
		{
			var r = new Random((int)Game1.stats.DaysPlayed
			                   + (int)Game1.uniqueIDForThisGame + (int)tilePosition.X + (int)tilePosition.Y * 777);
			if (Variety == BushVariety.Nettle && r.NextDouble() < 0.5)
			{
				tileSheetOffset.Value = 1;
			}
			else if (Variety == BushVariety.Redberry)
			{
				tileSheetOffset.Value = inBloom(Game1.currentSeason, Game1.dayOfMonth) ? 1 : 0;
			}
			SetUpSourceRectangle();
		}
		
		public void SetUpSourceRectangle()
		{
			if (Variety == BushVariety.Nettle)
			{
				var dimen = new Point(16, 32);
				SourceRectangle = new Rectangle(
					0 + tileSheetOffset * dimen.X,
					16,
					dimen.X,
					dimen.Y);
			}
			else if (Variety == BushVariety.Redberry)
			{
				var dimen = new Point(32, 32);
				var seasonNumber = greenhouseBush.Value ? 0 : Utility.getSeasonNumber(Game1.currentSeason);
				var age = getAge();
				SourceRectangle = new Rectangle(
					seasonNumber * dimen.X + Math.Min(2, age / 10) * dimen.X + tileSheetOffset * dimen.X,
					16 + 16,
					dimen.X,
					dimen.Y);
			}
		}
		
		public override Rectangle getRenderBounds(Vector2 tileLocation)
		{
			if (Variety == BushVariety.Nettle)
				return new Rectangle((int)tileLocation.X * 64, (int)(tileLocation.Y - 1f) * 64, 64, 160);
			if (Variety == BushVariety.Redberry)
				return new Rectangle((int)tileLocation.X * 64, (int)(tileLocation.Y - 2f) * 64, 128, 256);
			return Rectangle.Empty;
		}

		public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
		{
			var screenPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(
				tileLocation.X * 64f + 64f / 2,
				(tileLocation.Y + 1f) * 64f));
			spriteBatch.Draw(
				CustomTexture,
				screenPosition,
				SourceRectangle,
				Color.White * AlphaField.GetValue(),
				ShakeField.GetValue(),
				new Vector2(
					SourceRectangle.Width / 2,
					SourceRectangle.Height),
				4f,
				flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				(getBoundingBox(tileLocation).Center.Y + 48) / 10000f - tileLocation.X / 1000000f);

		}
		
		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 positionOnScreen,
			Vector2 tileLocation, float scale, float layerDepth)
		{
			layerDepth += positionOnScreen.X / 100000f;
			spriteBatch.Draw(
				texture.Value,
				positionOnScreen + new Vector2(0f, -64f * scale),
				new Rectangle(32, 96, 16, 32),
				Color.White,
				0f,
				Vector2.Zero,
				scale,
				flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				layerDepth + (positionOnScreen.Y + 448f * scale - 1f) / 20000f);
		}
		
		/* HarmonyPatch behaviours */

		public static bool InBloomBehaviour(CustomBush bush, string season, int dayOfMonth)
		{
			return bush.Variety != BushVariety.Nettle 
			       && (bush.getAge() >= bush.DaysToMature
			           && dayOfMonth >= 22 && (!season.Equals("winter") || bush.greenhouseBush.Value));
		}

		public static int GetEffectiveSizeBehaviour(CustomBush bush)
		{
			return bush.Variety == BushVariety.Nettle ? 0 : 1;
		}

		public static bool IsDestroyableBehaviour(CustomBush bush)
		{
			return true;
		}

		public static void ShakeBehaviour(CustomBush bush, Vector2 tileLocation)
		{
			if (bush.Variety == BushVariety.Redberry)
			{
				for (var i = 0; i < bush.HeldItemQuantity; ++i)
					Game1.createObjectDebris(bush.HeldItemId, (int)tileLocation.X, (int)tileLocation.Y);
			}

			if (bush.Variety == BushVariety.Nettle)
			{
				DelayedAction.playSoundAfterDelay("leafrustle", 100);
				Game1.player.takeDamage(NettlesDamage + Game1.player.resilience, true, null);
				if (Game1.player.health < 1)
					Game1.player.health = 1;
				Game1.buffsDisplay.otherBuffs.RemoveAll(b => b.source == NettleBuffSource);
				Game1.buffsDisplay.addOtherBuff(new Buff(
					0, 0, 0, 0, 0, 0, 0,
					0, 0, -1, 0, 0, 10,
					NettleBuffSource, ModEntry.Instance.Helper.Translation.Get("buff.nettles.inspect")));
			}
		}
	}
}
