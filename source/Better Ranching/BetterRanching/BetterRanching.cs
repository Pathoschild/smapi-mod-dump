using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace BetterRanching
{
	/// <summary>The mod entry class loaded by SMAPI.</summary>
	public class BetterRanching : Mod
	{
		private FarmAnimal AnimalBeingRanched { get; set; }
		private ModConfig Config { get; set; }

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Config = helper.ReadConfig<ModConfig>();
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.Display.RenderingHud += OnRenderingHud;
			helper.Events.Input.ButtonPressed += OnButtonPressed;
		}

		/// <summary>Raised after the game state is updated (≈60 times per second).</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			//Override auto-click on hold for milk pail
			if (Config.PreventHarvestRepeating && GameExtensions.HoldingOverridableTool() && GameExtensions.IsClickableArea() && Game1.mouseClickPolling > 50)
			{
				Game1.mouseClickPolling = 50;
			}

			if (!Game1.player.UsingTool && AnimalBeingRanched != null)
			{
				AnimalBeingRanched = null;
			}
		}

		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (!Context.IsWorldReady || !Game1.currentLocation.IsFarm)
			{
				return;
			}

			if (e.Button.IsUseToolButton() && Config.PreventFailedHarvesting && GameExtensions.HoldingOverridableTool() && GameExtensions.IsClickableArea())
			{
				Farmer who = Game1.player;

				if (e.Button == SButton.MouseLeft)
				{
					Vector2 pos = Game1.mouseCursorTransparency == 0.0 ? Game1.player.GetToolLocation() : new Vector2(Game1.getOldMouseX() + @Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y);

					if (Utility.withinRadiusOfPlayer((int)pos.X, (int)pos.Y, 1, Game1.player)
						&& (Math.Abs(pos.X - Game1.player.getStandingX()) >= (double)(Game1.tileSize / 2) || Math.Abs(pos.Y - Game1.player.getStandingY()) >= (double)(Game1.tileSize / 2)))
					{
						if (who.CanMove && Game1.mouseCursorTransparency != 0.0 && !Game1.isAnyGamePadButtonBeingHeld())
						{
							who.Halt();
							who.faceGeneralDirection(new Vector2((int)pos.X, (int)pos.Y), 0);
						}
					}
					OverrideRanching(Game1.currentLocation, (int)who.GetToolLocation().X, (int)who.GetToolLocation().Y, who, e.Button, who.CurrentTool?.Name);
				}
				else
				{
					OverrideRanching(Game1.currentLocation, (int)who.GetToolLocation().X, (int)who.GetToolLocation().Y, who, e.Button, who.CurrentTool?.Name);
				}
			}
		}

		private void OverrideRanching(GameLocation currentLocation, int x, int y, Farmer who, SButton button, string toolName)
		{
			AnimalBeingRanched = null;
			FarmAnimal animal = null;
			string ranchAction = string.Empty;
			string ranchProduct = string.Empty;

			if (toolName == null)
			{
				return;
			}

			switch (toolName)
			{
				case GameConstants.Tools.MilkPail:
					ranchAction = "Milking";
					ranchProduct = "milk";
					break;
				case GameConstants.Tools.Shears:
					ranchAction = "Shearing";
					ranchProduct = "wool";
					break;
			}
			var rectangle = new Rectangle(x - (Game1.tileSize / 2), y - (Game1.tileSize / 2), Game1.tileSize, Game1.tileSize);

			if (currentLocation is AnimalHouse animalHouse)
			{
				animal = animalHouse.GetSelectedAnimal(rectangle);
			}
			else if (currentLocation.IsFarm && currentLocation.IsOutdoors)
			{
				animal = ((Farm)currentLocation).GetSelectedAnimal(rectangle);
			}

			if (animal == null)
			{
				this.Helper.Input.OverwriteState(button, $"{ranchAction} Failed");
				return;
			}

			if (animal.CanBeRanched(toolName))
			{
				if (who.couldInventoryAcceptThisObject(animal.currentProduce.Value, 1, 0))
				{
					AnimalBeingRanched = animal;
					return;
				}
				else
				{
					this.Helper.Input.OverwriteState(button, "Inventory Full");
				}
			}
			else if (animal?.isBaby() == true && animal.toolUsedForHarvest.Equals(toolName))
			{
				this.Helper.Input.OverwriteState(button);
				DelayedAction.showDialogueAfterDelay($"Baby {animal.Name} will produce {ranchProduct} in {animal.ageWhenMature.Value - animal.age.Value} days.", 0);
			}
			else
			{
				this.Helper.Input.OverwriteState(button, $"{ranchAction} Failed");
			}
		}

		/// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnRenderingHud(object sender, RenderingHudEventArgs e)
		{
			if (!Context.IsWorldReady || !Game1.currentLocation.IsFarm)
			{
				return;
			}

			GameLocation currentLocation = Game1.currentLocation;

			List<FarmAnimal> farmAnimalList = new List<FarmAnimal>();
			if (currentLocation is AnimalHouse animalHouse)
			{
				farmAnimalList = animalHouse.animals.Values.ToList();
			}
			else if (currentLocation is Farm farm)
			{
				farmAnimalList = farm.animals.Values.ToList();
			}

			foreach (FarmAnimal farmAnimal in farmAnimalList)
			{
				DrawItemBubble(Game1.spriteBatch, farmAnimal);
			}

			foreach (NPC npc in currentLocation.characters)
			{
				if (npc is Pet pet)
				{
					DrawItemBubble(Game1.spriteBatch, pet);
				}
			}
		}

		public void DrawItemBubble(SpriteBatch spriteBatch, FarmAnimal animal)
		{
			bool hasProduce = AnimalBeingRanched != animal && (animal.CanBeRanched(GameConstants.Tools.MilkPail) || animal.CanBeRanched(GameConstants.Tools.Shears));
			Rectangle? sourceRectangle = new Rectangle(218, 428, 7, 6);

			if ((Config.DisplayProduce && hasProduce) || (Config.DisplayHearts && !animal.wasPet.Value))
			{
				float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2));
				if (animal.isCoopDweller() && !animal.isBaby()) { num -= Game1.tileSize * 1 / 2; }

				// Thought Bubble
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(animal.Position.X + (animal.Sprite.getWidth() / 2),
					animal.Position.Y - (Game1.tileSize * 4 / 3) + num)),
					new Rectangle(141, 465, 20, 24),
					Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None,
					0);

				if (Config.DisplayHearts && !animal.wasPet.Value)
				{
					if (Config.DisplayProduce && hasProduce)
					{
						// Small Heart
						spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(animal.Position.X + (animal.Sprite.getWidth() / 2) + (Game1.tileSize * .65)),
						   animal.Position.Y - (Game1.tileSize * 4 / 10) + num)),
							sourceRectangle,
							Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), Game1.pixelZoom, SpriteEffects.None,
							1);
					}
					else
					{
						// Big Heart
						spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(animal.Position.X + (animal.Sprite.getWidth() / 2) + (Game1.tileSize * 1.1)),
						   animal.Position.Y - (Game1.tileSize * 1 / 10) + num)),
							sourceRectangle,
							Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), (float)Game1.pixelZoom * 5 / 3, SpriteEffects.None,
							1);
					}
				}

				if (Config.DisplayProduce && hasProduce)
				{
					if (Config.DisplayHearts && !animal.wasPet.Value)
					{
						// Small Milk
						spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(animal.Position.X + (animal.Sprite.getWidth() / 2) + (Game1.tileSize * .85)),
						   animal.Position.Y - (Game1.tileSize * 7 / 10) + num)),
							Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, animal.currentProduce.Value, 16, 16),
							Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), (float)(Game1.pixelZoom * .60), SpriteEffects.None,
							1);
					}
					else
					{
						// Big Milk
						spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(animal.Position.X + (animal.Sprite.getWidth() / 2) + (Game1.tileSize * .625)),
						   animal.Position.Y - (Game1.tileSize * 7 / 10) + num)),
							Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, animal.currentProduce.Value, 16, 16),
							Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), Game1.pixelZoom, SpriteEffects.None,
							1);
					}
				}
			}
		}

		public void DrawItemBubble(SpriteBatch spriteBatch, Pet pet)
		{
			Rectangle? sourceRectangle = new Rectangle(218, 428, 7, 6);
			bool wasPet = (bool)typeof(Pet).GetField("wasPetToday", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(pet);
			if (!wasPet)
			{
				float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2)) - (Game1.tileSize * 1 / 2);

				// Thought Bubble
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(pet.Position.X + (pet.Sprite.getWidth() / 2),
					pet.Position.Y - (Game1.tileSize * 4 / 3) + num)),
					new Rectangle(141, 465, 20, 24),
					Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None,
					0);

				// Big Heart
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(pet.Position.X + (pet.Sprite.getWidth() / 2) + (Game1.tileSize * 1.1)),
				   pet.Position.Y - (Game1.tileSize * 1 / 10) + num)),
					sourceRectangle,
					Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), (float)Game1.pixelZoom * 5 / 3, SpriteEffects.None,
					1);
			}
		}
	}
}
