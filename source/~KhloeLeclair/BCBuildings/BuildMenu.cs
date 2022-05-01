/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;

#if IS_BETTER_CRAFTING
using Leclair.Stardew.Common.Crafting;
#else
using Leclair.Stardew.BetterCrafting;
#endif

using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Leclair.Stardew.BCBuildings;

internal class BuildMenu : IClickableMenu {

	public readonly ModEntry Mod;
	public readonly BluePrint? Blueprint;
	public readonly ActionType Action;
	public readonly BuildableGameLocation? Location;
	public readonly IPerformCraftEvent Event;

	public readonly ClickableTextureComponent btnCancel;

	private BluePrint? DemolishCheckBlueprint = null;
	private Building? MovingBuilding = null;

	private readonly string Message;

	private bool frozen = false;
	private bool checkingDemolish = false;

	public BuildMenu(BluePrint? bp, ActionType action, IPerformCraftEvent evt, ModEntry mod) : base() {
		Mod = mod;
		Action = action;
		Blueprint = bp;
		Event = evt;

		Location = Game1.currentLocation as BuildableGameLocation;

		Game1.displayHUD = false;
		Game1.viewportFreeze = true;
		Game1.panScreen(0, 0);

		btnCancel = new ClickableTextureComponent(
			"OK",
			new Rectangle(
				Game1.uiViewport.Width - 128,
				Game1.uiViewport.Height - 128,
				64, 64
			),
			null,
			null,
			Game1.mouseCursors,
			Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47),
			1f
		);

		if (Action == ActionType.Upgrade && Blueprint != null)
			Message = Game1.content.LoadString(@"Strings\UI:Carpenter_SelectBuilding_Upgrade", new BluePrint(Blueprint.nameOfBuildingToUpgrade).displayName);
		else if (Action == ActionType.Demolish)
			Message = Game1.content.LoadString(@"Strings\UI:Carpenter_SelectBuilding_Demolish");
		else if (Action == ActionType.Paint)
			Message = Game1.content.LoadString(@"Strings\UI:Carpenter_SelectBuilding_Paint");
		else
			Message = Game1.content.LoadString(@"Strings\UI:Carpenter_ChooseLocation");
	}

	protected override void cleanupBeforeExit() {
		base.cleanupBeforeExit();

		if (Location is not null)
			foreach (Building building in Location.buildings)
				building.color.Value = Color.White;

		Game1.displayHUD = true;
		Game1.viewportFreeze = false;
		Game1.displayFarmer = true;
	}

	public override bool shouldClampGamePadCursor() {
		return true;
	}

	public static bool CanPaintHouse() {
		return Game1.MasterPlayer.HouseUpgradeLevel >= 2;
	}

	public bool CanPaint(Building? building) {
		if (Location == null)
			return false;

		// null building means farmhouse
		if (building == null) {
			if (!CanPaintHouse())
				return false;

			if (Game1.player.UniqueMultiplayerID == Game1.MasterPlayer.UniqueMultiplayerID)
				return true;
			if (Game1.player.spouse == Game1.MasterPlayer.UniqueMultiplayerID.ToString())
				return true;
			return false;
		}

		if (!building.CanBePainted())
			return false;

		if (building.isCabin && building.indoors.Value is Cabin cabin) {
			Farmer who = cabin.owner;
			if (who == null)
				return false;
			if (Game1.player.UniqueMultiplayerID == who.UniqueMultiplayerID)
				return true;
			if (Game1.player.spouse == who.UniqueMultiplayerID.ToString())
				return true;
			return false;
		}

		return true;
	}

	public bool CanMove(Building building) {
		if (building == null || Location == null)
			return false;

		if (Location is Farm farm && building is GreenhouseBuilding && !farm.greenhouseUnlocked.Value)
			return false;

		if (Game1.IsMasterGame)
			return true;

		if (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.On)
			return true;

		if (Game1.player.team.farmhandsCanMoveBuildings.Value == FarmerTeam.RemoteBuildingPermissions.OwnedBuildings && building.hasCarpenterPermissions())
			return true;

		return false;
	}

	public bool CanDemolish(Building building) {
		if (building == null)
			return false;

		if (DemolishCheckBlueprint == null || DemolishCheckBlueprint.name != building.buildingType.Value)
			DemolishCheckBlueprint = new BluePrint(building.buildingType.Value);

		if (DemolishCheckBlueprint != null)
			return CanDemolish(DemolishCheckBlueprint);

		return true;
	}

	public bool CanDemolish(BluePrint blueprint) {
		if (blueprint == null || Location == null)
			return false;

		if (blueprint.moneyRequired < 0)
			return false;

		if (blueprint.name == "Shipping Bin") {
			int bins = 0;
			foreach(Building bld in Location.buildings) {
				if (bld is ShippingBin)
					bins++;
				if (bins > 1)
					break;
			}

			if (bins <= 1)
				return false;
		}

		return true;
	}

	public override void update(GameTime time) {
		base.update(time);

		if (frozen)
			return;

		int mouseX = Game1.getOldMouseX(ui_scale: false) + Game1.viewport.X;
		int mouseY = Game1.getOldMouseY(ui_scale: false) + Game1.viewport.Y;

		if (mouseX - Game1.viewport.X < 64) {
			Game1.panScreen(-8, 0);
		} else if (mouseX - (Game1.viewport.X + Game1.viewport.Width) >= -128) {
			Game1.panScreen(8, 0);
		}

		if (mouseY - Game1.viewport.Y < 64) {
			Game1.panScreen(0, -8);
		} else if (mouseY - (Game1.viewport.Y + Game1.viewport.Height) >= -64) {
			Game1.panScreen(0, 8);
		}

		Keys[] pressedKeys = Game1.oldKBState.GetPressedKeys();
		foreach (Keys key in pressedKeys)
			if (!Game1.options.doesInputListContain(Game1.options.menuButton, key))
				receiveKeyPress(key);

		checkingDemolish = false;

		if (Game1.IsMultiplayer)
			return;

		if (Location is Farm farm)
			foreach (FarmAnimal value in farm.animals.Values)
				value.MovePosition(Game1.currentGameTime, Game1.viewport, farm);
	}

	public override void performHoverAction(int x, int y) {
		btnCancel.tryHover(x, y);
		base.performHoverAction(x, y);

		if (frozen || Action == ActionType.Build || Location == null)
			return;

		Vector2 mouseTile = new(
			(Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64,
			(Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64
		);

		if (Action == ActionType.Paint && Location is Farm farm && farm.GetHouseRect().Contains(Utility.Vector2ToPoint(mouseTile)) && CanPaint(null))
			farm.frameHouseColor = Color.Lime;

		foreach (Building building in Location.buildings)
			building.color.Value = Color.White;

		Building bld = Location.getBuildingAt(mouseTile);
		if (bld == null) {
			mouseTile.Y++;
			bld = Location.getBuildingAt(mouseTile);
			if (bld == null) {
				mouseTile.Y++;
				bld = Location.getBuildingAt(mouseTile);
			}
		}

		if (bld == null)
			return;

		if (Action == ActionType.Upgrade && Blueprint != null) {
			if (Blueprint.nameOfBuildingToUpgrade == bld.buildingType.Value)
				bld.color.Value = Color.Lime * 0.8f;
			else
				bld.color.Value = Color.Red * 0.8f;
		}

		if (Action == ActionType.Move && CanMove(bld))
			bld.color.Value = Color.Lime * 0.8f;

		if (Action == ActionType.Demolish && CanDemolish(bld))
			bld.color.Value = Color.Red * 0.8f;

		if (Action == ActionType.Paint && CanPaint(bld))
			bld.color.Value = Color.Lime * 0.8f;
	}

	public override bool overrideSnappyMenuCursorMovementBan() {
		return true;
	}

	public override bool readyToClose() {
		if (MovingBuilding != null || checkingDemolish)
			return false;

		return base.readyToClose();
	}

	public override void receiveKeyPress(Keys key) {
		if (frozen)
			return;

		if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose()) {
			exitThisMenu();
			Game1.player.forceCanMove();
			return;
		}

		if (!Game1.options.SnappyMenus) {
			if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key)) {
				Game1.panScreen(0, 4);
			} else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key)) {
				Game1.panScreen(4, 0);
			} else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key)) {
				Game1.panScreen(0, -4);
			} else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key)) {
				Game1.panScreen(-4, 0);
			}
		}
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true) {
		if (frozen)
			return;

		if (btnCancel.containsPoint(x, y)) {
			if (!readyToClose()) {
				Game1.playSound("cancel");
				return;
			}

			Game1.playSound("smallSelect");
			exitThisMenu();
			Game1.player.forceCanMove();
			return;
		}

		if (Location is null)
			return;

		Vector2 mouseTile = new(
			(Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64,
			(Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64
		);

		if (Action == ActionType.Build && Blueprint != null)
			Game1.player.team.buildLock.RequestLock(delegate {
				if (!Location.buildStructure(
					Blueprint,
					mouseTile,
					Game1.player,
					true
				)) {
					Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(@"Strings\UI:Carpenter_CantBuild"), Color.Red, 3500f));
					return;
				}

				Building building = Location.buildings.Last();

				int day = Game1.dayOfMonth;
				while(Utility.isFestivalDay(day, Game1.currentSeason))
					day++;

				// We use dayUpdate to finish the construction so that any
				// custom behavior gets called.
				building.daysOfConstructionLeft.Value = 1;
				building.dayUpdate(day);

				frozen = true;

				DelayedAction.functionAfterDelay(delegate {
					Event.Complete();
					exitThisMenu();
					Game1.player.forceCanMove();
				}, 2000);
			});

		if (Action == ActionType.Upgrade && Blueprint != null) {
			Building? building = Location.getBuildingAt(mouseTile);
			if (building != null) {
				if (building.buildingType.Value == Blueprint.nameOfBuildingToUpgrade) {

					building.showUpgradeAnimation(Location);
					Game1.playSound("axe");

					int day = Game1.dayOfMonth;
					while (Utility.isFestivalDay(day, Game1.currentSeason))
						day++;

					// We use dayUpdate to finish the construction so that any
					// custom behavior gets called.
					building.daysUntilUpgrade.Value = 1;
					building.dayUpdate(day);

					frozen = true;

					DelayedAction.functionAfterDelay(delegate {
						Event.Complete();
						exitThisMenu();
						Game1.player.forceCanMove();
					}, 2000);

				} else {
					Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(@"Strings\UI:Carpenter_CantUpgrade_BuildingType"), Color.Red, 3500f));
				}
			}
		}

		if (Action == ActionType.Demolish) {
			Building? building = Location.getBuildingAt(mouseTile);
			if (building != null) {
				Cabin? cabin = building.indoors.Value as Cabin;
				if (cabin != null && !Game1.IsMasterGame) { 
					Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(@"Strings\UI:Carpenter_CantDemolish_LockFailed"), Color.Red, 3500f));
					return;
				}

				if (!CanDemolish(building))
					return;

				if (building.daysOfConstructionLeft.Value > 0 || building.daysUntilUpgrade.Value > 0) {
					Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(@"Strings\UI:Carpenter_CantDemolish_DuringConstruction"), Color.Red, 3500f));
					return;
				}

				void lockFailed() {
					Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(@"Strings\UI:Carpenter_CantDemolish_LockFailed"), Color.Red, 3500f));
				}

				void continueDemolish() {
					if (Location == null || !Location.buildings.Contains(building))
						return;

					if (building.indoors.Value is AnimalHouse house && house.animalsThatLiveHere.Count > 0) {
						Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(@"Strings\UI:Carpenter_CantDemolish_AnimalsHere"), Color.Red, 3500f));
						return;
					}

					if (building.indoors.Value != null && building.indoors.Value.farmers.Any()) {
						Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(@"Strings\UI:Carpenter_CantDemolish_PlayerHere"), Color.Red, 3500f));
						return;
					}

					if (building.indoors.Value is Cabin cabin) {
						string name = cabin.GetCellarName();
						foreach (Farmer who in Game1.getAllFarmers()) {
							if (who.currentLocation != null && who.currentLocation.Name == name) {
								Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(@"Strings\UI:Carpenter_CantDemolish_PlayerHere"), Color.Red, 3500f));
								return;
							}
						}

						if (cabin.farmhand.Value.isActive()) {
							Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(@"Strings\UI:Carpenter_CantDemolish_FarmhandOnline"), Color.Red, 3500f));
							return;
						}
					}

					building.BeforeDemolish();
					Chest? chest = null;

					if (building.indoors.Value is Cabin cbn) {
						List<Item> list = cbn.demolish();
						if (list.Count > 0) {
							chest = new Chest(playerChest: true);
							chest.fixLidFrame();
							chest.items.Set(list);
						}
					}

					if (Location.destroyStructure(building)) {
						Game1.flashAlpha = 1f;
						building.showDestroyedAnimation(Location);
						Game1.playSound("explosion");
						if (Location is Farm farm)
							Utility.spreadAnimalsAround(building, farm);

						frozen = true;

						if (chest != null) {
							Location.objects[new Vector2(
								building.tileX.Value + building.tilesWide.Value / 2,
								building.tileY.Value + building.tilesHigh.Value / 2
							)] = chest;
						}

						// TODO: Config option to return materials to the player?
						/*if (DemolishCheckBlueprint == null || DemolishCheckBlueprint.name != building.buildingType.Value)
							DemolishCheckBlueprint = new BluePrint(building.buildingType.Value);

						if (DemolishCheckBlueprint != null) {
							foreach (var entry in DemolishCheckBlueprint.itemsRequired) {
								Game1.player.addItemToInventory(new StardewValley.Object(entry.Key, entry.Value));
							}
						}*/

						DelayedAction.functionAfterDelay(delegate {
							Event.Complete();
							exitThisMenu();
							Game1.player.forceCanMove();
						}, 2000);
					}
				}

				if (cabin != null && cabin.farmhand.Value.isCustomized.Value) {
					checkingDemolish = true;
					Game1.currentLocation.createQuestionDialogue(
						Game1.content.LoadString(@"Strings\UI:Carpenter_DemolishCabinConfirm", cabin.farmhand.Value.Name),
						Game1.currentLocation.createYesNoResponses(),
						delegate(Farmer who, string answer) {
							Game1.activeClickableMenu = this;
							if (answer == "Yes")
								Game1.player.team.demolishLock.RequestLock(continueDemolish, lockFailed);
						}
					);

					return;
				}

				Game1.player.team.demolishLock.RequestLock(continueDemolish, lockFailed);
			}
		}

		if (Action == ActionType.Move) {
			if (MovingBuilding == null) {
				MovingBuilding = Location.getBuildingAt(mouseTile);
				if (MovingBuilding != null) {
					if (MovingBuilding.daysOfConstructionLeft.Value > 0) {
						MovingBuilding = null;
						return;
					}

					if (!CanMove(MovingBuilding)) {
						MovingBuilding = null;
						return;
					}

					MovingBuilding.isMoving = true;
					Game1.playSound("axchop");
				}

			} else if (Location.buildStructure(MovingBuilding, mouseTile, Game1.player)) {
				MovingBuilding.isMoving = false;
				if (MovingBuilding is ShippingBin bin)
					bin.initLid();
				if (MovingBuilding is GreenhouseBuilding green && Location is Farm farm)
					farm.greenhouseMoved.Value = true;

				MovingBuilding.performActionOnBuildingPlacement();
				MovingBuilding = null;
				Game1.playSound("axchop");

				DelayedAction.playSoundAfterDelay("dirtyHit", 50);
				DelayedAction.playSoundAfterDelay("dirtyHit", 150);

			} else
				Game1.playSound("cancel");
		}

		if (Action == ActionType.Paint) {
			Building bld = Location.getBuildingAt(mouseTile);
			if (bld == null) {
				mouseTile.Y++;
				bld = Location.getBuildingAt(mouseTile);
				if (bld == null) {
					mouseTile.Y++;
					bld = Location.getBuildingAt(mouseTile);
				}
			}

			if (bld != null) {
				if (!bld.CanBePainted()) {
					Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(@"Strings\UI:Carpenter_CannotPaint"), Color.Red, 3500f));
					return;
				}

				if (!CanPaint(bld)) {
					Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(@"Strings\UI:Carpenter_CannotPaint_Permission"), Color.Red, 3500f));
					return;
				}

				bld.color.Value = Color.White;
				SetChildMenu(new BuildingPaintMenu(bld));
				return;

			} else if (Location is Farm farm && farm.GetHouseRect().Contains(Utility.Vector2ToPoint(mouseTile))) {
				// Check Farmhouse
				if (!CanPaintHouse()) {
					Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(@"Strings\UI:Carpenter_CannotPaint"), Color.Red, 3500f));
					return;
				}

				if (!CanPaint(null)) {
					Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString(@"Strings\UI:Carpenter_CannotPaint_Permission"), Color.Red, 3500f));
					return;
				}

				SetChildMenu(new BuildingPaintMenu(
					"House",
					() => farm.paintedHouseTexture ?? Farm.houseTextures,
					farm.houseSource.Value,
					farm.housePaintColor.Value
				));
			}
		}

	}

	public override void draw(SpriteBatch b) {
		if (frozen)
			return;

		Game1.StartWorldDrawInUI(b);

		Vector2 mouseTile = new(
			(Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64,
			(Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64
		);

		if (Action == ActionType.Build)
			DrawBuild(b, mouseTile);

		else if (Action == ActionType.Move)
			DrawMove(b, mouseTile);

		Game1.EndWorldDrawInUI(b);

		if (!string.IsNullOrEmpty(Message))
			SpriteText.drawStringWithScrollBackground(
				b,
				Message,
				Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(Message) / 2,
				16
			);

		btnCancel.draw(b);
		drawMouse(b);
	}

	public void DrawBuild(SpriteBatch b, Vector2 mouseTile) {
		if (Blueprint == null || Location is null)
			return;

		for (int y = 0; y < Blueprint.tilesHeight; y++) {
			for (int x = 0; x < Blueprint.tilesWidth; x++) {
				int idx = Blueprint.getTileSheetIndexForStructurePlacementTile(x, y);
				Vector2 tile = new(mouseTile.X + x, mouseTile.Y + y);
				if (!Location.isBuildable(tile))
					idx++;

				b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tile * 64f), new Rectangle(194 + idx * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
			}
		}
		foreach (Point extra in Blueprint.additionalPlacementTiles) {
			int x = extra.X;
			int y = extra.Y;
			int idx = Blueprint.getTileSheetIndexForStructurePlacementTile(x, y);
			Vector2 tile = new(mouseTile.X + x, mouseTile.Y + y);
			if (!Location.isBuildable(tile))
				idx++;
			b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tile * 64f), new Rectangle(194 + idx * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
		}
	}

	public void DrawMove(SpriteBatch b, Vector2 mouseTile) {
		if (MovingBuilding == null || Location is null)
			return;

		for (int y = 0; y < MovingBuilding.tilesHigh.Value; y++) {
			for (int x = 0; x < MovingBuilding.tilesWide.Value; x++) {
				int idx = MovingBuilding.getTileSheetIndexForStructurePlacementTile(x, y);
				Vector2 tile = new(mouseTile.X + x, mouseTile.Y + y);
				bool occupying = Location.buildings.Contains(MovingBuilding) && MovingBuilding.occupiesTile(tile);
				if (!Location.isBuildable(tile) && !occupying)
					idx++;

				b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tile * 64f), new Rectangle(194 + idx * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
			}
		}
		foreach (Point extra in MovingBuilding.additionalPlacementTiles) {
			int x = extra.X;
			int y = extra.Y;
			int idx = MovingBuilding.getTileSheetIndexForStructurePlacementTile(x, y);
			Vector2 tile = new(mouseTile.X + x, mouseTile.Y + y);
			bool occupying = Location.buildings.Contains(MovingBuilding) && MovingBuilding.occupiesTile(tile);
			if (!Location.isBuildable(tile) && !occupying)
				idx++;
			b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, tile * 64f), new Rectangle(194 + idx * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
		}
	}
}
