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
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Types;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;

using StardewValley;
using StardewValley.Objects;
using StardewValley.GameData;
using StardewValley.Menus;

using SDObject = StardewValley.Object;

using Leclair.Stardew.Almanac.Menus;

namespace Leclair.Stardew.Almanac.Pages;

internal class DebugItemsState : BaseState {
	public string Item;
}

internal class DebugItemsPage : BasePage<DebugItemsState> {

	private Item CurrentItem;

	private readonly Cache<IEnumerable<IFlowNode>, Item> ItemInfo;
	private readonly Dictionary<string, SelectableNode> ItemNodes = new();

	#region Life Cycle

	public static DebugItemsPage GetPage(AlmanacMenu menu, ModEntry mod) {
		if (!mod.HasAlmanac(Game1.player) )// || !mod.Config.DebugMode)
			return null;

		return new(menu, mod);
	}

	public DebugItemsPage(AlmanacMenu menu, ModEntry mod) : base(menu, mod) {
		ItemInfo = new(item => BuildRightPage(item), () => CurrentItem);
	}

	public override void ThemeChanged() {
		base.ThemeChanged();

		foreach (var node in ItemNodes.Values) {
			node.SelectedTexture = Menu.background;
			node.HoverTexture = Menu.background;
		}
	}

	#endregion

	#region State Saving

	public override DebugItemsState SaveState() {
		var state = base.SaveState();

		state.Item = CurrentItem?.QualifiedItemId;

		return state;
	}

	public override void LoadState(DebugItemsState state) {
		base.LoadState(state);

		var item = string.IsNullOrEmpty(state.Item) ? null : ItemRegistry.Create(state.Item, 1, allowNull: true);
		SelectItem(item);
	}

	#endregion

	#region Logic

	public bool SelectItem(Item item) {
		if (item == null || CurrentItem == null || item.QualifiedItemId != CurrentItem.QualifiedItemId) {
			CurrentItem = item;
			SetRightFlow(ItemInfo.Value);

			foreach(var pair in ItemNodes) {
				pair.Value.Selected = pair.Key == CurrentItem?.QualifiedItemId;
			}

			return true;
		}

		return false;
	}

	public IFlowNode[] BuildRightPage(Item item) {
		if (CurrentItem == null)
			return null;

		FlowBuilder builder = FlowHelper.Builder();

		var sprite = SpriteHelper.GetSprite(item);

		builder.Text(item.DisplayName, fancy: true, align: Alignment.HCenter);
		builder.Text("\n\n");

		builder
			.Sprite(sprite, 1f, Alignment.Bottom | Alignment.HCenter)
			.Text(" ")
			.Sprite(sprite, 2f, Alignment.Bottom)
			.Text(" ")
			.Sprite(sprite, 4f, Alignment.Bottom)
			.Text(" ")
			.Sprite(sprite, 8f, Alignment.Bottom);

		builder.Text("\n\n");

		builder
			.Text("ID: ", bold: true)
			.Text(item.QualifiedItemId, shadow: false);

		return builder.Build();
	}

	public override void Update() {
		base.Update();

		FlowBuilder builder = new();
		ItemNodes.Clear();
		foreach (var type in ItemRegistry.ItemTypes) {
			builder.Text("\n").Text(type.Identifier, font: Game1.dialogueFont).Text("\n");
			foreach (string itemID in type.GetAllIds()) {
				var item = ItemRegistry.Create($"{type.Identifier}{itemID}", 1, allowNull: true);
				if (item == null)
					continue;

				FlowBuilder sb = FlowHelper.Builder()
					.Sprite(SpriteHelper.GetSprite(item), 3f);

				var node = new SelectableNode(
					sb.Build(),
					onHover: (_, _, _) => {
						Menu.HoveredItem = item;
						Menu.HoverText = $"{item.DisplayName}\n{item.QualifiedItemId}";
						return true;
					},

					onClick: (_, _, _) => {
						if (SelectItem(item))
							Game1.playSound("smallSelect");
						return true;
					}
				) {
					SelectedTexture = Menu.background,
					SelectedSource = new(336, 352, 16, 16),
					HoverTexture = Menu.background,
					HoverSource = new(336, 352, 16, 16),
					HoverColor = Color.White * 0.4f
				};

				ItemNodes.Add(item.QualifiedItemId, node);
				builder.Add(node);
			}
		}

		SetLeftFlow(builder);
	}

	#endregion

	#region ITab

	public override int SortKey => 1000;
	public override string TabSimpleTooltip => "Debug: Items";
	public override Texture2D TabTexture => Game1.mouseCursors;
	public override Rectangle? TabSource => Rectangle.Empty;

	#endregion

	#region IAlmanacPage

	public override PageType Type => PageType.Blank;

	#endregion

}
