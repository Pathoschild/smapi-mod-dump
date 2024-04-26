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
using System.Collections.Generic;


using Leclair.Stardew.BetterCrafting;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Types;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;

namespace Leclair.Stardew.BCBuildings;

public class BuildingRecipe : IDynamicDrawingRecipe {

	public readonly ModEntry Mod;
	public readonly BuildingData Data;

	private int CacheBuster = 0;

	private Cache<string, int> _DisplayName;
	private Cache<string, int> _Description;
	//private Cache<Texture2D, int> _Texture;

	private Texture2D? _Texture = null;
	private bool IsTextureLoading = false;

	public readonly string BuildingId;
	public readonly string? SkinId;
	public readonly int SkinCount;

	public readonly BuildingSkin? SkinData;

	public BuildingRecipe(ModEntry mod, string id, string? skinId, BuildingData data) {
		Mod = mod;
		Data = data;
		BuildingId = id;
		SkinId = skinId;
		Name = "bcbuildings:" + (skinId == null ? BuildingId : $"{BuildingId}/{SkinId}");

		BuildingSkin? sd = null;
		int skins = 1;

		if (Data.Skins != null)
			foreach(var skin in Data.Skins) {
				if (!skin.ShowAsSeparateConstructionEntry)
					skins++;

				if (skin.Id == SkinId)
					sd = skin;
			}

		if (sd != null)
			skins = 1;

		SkinData = sd;
		SkinCount = skins;

		_DisplayName = new(_ => TokenParser.ParseText(SkinData?.Name ?? Data.Name), () => CacheBuster);
		_Description = new(_ => TokenParser.ParseText(SkinData?.Description ?? Data.Description), () => CacheBuster);

		List<IIngredient> ings = new();
		int amount;

		//ings.Add(Mod.BCAPI!.CreateCurrencyIngredient(CurrencyType.Money, 1));

		if (Data.BuildMaterials is not null)
			foreach (var mat in Data.BuildMaterials) {
				amount = (int) (mat.Amount * (Mod.Config.CostMaterial / 100.0));
				if (amount > 0)
					ings.Add(Mod.BCAPI!.CreateBaseIngredient(mat.ItemId, mat.Amount));
			}

		amount = (int) (Data.BuildCost * (Mod.Config.CostMaterial / 100.0));
		if (amount > 0)
			ings.Add(Mod.BCAPI!.CreateCurrencyIngredient(CurrencyType.Money, amount));

		var additional = Mod.GetAdditionalCost();
		if (additional is not null)
			ings.AddRange(additional);

		Ingredients = ings.ToArray();
	}

	public string Builder => Data.Builder;

	#region IRecipe

	// Identity

	public string SortValue => "aaa"; // string.Empty;

	public string Name { get; }

	public string DisplayName => _DisplayName.Value;

	public string? Description => SkinCount > 1 ? $"{_Description.Value}\nSkins: {SkinCount}" : _Description.Value;

	public bool AllowRecycling => false;

	public bool HasRecipe(Farmer who) {
		return true;
	}

	public int GetTimesCrafted(Farmer who) {
		return -1;
	}

	public CraftingRecipe? CraftingRecipe => null;

	// Display

	private void LoadTexture() {
		if (IsTextureLoading)
			return;

		IsTextureLoading = true;
		Mod.Renderer.RenderBuilding(BuildingId, SkinId, tex => _Texture = tex);
	}

	public bool ShouldDoDynamicDrawing => true;

	public void Draw(SpriteBatch b, Rectangle bounds, Color color, bool ghosted, bool canCraft, float layerDepth, ClickableTextureComponent? cmp) {
		LoadTexture();

		if (cmp != null) {
			cmp.texture = Texture;
			cmp.sourceRect = SourceRectangle;

			cmp.DrawBounded(b, color, layerDepth);
			return;
		}

		Rectangle source = SourceRectangle;

		float width = source.Width;
		float height = source.Height;

		float bWidth = bounds.Width;
		float bHeight = bounds.Height;

		float s = Math.Min(bWidth / width, bHeight / height);

		width *= s;
		height *= s;

		float offsetX = (bWidth - width) / 2;
		float offsetY = (bHeight - height) / 2;

		b.Draw(
			Texture,
			new Vector2(bounds.X + offsetX, bounds.Y + offsetY),
			source,
			color,
			0f,
			Vector2.Zero,
			s,
			SpriteEffects.None,
			layerDepth
		);
	}


	public Texture2D Texture => _Texture ?? Game1.mouseCursors;
	public Rectangle SourceRectangle => _Texture?.Bounds ?? new Rectangle(173, 423, 16, 16);

	public int GridHeight {
		get {
			Rectangle rect = SourceRectangle;
			return rect.Height > rect.Width ? 2 : 2;
		}
	}

	public int GridWidth {
		get {
			Rectangle rect = SourceRectangle;
			return rect.Width > rect.Height ? 2 : 2;
		}
	}

	// Cost

	public int QuantityPerCraft => 1;

	public IIngredient[] Ingredients { get; }

	// Creation

	public bool Stackable => false;

	public Item? CreateItem() {
		return null;
	}

	public bool CanCraft(Farmer who) {
		if ( ! who.currentLocation.IsBuildableLocation() )
			return false;

		if (Data.BuildingToUpgrade != null && !who.currentLocation.isBuildingConstructed(Data.BuildingToUpgrade))
			return false;

		return true;
	}


	public string? GetTooltipExtra(Farmer who) {
		if (!who.currentLocation.IsBuildableLocation())
			return I18n.Error_NotBuildable();

		if (Data.BuildingToUpgrade != null && !who.currentLocation.isBuildingConstructed(Data.BuildingToUpgrade)) {
			// TODO: Look up display name.
			string other = Data.BuildingToUpgrade;
			return I18n.Error_CantUpgrade(other);
		}

		return null;
	}

	public void PerformCraft(IPerformCraftEvent evt) {

		var bld = Building.CreateInstanceFromId(BuildingId, Vector2.Zero);
		bld.skinId.Value = SkinId;

		if (bld.CanBeReskinned(ignoreSeparateConstructionEntries: true)) {
			var child = new BuildingSkinMenu(bld);

			var old_menu = Game1.activeClickableMenu;
			Game1.activeClickableMenu = child;

			child.exitFunction = () => {
				Game1.activeClickableMenu = old_menu;
				StageTwoPerformCraft(evt, bld.skinId.Value);
			};

		} else
			StageTwoPerformCraft(evt, SkinId);
			
	}

	private void StageTwoPerformCraft(IPerformCraftEvent evt, string? skinId) { 

		var menu = new BuildMenu(Mod, Data.BuildingToUpgrade != null ? ActionType.Upgrade : ActionType.Build, BuildingId, skinId, Data, evt);
		var old_menu = Game1.activeClickableMenu;

		Game1.activeClickableMenu = menu;

		menu.exitFunction = () => {
			Game1.activeClickableMenu = old_menu;
			evt.Cancel();
		};

	}

	#endregion

}
