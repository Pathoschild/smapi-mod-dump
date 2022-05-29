/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace EndgameBundles
{
	internal class EndgameBundlePage : ClickableComponent
	{
		public EndgameBundle bundle;

		public int bundleColor;

		public int bundleIndex;
		public int numberOfIngredientSlots;

		// Used for localization?
		public Texture2D? bundleTextureOverride;
		public int bundleTextureIndexOverride = -1;

		// Bundle variables
		public const float shakeRate = (float)Math.PI / 200f;
		public const float shakeDecayRate = 0.00306796166f;
		public const int Color_Green = 0;
		public const int Color_Purple = 1;
		public const int Color_Orange = 2;
		public const int Color_Yellow = 3;
		public const int Color_Red = 4;
		public const int Color_Blue = 5;
		public const int Color_Teal = 6;
		public const float DefaultShakeForce = (float)Math.PI * 3f / 128f;
		//public string rewardDescription;
		//public List<BundleIngredientDescription> ingredients;
		
		public int completionTimer;
		public bool complete;
		public bool depositsAllowed = true;
		
		public TemporaryAnimatedSprite sprite;
		private float maxShake;
		private bool shakeLeft;


		public EndgameBundlePage(int bundleIndex, EndgameBundle bundle, Point position, string textureName, EndgameBundleMenu menu)
			: base(new Rectangle(position.X, position.Y, 64, 64), "")
		{
			this.bundle = bundle;

			this.bundleIndex = bundleIndex;
			//string[] split = rawBundleInfo.Split('/');
			name = bundle.Name;
			label = bundle.Name;

			if (LocalizedContentManager.CurrentLanguageCode != 0)
			{
				// TODO: Localization
				label = bundle.Name;
			}

			//rewardDescription = split[1];

			//TODO: Localization support for the images
			/*if (split.Length > 5 && (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en || split.Length > 6))
			{
				string bundle_image_override = split[5];

				try
				{
					if (bundle_image_override.Contains(':'))
					{
						string[] bundle_image_parts = bundle_image_override.Split(':');
						bundleTextureOverride = Game1.content.Load<Texture2D>(bundle_image_parts[0]);
						bundleTextureIndexOverride = int.Parse(bundle_image_parts[1]);
					}
					else
					{
						bundleTextureIndexOverride = int.Parse(bundle_image_override);
					}
				}
				catch (Exception)
				{
					bundleTextureOverride = null;
					bundleTextureIndexOverride = -1;
				}
			}*/

			// What's this part?
			/*string[] ingredientsSplit = split[2].Split(' ');
			complete = true;
			ingredients = new List<BundleIngredientDescription>();
			int tally = 0;

			for (int i = 0; i < ingredientsSplit.Length; i += 3)
			{
				ingredients.Add(new BundleIngredientDescription(Convert.ToInt32(ingredientsSplit[i]), Convert.ToInt32(ingredientsSplit[i + 1]), Convert.ToInt32(ingredientsSplit[i + 2]), completedIngredientsList[i / 3]));

				if (!completedIngredientsList[i / 3])
				{
					complete = false;
				}
				else
				{
					tally++;
				}
			}*/

			Random random = new Random();
			bundleColor = random.Next(0, 7);
			//bundleColor = Convert.ToInt32(split[3]);

			/*int count = 4;

			if (LocalizedContentManager.CurrentLanguageCode != 0)
			{
				count = 5;
			}*/

			//numberOfIngredientSlots = (split.Length > count) ? Convert.ToInt32(split[4]) : ingredients.Count;
			numberOfIngredientSlots = bundle.Ingredients.Count > bundle.MinRequiredAmount && bundle.MinRequiredAmount != -1 ? bundle.MinRequiredAmount : bundle.Ingredients.Count;

			/*if (tally >= numberOfIngredientSlots)
			{
				complete = true;
			}*/

			sprite = new TemporaryAnimatedSprite(textureName, new Rectangle(bundleColor * 256 % 512, 244 + bundleColor * 256 / 512 * 16, 16, 16), 70f, 3, 99999, new Vector2(bounds.X, bounds.Y), flicker: false, flipped: false, 0.8f, 0f, Color.White, 4f, 0f, 0f, 0f)
			{
				pingPong = true,
				paused = true
			};

			sprite.sourceRect.X += sprite.sourceRect.Width;

			if (name.ToLower().Contains(Game1.currentSeason) && !complete)
			{
				Shake();
			}

			if (complete)
			{
				CompletionAnimation(menu, playSound: false);
			}
		}

		/*public EndgameBundlePage(int bundleIndex, string rawBundleInfo, bool[] completedIngredientsList, Point position, string textureName, EndgameBundleMenu menu)
			: base(new Rectangle(position.X, position.Y, 64, 64), "")
		{
			if (menu.fromGameMenu)
			{
				depositsAllowed = false;
			}

			this.bundleIndex = bundleIndex;
			string[] split = rawBundleInfo.Split('/');
			name = split[0];
			label = split[0];

			if (LocalizedContentManager.CurrentLanguageCode != 0)
			{
				label = split[^1];
			}

			rewardDescription = split[1];

			if (split.Length > 5 && (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en || split.Length > 6))
			{
				string bundle_image_override = split[5];

				try
				{
					if (bundle_image_override.Contains(':'))
					{
						string[] bundle_image_parts = bundle_image_override.Split(':');
						bundleTextureOverride = Game1.content.Load<Texture2D>(bundle_image_parts[0]);
						bundleTextureIndexOverride = int.Parse(bundle_image_parts[1]);
					}
					else
					{
						bundleTextureIndexOverride = int.Parse(bundle_image_override);
					}
				}
				catch (Exception)
				{
					bundleTextureOverride = null;
					bundleTextureIndexOverride = -1;
				}
			}

			string[] ingredientsSplit = split[2].Split(' ');
			complete = true;
			ingredients = new List<BundleIngredientDescription>();
			int tally = 0;

			for (int i = 0; i < ingredientsSplit.Length; i += 3)
			{
				ingredients.Add(new BundleIngredientDescription(Convert.ToInt32(ingredientsSplit[i]), Convert.ToInt32(ingredientsSplit[i + 1]), Convert.ToInt32(ingredientsSplit[i + 2]), completedIngredientsList[i / 3]));
				
				if (!completedIngredientsList[i / 3])
				{
					complete = false;
				}
				else
				{
					tally++;
				}
			}

			bundleColor = Convert.ToInt32(split[3]);
			int count = 4;

			if (LocalizedContentManager.CurrentLanguageCode != 0)
			{
				count = 5;
			}

			numberOfIngredientSlots = (split.Length > count) ? Convert.ToInt32(split[4]) : ingredients.Count;

			if (tally >= numberOfIngredientSlots)
			{
				complete = true;
			}

			sprite = new TemporaryAnimatedSprite(textureName, new Rectangle(bundleColor * 256 % 512, 244 + bundleColor * 256 / 512 * 16, 16, 16), 70f, 3, 99999, new Vector2(bounds.X, bounds.Y), flicker: false, flipped: false, 0.8f, 0f, Color.White, 4f, 0f, 0f, 0f)
			{
				pingPong = true,
				paused = true
			};

			sprite.sourceRect.X += sprite.sourceRect.Width;

			if (name.ToLower().Contains(Game1.currentSeason) && !complete)
			{
				Shake();
			}

			if (complete)
			{
				CompletionAnimation(menu, playSound: false);
			}
		}*/

		public Item GetReward()
		{
			EndgameBundleReward reward = bundle.Reward;
			return Utility.getItemFromStandardTextDescription(reward.ToString(), Game1.player);

			//return Utility.getItemFromStandardTextDescription(rewardDescription, Game1.player);
		}

		public void Shake(float force = (float)Math.PI * 3f / 128f)
		{
			if (sprite.paused)
			{
				maxShake = force;
			}
		}

		public void Shake(int extraInfo)
		{
			maxShake = (float)Math.PI * 3f / 128f;

			if (extraInfo == 1)
			{
				Game1.playSound("leafrustle");

				EndgameBundleMenu.tempSprites.Add(new TemporaryAnimatedSprite(50, sprite.position, EndgameBundlePage.GetColorFromColorIndex(bundleColor))
				{
					motion = new Vector2(-1f, 0.5f),
					acceleration = new Vector2(0f, 0.02f)
				});

				EndgameBundleMenu.tempSprites.Last().sourceRect.Y++;
				EndgameBundleMenu.tempSprites.Last().sourceRect.Height--;
				
				EndgameBundleMenu.tempSprites.Add(new TemporaryAnimatedSprite(50, sprite.position, EndgameBundlePage.GetColorFromColorIndex(bundleColor))
				{
					motion = new Vector2(1f, 0.5f),
					acceleration = new Vector2(0f, 0.02f),
					flipped = true,
					delayBeforeAnimationStart = 50
				});

				EndgameBundleMenu.tempSprites.Last().sourceRect.Y++;
				EndgameBundleMenu.tempSprites.Last().sourceRect.Height--;
			}
		}

		public void ShakeAndAllowClicking(int extraInfo)
		{
			maxShake = (float)Math.PI * 3f / 128f;
			EndgameBundleMenu.canClick = true;
		}

		public void TryHoverAction(int x, int y)
		{
			if (bounds.Contains(x, y) && !complete)
			{
				sprite.paused = false;
				EndgameBundleMenu.hoverText = Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", label);
			}
			else if (!complete)
			{
				sprite.reset();
				sprite.sourceRect.X += sprite.sourceRect.Width;
				sprite.paused = true;
			}
		}

		public bool IsValidItemForTheBundle(Item? item, EndgameBundleIngredient ingredient)
		{
			// For now don't allow other than objects
			if (item is not SObject obj)
			{
				return false;
			}

			if (!ingredient.Completed && ingredient.Quality <= obj.Quality)
			{
				if (ingredient.ItemID < 0)
				{
					if (item.Category == ingredient.ItemID)
					{
						return true;
					}
				}
				else if (ingredient.ItemID == item.ParentSheetIndex)
				{
					return true;
				}
			}

			return false;
		}

		public int GetBundleIngredientIndexForItem(Item item)
		{
			if (item is SObject o)
			{
				for (int i = 0; i < bundle.Ingredients.Count; i++)
				{
					if (IsValidItemForTheBundle(o, bundle.Ingredients[i]))
					{
						return i;
					}
				}

				return -1;
			}

			return -1;
		}

		public bool CanAcceptThisItem(Item? item, ClickableTextureComponent slot)
		{
			return CanAcceptThisItem(item, slot, ignore_stack_count: false);
		}

		public bool CanAcceptThisItem(Item? item, ClickableTextureComponent slot, bool ignore_stack_count = false)
		{
			if (item is SObject o)
			{
				for (int i = 0; i < bundle.Ingredients.Count; i++)
				{
					if (IsValidItemForTheBundle(o, bundle.Ingredients[i]) && (ignore_stack_count || bundle.Ingredients[i].Amount <= item.Stack) && (slot is null || slot.item is null))
					{
						return true;
					}
				}

				return false;
			}

			return false;
		}

		public Item? TryToDepositThisItem(Item? item, ClickableTextureComponent slot, string noteTextureName)
		{
			if (item is not SObject || item is Furniture)
			{
				return item;
			}

			for (int i = 0; i < bundle.Ingredients.Count; i++)
			{
				if (IsValidItemForTheBundle(item, bundle.Ingredients[i]) && slot.item is null)
				{
					item.Stack -= bundle.Ingredients[i].Amount;
					bundle.Ingredients[i].Completed = true;
					IngredientDepositAnimation(slot, noteTextureName);
					int index = bundle.Ingredients[i].ItemID;
					
					if (index < 0)
					{
						index = EndgameBundleMenu.GetObjectOrCategoryIndex(index);
					}

					slot.item = new SObject(index, bundle.Ingredients[i].ItemID, isRecipe: false, -1, bundle.Ingredients[i].Quality);
					Game1.playSound("newArtifact");

					// TODO: bundle logic
					//(Game1.getLocationFromName("CommunityCenter") as CommunityCenter)!.bundles.FieldDict[bundleIndex][i] = true;
					//ModEntry.bundles[bundleIndex] = true;

					slot.sourceRect.X = 512;
					slot.sourceRect.Y = 244;
					//Game1.multiplayer.globalChatInfoMessage("BundleDonate", Game1.player.displayName, slot.item.DisplayName);
					break;
				}
			}

			if (item.Stack > 0)
			{
				return item;
			}

			return null;
		}

		public bool CouldThisItemBeDeposited(Item item)
		{
			if (item is not SObject || item is Furniture)
			{
				return false;
			}

			for (int i = 0; i < bundle.Ingredients.Count; i++)
			{
				if (!IsValidItemForTheBundle(item, bundle.Ingredients[i]))
				{
					return true;
				}
			}

			return false;
		}

		public void IngredientDepositAnimation(ClickableTextureComponent slot, string noteTextureName, bool skipAnimation = false)
		{
			TemporaryAnimatedSprite t = new TemporaryAnimatedSprite(noteTextureName, new Rectangle(530, 244, 18, 18), 50f, 6, 1, new Vector2(slot.bounds.X, slot.bounds.Y), flicker: false, flipped: false, 0.88f, 0f, Color.White, 4f, 0f, 0f, 0f, local: true)
			{
				holdLastFrame = true,
				endSound = "cowboy_monsterhit"
			};

			if (skipAnimation)
			{
				t.sourceRect.Offset(t.sourceRect.Width * 5, 0);
				t.sourceRectStartingPos = new Vector2(t.sourceRect.X, t.sourceRect.Y);
				t.animationLength = 1;
			}

			EndgameBundleMenu.tempSprites.Add(t);
		}

		public bool CanBeClicked()
		{
			return !complete;
		}

		public void CompletionAnimation(EndgameBundleMenu menu, bool playSound = true, int delay = 0)
		{
			if (delay <= 0)
			{
				CompletionAnimation(playSound);
			}
			else
			{
				completionTimer = delay;
			}
		}

		private void CompletionAnimation(bool playSound = true)
		{
			if (Game1.activeClickableMenu is not null && Game1.activeClickableMenu is EndgameBundleMenu bundleMenu)
			{
				bundleMenu.TakeDownBundleSpecificPage();
			}

			sprite.pingPong = false;
			sprite.paused = false;
			sprite.sourceRect.X = (int)sprite.sourceRectStartingPos.X;
			sprite.sourceRect.X += sprite.sourceRect.Width;
			sprite.animationLength = 15;
			sprite.interval = 50f;
			sprite.totalNumberOfLoops = 0;
			sprite.holdLastFrame = true;
			sprite.endFunction = Shake;
			sprite.extraInfoForEndBehavior = 1;

			if (complete)
			{
				sprite.sourceRect.X += sprite.sourceRect.Width * 14;
				sprite.sourceRectStartingPos = new Vector2(sprite.sourceRect.X, sprite.sourceRect.Y);
				sprite.currentParentTileIndex = 14;
				sprite.interval = 0f;
				sprite.animationLength = 1;
				sprite.extraInfoForEndBehavior = 0;
			}
			else
			{
				if (playSound)
				{
					Game1.playSound("dwop");
				}

				bounds.Inflate(64, 64);
				EndgameBundleMenu.tempSprites.AddRange(Utility.sparkleWithinArea(bounds, 8, EndgameBundlePage.GetColorFromColorIndex(bundleColor) * 0.5f));
				bounds.Inflate(-64, -64);
			}

			complete = true;
		}

		public void Update(GameTime time)
		{
			sprite.update(time);

			if (completionTimer > 0 && EndgameBundleMenu.screenSwipe is null)
			{
				completionTimer -= time.ElapsedGameTime.Milliseconds;

				if (completionTimer <= 0)
				{
					CompletionAnimation();
				}
			}

			if (Game1.random.NextDouble() < 0.005 && (complete || name.ToLower().Contains(Game1.currentSeason)))
			{
				Shake();
			}

			if (maxShake > 0f)
			{
				if (shakeLeft)
				{
					sprite.rotation -= (float)Math.PI / 200f;

					if (sprite.rotation <= 0f - maxShake)
					{
						shakeLeft = false;
					}
				}
				else
				{
					sprite.rotation += (float)Math.PI / 200f;

					if (sprite.rotation >= maxShake)
					{
						shakeLeft = true;
					}
				}
			}

			if (maxShake > 0f)
			{
				maxShake = Math.Max(0f, maxShake - 0.0007669904f);
			}
		}

		public void Draw(SpriteBatch b)
		{
			sprite.draw(b, localPosition: true);
		}

		public static Color GetColorFromColorIndex(int color)
		{
			return color switch
			{
				5 => Color.LightBlue,
				0 => Color.Lime,
				2 => Color.Orange,
				1 => Color.DeepPink,
				4 => Color.Red,
				6 => Color.Cyan,
				3 => Color.Orange,
				_ => Color.Lime,
			};
		}
	}
}
