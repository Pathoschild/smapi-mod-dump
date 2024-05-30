/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using AutoTrasher.Components;
using HedgeTech.Common.Classes;
using HedgeTech.Common.Extensions;

using SObject = StardewValley.Object;

namespace AutoTrasher
{
	public class ModEntry : Mod
	{
		private const string _trasherMessageType = "autotrash_{0}";

		private readonly LimitedList<Item> _reclaimList = new(10);
		private readonly List<Item> _ignoreItems = new();

		private ModConfig _config = new();
		private bool _isTrasherActive = true;

		public override void Entry(IModHelper helper)
		{
			I18n.Init(helper.Translation);

			_config = helper.ReadConfig<ModConfig>();
			_config.AddHelper(helper);

			_reclaimList.UpdateMaxSize(_config.ReclaimableItemCount);

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.Input.ButtonsChanged += OnButtonsChanged;
			helper.Events.Player.InventoryChanged += OnInventoryChanged;
		}

		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			_config.RegisterModConfigMenu(Helper, ModManifest, _reclaimList);
		}

		private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
		{

			if (Game1.activeClickableMenu is not null && _config.AddTrashKeybind.JustPressed())
			{
				if (Game1.activeClickableMenu is GameMenu menu && menu?.GetCurrentPage() is InventoryPage page)
				{
					var hoveredItem = page.hoveredItem;
					var notifMessage = string.Empty;
					var addNotifSubject = false;

					if (hoveredItem is not null)
					{
						if (hoveredItem is not SObject)
						{
							notifMessage = I18n.Notification_AddTrash_Fail(hoveredItem.DisplayName);
						}
						else if (!_config.TrashList.Contains(hoveredItem.ItemId))
						{
							_config.AddTrashItemToTrashList(hoveredItem.ItemId);
							RemoveItemFromInventory(hoveredItem);
							notifMessage = I18n.Notification_AddTrash_Success(hoveredItem.DisplayName);
							addNotifSubject = true;
						}
						else
						{
							notifMessage = I18n.Notification_AddTrash_Repeat(hoveredItem.DisplayName);
							addNotifSubject = true;
						}
					}
					else
					{
						Monitor.Log(I18n.Log_No_Item(), LogLevel.Debug);
					}

					if (notifMessage != string.Empty)
					{
						Monitor.Log(notifMessage, LogLevel.Info);

						var message = new HUDMessage(notifMessage)
						{
							messageSubject = addNotifSubject ? hoveredItem : null,
							type = $"autotrash_add_{hoveredItem?.ItemId ?? "-1"}",
							whatType = HUDMessage.error_type
						};

						Game1.addHUDMessage(message);
					}
				}
			}
			else if (Game1.activeClickableMenu is null)
			{
				if (Context.IsPlayerFree && Game1.currentMinigame is null)
				{
					if (_config.ToggleTrasherKeybind.JustPressed())
					{
						var message = string.Empty;
						_isTrasherActive = !_isTrasherActive;

						if (_isTrasherActive)
						{
							Helper.Events.Player.InventoryChanged += OnInventoryChanged;
							message = I18n.Notification_State(I18n.State_Activated());
						}
						else
						{
							Helper.Events.Player.InventoryChanged -= OnInventoryChanged;
							message = I18n.Notification_State(I18n.State_Deactivated());
						}

						Game1.addHUDMessage(new HUDMessage(message) { noIcon = true });
					}
					else if (_config.OpenTrashMenuKeybind.JustPressed())
					{
						Game1.activeClickableMenu = new TrashListMenu(_reclaimList, _ignoreItems, Monitor, _config);
					}
				}
			}
		}

		private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			foreach (var item in e.Added)
			{
				if (item is null) continue;
				if (!_config.TrashList.Contains(item.ItemId)) continue;

				if (_ignoreItems.Contains(item))
				{
					_ignoreItems.Remove(item);
					continue;
				}

				var message = new HUDMessage(I18n.Notification_Trashed(item.DisplayName))
				{
					number = item.Stack,
					type = _trasherMessageType.FormatWith(item.Name),
					messageSubject = item
				};

				Game1.addHUDMessage(message);
				RemoveItemFromInventory(item);
			}
		}

		private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
		{
			if (!Context.IsPlayerFree) return;
			if (!_isTrasherActive) return;

			var localMessages = Game1.hudMessages.ToList();

			if (localMessages.Any())
			{
				var currentMessages = localMessages.Where(m =>
					{
						if (m.messageSubject is null) return false;

						var isTrash = _config.TrashList.Contains(m.messageSubject.ItemId);
						var isNotTrasherMessage = !m.type.IEquals(_trasherMessageType.FormatWith(m.messageSubject.Name));

						return isTrash && isNotTrasherMessage;
					});

				if (currentMessages.Any())
				{
					foreach (var msg in currentMessages)
					{
						Game1.hudMessages.Remove(msg);
					}
				}
			}
		}

		private void RemoveItemFromInventory(Item item)
		{
			var reclamationPrice = Utility.getTrashReclamationPrice(item, Game1.player);

			if (reclamationPrice > 0)
			{
				Game1.player.Money += reclamationPrice;
				Game1.soundBank.PlayCue("coin");
			}

			_reclaimList.Add(item);
			Game1.player.removeItemFromInventory(item);
			Game1.soundBank.PlayCue("trashcan");
		}
	}
}
