using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BetterRanching
{
	public class BetterRanching : Mod
	{
		private FarmAnimal AnimalBeingRanched { get; set; }
		private ModConfig Config { get; set; }

		public override void Entry(IModHelper helper)
		{
			Config = helper.ReadConfig<ModConfig>();
			GameEvents.UpdateTick += Event_UpdateTick;
			GraphicsEvents.OnPreRenderHudEvent += Event_OnPreRenderHudEvent;
			ControlEvents.MouseChanged += Event_MouseChanged;
			ControlEvents.ControllerButtonPressed += Event_ControllerButtonPressed;
		}

		private void Event_UpdateTick(object sender, EventArgs e)
		{
			//Override auto-click on hold for milkpail
			if (Config.PreventHarvestRepeating && GameExtensions.HoldingOverridableTool() && GameExtensions.IsClickableArea() && Game1.mouseClickPolling > 50)
			{
				Game1.mouseClickPolling = 50;
			}

			if (!Game1.player.UsingTool && AnimalBeingRanched != null)
			{
				AnimalBeingRanched = null;
			}
		}

		private void Event_MouseChanged(object sender, EventArgsMouseStateChanged e)
		{
			if (!Game1.hasLoadedGame || !Game1.currentLocation.IsFarm)
			{
				return;
			}

			if (e.NewState.LeftButton == ButtonState.Pressed)
			{
				ActionTriggered(e);
			}
		}

		private void Event_ControllerButtonPressed(object sender, EventArgsControllerButtonPressed e)
		{
			if (!Game1.hasLoadedGame || !Game1.currentLocation.IsFarm)
			{
				return;
			}

			if (e.ButtonPressed == Buttons.X)
			{
				ActionTriggered(e);
			}
		}

		private void ActionTriggered(EventArgs e)
		{
			var who = Game1.player;

			if (Config.PreventFailedHarvesting && GameExtensions.HoldingOverridableTool() && GameExtensions.IsClickableArea())
			{
				if (e is EventArgsMouseStateChanged mouse)
				{
					Vector2 vector2 = (double)Game1.mouseCursorTransparency == 0.0 ? Game1.player.GetToolLocation(false) : new Vector2((float)(Game1.getOldMouseX() + @Game1.viewport.X), (float)(Game1.getOldMouseY() + Game1.viewport.Y));

					if (Utility.withinRadiusOfPlayer((int)vector2.X, (int)vector2.Y, 1, Game1.player)
						&& ((double)Math.Abs(vector2.X - (float)Game1.player.getStandingX()) >= (double)(Game1.tileSize / 2) || (double)Math.Abs(vector2.Y - (float)Game1.player.getStandingY()) >= (double)(Game1.tileSize / 2)))
					{
						if (who.CanMove && (double)Game1.mouseCursorTransparency != 0.0 && !Game1.isAnyGamePadButtonBeingHeld())
						{
							who.Halt();
							who.faceGeneralDirection(new Vector2((float)(int)vector2.X, (float)(int)vector2.Y), 0);
						}
					}
					OverrideRanching(Game1.currentLocation, (int)who.GetToolLocation().X, (int)who.GetToolLocation().Y, who, mouse.NewState, who.CurrentTool?.Name);
				}
				else if (e is EventArgsControllerButtonPressed)
				{
					OverrideRanching(Game1.currentLocation, (int)who.GetToolLocation().X, (int)who.GetToolLocation().Y, who
						, new GamePadState(Game1.oldPadState.ThumbSticks, Game1.oldPadState.Triggers, new GamePadButtons(Buttons.X), Game1.oldPadState.DPad)
						, who.CurrentTool?.Name);
				}
			}
		}

		private void OverrideRanching(GameLocation currentLocation, int x, int y, StardewValley.Farmer who, object state, string toolName)
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
				Game1.game1.OverwriteState(state, $"{ranchAction} Failed");
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
					Game1.game1.OverwriteState(state, "Inventory Full");
				}
			}
			else if (animal?.isBaby() == true && animal.toolUsedForHarvest.Equals(toolName))
			{
				Game1.game1.OverwriteState(state);
				DelayedAction.showDialogueAfterDelay($"Baby {animal.Name} will produce {ranchProduct} in {animal.ageWhenMature.Value - animal.age.Value} days.", 0);
			}
			else
			{
				Game1.game1.OverwriteState(state, $"{ranchAction} Failed");
			}
		}

		private void Event_OnPreRenderHudEvent(object sender, EventArgs e)
		{
			if (!Game1.hasLoadedGame || !Game1.currentLocation.IsFarm)
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
			Rectangle? sourceRectangle = new Rectangle?(new Rectangle(218, 428, 7, 6));

			if ((Config.DisplayProduce && hasProduce) || (Config.DisplayHearts && !animal.wasPet.Value))
			{
				float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2));
				if (animal.isCoopDweller() && !animal.isBaby()) { num -= Game1.tileSize * 1 / 2; }

				// Thought Bubble
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(animal.Position.X + (animal.Sprite.getWidth() / 2)),
					(float)(animal.Position.Y - (Game1.tileSize * 4 / 3)) + num)),
					new Microsoft.Xna.Framework.Rectangle?(new Rectangle(141, 465, 20, 24)),
					Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None,
					0);

				if (Config.DisplayHearts && !animal.wasPet.Value)
				{
					if (Config.DisplayProduce && hasProduce)
					{
						// Small Heart
						spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(animal.Position.X + (animal.Sprite.getWidth() / 2) + (Game1.tileSize * .65)),
						   (float)(animal.Position.Y - (Game1.tileSize * 4 / 10)) + num)),
							sourceRectangle,
							Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), (float)Game1.pixelZoom, SpriteEffects.None,
							1);
					}
					else
					{
						// Big Heart
						spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(animal.Position.X + (animal.Sprite.getWidth() / 2) + (Game1.tileSize * 1.1)),
						   (float)(animal.Position.Y - (Game1.tileSize * 1 / 10)) + num)),
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
						   (float)(animal.Position.Y - (Game1.tileSize * 7 / 10)) + num)),
							new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, animal.currentProduce.Value, 16, 16)),
							Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), (float)(Game1.pixelZoom * .60), SpriteEffects.None,
							1);
					}
					else
					{
						// Big Milk
						spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(animal.Position.X + (animal.Sprite.getWidth() / 2) + (Game1.tileSize * .625)),
						   (float)(animal.Position.Y - (Game1.tileSize * 7 / 10)) + num)),
							new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, animal.currentProduce.Value, 16, 16)),
							Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), (float)(Game1.pixelZoom), SpriteEffects.None,
							1);
					}
				}
			}
		}

		public void DrawItemBubble(SpriteBatch spriteBatch, Pet pet)
		{
			Rectangle? sourceRectangle = new Rectangle?(new Rectangle(218, 428, 7, 6));
			bool wasPet = (bool)typeof(Pet).GetField("wasPetToday", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(pet);
			if (!wasPet)
			{
				float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2)) - (Game1.tileSize * 1 / 2);

				// Thought Bubble
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(pet.Position.X + (pet.Sprite.getWidth() / 2)),
					(float)(pet.Position.Y - (Game1.tileSize * 4 / 3)) + num)),
					new Microsoft.Xna.Framework.Rectangle?(new Rectangle(141, 465, 20, 24)),
					Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None,
					0);

				// Big Heart
				spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(pet.Position.X + (pet.Sprite.getWidth() / 2) + (Game1.tileSize * 1.1)),
				   (float)(pet.Position.Y - (Game1.tileSize * 1 / 10)) + num)),
					sourceRectangle,
					Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), (float)Game1.pixelZoom * 5 / 3, SpriteEffects.None,
					1);
			}
		}
	}
}
