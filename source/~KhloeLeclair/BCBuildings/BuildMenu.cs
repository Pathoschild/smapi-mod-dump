/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leclair.Stardew.BetterCrafting;

using StardewValley;
using StardewValley.GameData.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using StardewValley.Buildings;
using Leclair.Stardew.Common;
using StardewValley.Locations;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Net.Sockets;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;

namespace Leclair.Stardew.BCBuildings;

public class BuildMenu : IClickableMenu {

	public static readonly Color COLOR_OKAY = Color.Lime * 0.8f;
	public static readonly Color COLOR_ERROR = Color.Red * 0.8f;


	// Basics

	public readonly ModEntry Mod;

	public readonly string? BuildingId;
	public readonly string? SkinId;
	public readonly BuildingData? Data;
	public readonly ActionType Action;

	public readonly GameLocation? Location;
	public readonly IPerformCraftEvent Event;

	// UI Stuff

	public readonly ClickableTextureComponent btnCancel;
	public readonly string Message;

	// State

	private Building? BuildingToBuild = null;

	private Building? MovingBuilding = null;

	private bool frozen = false;
	private bool checkingDemolish = false;

	#region Life Cycle

	public BuildMenu(ModEntry mod, ActionType action, string? buildingId, string? skinId, BuildingData? building, IPerformCraftEvent evt) : base() {
		Mod = mod;
		Action = action;
		BuildingId = buildingId;
		SkinId = skinId;
		Data = building;
		Event = evt;

		Location = Game1.currentLocation;

		// We need a building instance if we're trying to build.
		if (Action == ActionType.Build && BuildingId != null) {
			BuildingToBuild = Building.CreateInstanceFromId(BuildingId, Vector2.Zero);
			if (BuildingToBuild != null)
				BuildingToBuild.skinId.Value = SkinId;
		}

		// Ui Stuff
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


		// Message
		if (Action == ActionType.Upgrade && Data?.BuildingToUpgrade != null && DataLoader.Buildings(Game1.content).TryGetValue(Data.BuildingToUpgrade, out var target))
			Message = Game1.content.LoadString(@"Strings\UI:Carpenter_SelectBuilding_Upgrade", TokenParser.ParseText(target.Name));
		else if (Action == ActionType.Demolish)
			Message = Game1.content.LoadString(@"Strings\UI:Carpenter_SelectBuilding_Demolish");
		else if (Action == ActionType.Paint)
			Message = Game1.content.LoadString(@"Strings\UI:Carpenter_SelectBuilding_Paint");
		else
			Message = Game1.content.LoadString(@"Strings\UI:Carpenter_ChooseLocation");


		// Set up for building placement.
		Game1.displayHUD = false;
		Game1.viewportFreeze = true;
		Game1.clampViewportToGameMap();
		Game1.panScreen(0, 0);
	}

	public override bool readyToClose() {
		if (MovingBuilding is not null || checkingDemolish)
			return false;

		return base.readyToClose();
	}

	protected override void cleanupBeforeExit() {
		base.cleanupBeforeExit();

		if (Location is not null)
			foreach(var building in Location.buildings)
				building.color = Color.White;

		Game1.displayHUD = true;
		Game1.viewportFreeze = false;
		Game1.displayFarmer = true;
	}

	private void Close(bool complete = false) {
		if (complete)
			Event.Complete();
		exitThisMenu();
		Game1.player.forceCanMove();
	}

	private void SuccessClose() {
		Close(true);
	}

	#endregion

	#region Permission Checks

	public bool CanMove(Building building) {
		if (Location is null || building is null)
			return false;

		if (building is GreenhouseBuilding && !Game1.getFarm().greenhouseUnlocked.Value && !Mod.Config.AllowMovingUnfinishedGreenhouse)
			return false;

		if (Game1.IsMasterGame)
			return true;

		return Game1.player.team.farmhandsCanMoveBuildings.Value switch {
			FarmerTeam.RemoteBuildingPermissions.On => true,
			FarmerTeam.RemoteBuildingPermissions.OwnedBuildings => building.hasCarpenterPermissions(),
			_ => false
		};
	}

	public bool CanPaint(Building building) {
		if (Location is null || building is null)
			return false;

		if ( ! building.CanBePainted() && ! building.CanBeReskinned(ignoreSeparateConstructionEntries: true) )
			return false;

		if ((building.isCabin || building.HasIndoorsName("Farmhouse")) && building.GetIndoors() is FarmHouse house)
			return house.IsOwnedByCurrentPlayer || house.OwnerId.ToString() == Game1.player.spouse;

		return true;
	}

	public bool CanDemolish(Building building) {
		if (Location is null || building is null)
			return false;

		string type = building.buildingType.Value;

		switch (building.buildingType.Value) {
			case "Farmhouse":
				return ! building.HasIndoorsName("Farmhouse");

			case "Greenhouse":
				return ! building.HasIndoorsName("Greenhouse");

			case "Pet Bowl":
			case "Shipping Bin":
				return Location != Game1.getFarm() || Location.HasMinBuildings(type, 2);

			default:
				return true;
		}
	}

	public bool HasPermissionToDemolish(Building building) {
		return Game1.IsMasterGame;
	}

	#endregion

	#region Helper Methods

	private Vector2 GetMouseTile() {
		return new(
			(Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64,
			(Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64
		);
	}

	private Building? GetBuildingAt(Vector2 tile) {
		if (Location is null)
			return null;

		var building = Location.getBuildingAt(tile)
			?? Location.getBuildingAt(tile.Move(0, 1))
			?? Location.getBuildingAt(tile.Move(0, 2))
			?? Location.getBuildingAt(tile.Move(0, 3));

		var data = building?.GetData();
		if (data != null) {
			int height = data.SourceRect.IsEmpty ? building!.texture.Value.Height : data.SourceRect.Height;
			int extraHeight = (int) (height * (4 / 64.0) - building!.tilesHigh.Value);

			if (building.tileY.Value - extraHeight > tile.Y)
				return null;
		}

		return building;
	}

	#region Pathfinding Nonsense

	public bool ConfirmBuildingAccessibility(Vector2 pos, Building building) {
		if (Location is null)
			return false;

		if (building.buildingType.Value != "Farmhouse")
			return true;

		Point start = building.humanDoor.Value;
		start.X += (int) pos.X;
		start.Y += (int) pos.Y;
		start.Y++;

		HashSet<Point> closedTiles = new();
		Stack<Point> openTiles = new();

		openTiles.Push(start);
		closedTiles.Add(start);

		HashSet<Point> validWarpTiles = new();

		foreach (var warp in Location.warps) {
			if (warp.TargetName != "FarmCave")
				validWarpTiles.Add(new(warp.X, warp.Y));
		}

		bool success = false;
		while (openTiles.Count > 0) {
			var tile = openTiles.Pop();
			if (validWarpTiles.Contains(tile)) {
				success = true;
				break;
			}

			if (Location.isTileOnMap(tile.X, tile.Y) && VerifyTileAccessibility(tile.X, tile.Y, pos, building)) {
				Point newPoint = tile;
				newPoint.X++;
				if (closedTiles.Add(newPoint))
					openTiles.Push(newPoint);

				newPoint = tile;
				newPoint.X--;
				if (closedTiles.Add(newPoint))
					openTiles.Push(newPoint);

				newPoint = tile;
				newPoint.Y++;
				if (closedTiles.Add(newPoint))
					openTiles.Push(newPoint);

				newPoint = tile;
				newPoint.Y--;
				if (closedTiles.Add(newPoint))
					openTiles.Push(newPoint);
			}
		}

		return success;
	}

	public bool VerifyTileAccessibility(int x, int y, Vector2 pos, Building building) {
		Vector2 targetPos = new(x, y);
		if (Location is null || !Location.isTilePassable(targetPos.ToLocation(), Game1.viewport))
			return false;

		if (building != null) {
			int relativeX = x - (int) pos.X;
			int relativeY = y - (int) pos.Y;

			if (!building.isTilePassable(new Vector2(building.tileX.Value + relativeX, building.tileY.Value + relativeY)))
				return false;
		}

		Building? bld = Location.getBuildingAt(targetPos);
		if (bld != null && !bld.isMoving && !bld.isTilePassable(targetPos))
			return false;

		var rect = new Rectangle(x * 64, y * 64, 64, 64);
		rect.Inflate(-1, -1);
		foreach (var clump in Location.resourceClumps)
			if (clump.getBoundingBox().Intersects(rect))
				return false;

		if (Location.getLargeTerrainFeatureAt(x, y) != null)
			return false;

		return true;
	}

	#endregion

	#endregion

	#region Events

	public override bool shouldClampGamePadCursor() {
		return true;
	}

	public override bool overrideSnappyMenuCursorMovementBan() {
		return true;
	}

	public override void performHoverAction(int x, int y) {
		btnCancel.tryHover(x, y);
		base.performHoverAction(x, y);

		if (frozen || Action == ActionType.Build || Location is null)
			return;

		foreach (var bld in Location.buildings)
			bld.color = Color.White;

		Vector2 mouseTile = GetMouseTile();
		Building? building = GetBuildingAt(mouseTile);

		if (building is null || MovingBuilding is not null)
			return;

		if ( Action == ActionType.Upgrade && Data != null ) {
			if (Data.BuildingToUpgrade == building.buildingType.Value)
				building.color = COLOR_OKAY;
			else
				building.color = COLOR_ERROR;
		}

		if (Action == ActionType.Move && CanMove(building))
			building.color = COLOR_OKAY;

		if (Action == ActionType.Demolish && HasPermissionToDemolish(building) && CanDemolish(building))
			building.color = COLOR_ERROR;

		if (Action == ActionType.Paint && CanPaint(building))
			building.color = COLOR_OKAY;
	}

	public override void receiveKeyPress(Keys key) {
		if (frozen)
			return;

		if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose()) {
			Close();
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

			Game1.playSound("bigDeSelect");
			Close();
			return;
		}

		if (Location is null)
			return;

		Vector2 pos = GetMouseTile();
		Building? building = GetBuildingAt(pos);

		if (Action == ActionType.Demolish)
			HandleClickDemolish(building, playSound);

		else if (Action == ActionType.Upgrade)
			HandleClickUpgrade(building, playSound);

		else if (Action == ActionType.Move)
			HandleClickMove(pos, building, playSound);

		else if (Action == ActionType.Paint)
			HandleClickPaint(building, playSound);

		else if (Action == ActionType.Build)
			HandleClickBuild(pos, playSound);
	}

	public void HandleClickBuild(Vector2 pos, bool playSound) {
		if (Data is null || Location is null || BuildingId is null)
			return;

		void OnLocked() {
			if (!Location.buildStructure(BuildingId, pos, Game1.player, out var building, true)) {
				Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), 3));
				return;
			}

			building.skinId.Value = SkinId;
			building.FinishConstruction();

			frozen = true;
			DelayedAction.functionAfterDelay(SuccessClose, 2000);
		}

		Game1.player.team.buildLock.RequestLock(OnLocked);
	}

	public void HandleClickPaint(Building? building, bool playSound) {
		if (building is null)
			return;

		if ( ! building.CanBePainted() && !building.CanBeReskinned(ignoreSeparateConstructionEntries: true) ) {
			Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint"), 3));
			return;
		}

		if (! CanPaint(building) ) {
			Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CannotPaint_Permission"), 3));
			return;
		}

		building.color = Color.White;
		SetChildMenu(
			building.CanBePainted()
				? new BuildingPaintMenu(building)
				: new BuildingSkinMenu(building, ignoreSeparateConstructionEntries: true)
		);
	}

	public void HandleClickUpgrade(Building? building, bool playSound) {
		if (Location is null || building is null || Data is null)
			return;

		if (Data.BuildingToUpgrade != building.buildingType.Value) {
			Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantUpgrade_BuildingType"), 3));
			return;
		}

		building.upgradeName.Value = BuildingId;
		building.daysUntilUpgrade.Value = Math.Max(Data.BuildDays, 1);
		building.showUpgradeAnimation(Location);
		if (playSound)
			Game1.playSound("axe");

		building.FinishConstruction();

		frozen = true;
		DelayedAction.functionAfterDelay(SuccessClose, 500);
	}

	public void HandleClickMove(Vector2 pos, Building? building, bool playSound) {
		if (Location is null)
			return;

		if (MovingBuilding is null) {
			if (building is null)
				return;

			if (building.daysOfConstructionLeft.Value > 0)
				return;

			if (!CanMove(building))
				return;

			MovingBuilding = building;
			MovingBuilding.isMoving = true;
			if (playSound)
				Game1.playSound("axchop");
			return;
		}

		if (! ConfirmBuildingAccessibility(pos, MovingBuilding)) {
			Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantBuild"), 3));
			if (playSound)
				Game1.playSound("cancel");

		} else if (Location.buildStructure(MovingBuilding, pos, Game1.player)) {
			MovingBuilding.isMoving = false;
			MovingBuilding = null;
			if (playSound)
				Game1.playSound("axchop");
			DelayedAction.playSoundAfterDelay("dirtyHit", 50);
			DelayedAction.playSoundAfterDelay("dirtyHit", 150);

		} else if (playSound)
			Game1.playSound("cancel");
	}

	public void HandleClickDemolish(Building? building, bool playSound) {
		if (building is null || checkingDemolish)
			return;

		if (!CanDemolish(building) || !HasPermissionToDemolish(building))
			return;

		GameLocation? interior = building.GetIndoors();
		Cabin? cabin = interior as Cabin;

		if (cabin is not null && ! Game1.IsMasterGame) {
			Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), 3));
			return;
		}

		if (building.daysOfConstructionLeft.Value > 0 || building.daysUntilUpgrade.Value > 0) {
			Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_DuringConstruction"), 3));
			return;
		}

		void lockFailed() {
			checkingDemolish = false;
			Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_LockFailed"), 3));
		}

		void continueDemolish() {
			checkingDemolish = false;
			if (Location is null || !Location.buildings.Contains(building))
				return;

			if (interior is AnimalHouse house && house.animalsThatLiveHere.Count > 0) {
				Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_AnimalsHere"), 3));
				return;
			}

			if (interior != null && interior.farmers.Any()) {
				Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), 3));
				return;
			}

			if (cabin is not null) {
				string name = cabin.GetCellarName();
				foreach(var who in Game1.getAllFarmers()) {
					if (who.currentLocation != null && who.currentLocation.Name == name) {
						Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_PlayerHere"), 3));
						return;
					}
				}

				if (cabin.IsOwnerActivated) {
					Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Carpenter_CantDemolish_FarmhandOnline"), 3));
					return;
				}
			}

			building.BeforeDemolish();

			Chest? chest = null;
			if (cabin is not null) {
				var list = cabin.demolish();
				if (list.Count > 0) {
					chest = new Chest(playerChest: true);
					chest.fixLidFrame();
					chest.Items.OverwriteWith(list);
				}
			}

			if (Location.destroyStructure(building)) {
				Game1.flashAlpha = 1f;
				building.showDestroyedAnimation(Location);
				if (playSound)
					Game1.playSound("explosion");
				Utility.spreadAnimalsAround(building, Location);

				frozen = true;

				if (chest != null)
					Location.objects[new Vector2(
						building.tileX.Value + building.tilesWide.Value / 2,
						building.tileY.Value + building.tilesHigh.Value / 2
					)] = chest;


				// Try to refund the materials, maybe.
				var data = building.GetData();

				if (data?.BuildMaterials != null && Mod.Config.RefundMaterial > 0) 
					foreach(var entry in data.BuildMaterials) {
						int amount = (int) (entry.Amount * (Mod.Config.RefundMaterial / 100.0));
						if (amount > 0)
							Game1.player.addItemToInventory(ItemRegistry.Create(entry.ItemId, amount));
					}

				if (data != null && data.BuildCost > 0 && Mod.Config.RefundCurrency > 0) {
					int amount = (int) (data.BuildCost * (Mod.Config.RefundCurrency / 100.0));
					if (amount > 0)
						Game1.player.addUnearnedMoney(amount);
				}

				// Close the menu, we won.
				DelayedAction.functionAfterDelay(SuccessClose, 2000);
			}
		}

		checkingDemolish = true;

		if (cabin != null && cabin.HasOwner && cabin.owner.isCustomized.Value) {
			Game1.currentLocation.createQuestionDialogue(
				Game1.content.LoadString("Strings\\UI:Carpenter_DemolishCabinConfirm", cabin.owner.Name),
				Game1.currentLocation.createYesNoResponses(),
				delegate(Farmer who, string answer) {
					Game1.activeClickableMenu = this;
					if (answer == "Yes")
						Game1.player.team.demolishLock.RequestLock(continueDemolish, lockFailed);
					else
						checkingDemolish = false;
				}
			);

			return;
		}

		Game1.player.team.demolishLock.RequestLock(continueDemolish, lockFailed);
	}

	public override void update(GameTime time) {
		base.update(time);

		if (frozen)
			return;

		int mouseX = Game1.getOldMouseX(ui_scale: false) + Game1.viewport.X;
		int mouseY = Game1.getOldMouseY(ui_scale: false) + Game1.viewport.Y;

		// TODO: Scroll faster holding a key?

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

		if (Game1.IsMultiplayer || Location is null)
			return;

		foreach (var animal in Location.Animals.Values)
			animal.MovePosition(Game1.currentGameTime, Game1.viewport, Location);
	}

	#endregion

	#region Drawing

	public override void draw(SpriteBatch b) {
		if (frozen)
			return;

		Game1.StartWorldDrawInUI(b);

		if (Action == ActionType.Build)
			DrawBuild(b);

		else if (Action == ActionType.Move)
			DrawMove(b);

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

	public void DrawBuild(SpriteBatch b) {
		if (BuildingToBuild is null)
			return;

		DrawBuildingTiles(b, BuildingToBuild);
	}

	public void DrawBuildingTiles(SpriteBatch b, Building building) {
		if (Location is null)
			return;

		Vector2 pos = GetMouseTile();

		for (int y = 0; y < building.tilesHigh.Value; y++) {
			for(int x = 0; x < building.tilesWide.Value; x++) {
				int idx = building.getTileSheetIndexForStructurePlacementTile(x, y);
				Vector2 tile = pos.Move(x, y);
				if (!Location.isBuildable(tile))
					idx++;

				b.Draw(
					Game1.mouseCursors,
					Game1.GlobalToLocal(Game1.viewport, tile * 64f),
					new Rectangle(194 + idx * 16, 388, 16, 16),
					Color.White,
					0f,
					Vector2.Zero,
					4f,
					SpriteEffects.None,
					0.999f
				);
			}
		}

		foreach(var additional in building.GetAdditionalPlacementTiles()) {
			bool only_passable = additional.OnlyNeedsToBePassable;

			foreach(var point in additional.TileArea.GetPoints()) {
				int idx = building.getTileSheetIndexForStructurePlacementTile(point.X, point.Y);
				Vector2 tile = pos.Move(point.X, point.Y);
				if (!Location.isBuildable(tile, onlyNeedsToBePassable: only_passable))
					idx++;

				b.Draw(
					Game1.mouseCursors,
					Game1.GlobalToLocal(Game1.viewport, tile * 64f),
					new Rectangle(194 + idx * 16, 388, 16, 16),
					Color.White,
					0f,
					Vector2.Zero,
					4f,
					SpriteEffects.None,
					0.999f
				);
			}
		}
	}

	public void DrawMove(SpriteBatch b) {
		if (MovingBuilding is null || Location is null)
			return;

		DrawBuildingTiles(b, MovingBuilding);
	}

	#endregion

}
