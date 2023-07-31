/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Shockah.Kokoro;

internal static class LoadGameMenuPatches
{
	public enum TooltipContent
	{
		ListMods, ListChanges, ListChangesAndOtherMods
	}

	private static readonly ConditionalWeakTable<LoadGameMenu, List<ClickableTextureComponent>> ModInfoButtons = new();
	private static readonly ConditionalWeakTable<LoadGameMenu, Func<LoadGameMenu, List<LoadGameMenu.MenuSlot>>> MenuSlotsGetters = new();
	private static readonly Lazy<Func<LoadGameMenu, int>> CurrentItemIndexGetter = new(() => AccessTools.Field(typeof(LoadGameMenu), "currentItemIndex").EmitInstanceGetter<LoadGameMenu, int>());
    private static readonly Lazy<Func<LoadGameMenu, int>> TimerToLoadGetter = new(() => AccessTools.Field(typeof(LoadGameMenu), "timerToLoad").EmitInstanceGetter<LoadGameMenu, int>());
    private static readonly Lazy<Func<LoadGameMenu, bool>> LoadingGetter = new(() => AccessTools.Field(typeof(LoadGameMenu), "loading").EmitInstanceGetter<LoadGameMenu, bool>());
    private static readonly Lazy<Func<LoadGameMenu, bool>> DeletingGetter = new(() => AccessTools.Field(typeof(LoadGameMenu), "deleting").EmitInstanceGetter<LoadGameMenu, bool>());
    private static readonly Lazy<Action<LoadGameMenu, string>> HoverTextSetter = new(() => AccessTools.Field(typeof(LoadGameMenu), "hoverText").EmitInstanceSetter<LoadGameMenu, string>());
	private static readonly Lazy<Dictionary<string, SaveFileDescriptor.ModDescriptor>> CurrentStateModDictionary = new(SaveFileDescriptor.GetModDictionaryFromCurrentState);

	internal static void Apply(Harmony harmony)
	{
		harmony.TryPatch(
			monitor: Kokoro.Instance.Monitor,
			original: () => AccessTools.Constructor(typeof(LoadGameMenu), Array.Empty<Type>()),
			postfix: new HarmonyMethod(AccessTools.Method(typeof(LoadGameMenuPatches), nameof(LoadGameMenu_ctor_Postfix)))
		);
        harmony.TryPatch(
            monitor: Kokoro.Instance.Monitor,
            original: () => AccessTools.Method(typeof(LoadGameMenu), nameof(LoadGameMenu.UpdateButtons)),
            postfix: new HarmonyMethod(AccessTools.Method(typeof(LoadGameMenuPatches), nameof(LoadGameMenu_UpdateButtons_Postfix)))
        );
        harmony.TryPatch(
            monitor: Kokoro.Instance.Monitor,
            original: () => AccessTools.Method(typeof(LoadGameMenu), nameof(LoadGameMenu.gameWindowSizeChanged)),
            postfix: new HarmonyMethod(AccessTools.Method(typeof(LoadGameMenuPatches), nameof(LoadGameMenu_gameWindowSizeChanged_Postfix)))
        );
        harmony.TryPatch(
            monitor: Kokoro.Instance.Monitor,
            original: () => AccessTools.Method(typeof(LoadGameMenu), nameof(LoadGameMenu.performHoverAction)),
            postfix: new HarmonyMethod(AccessTools.Method(typeof(LoadGameMenuPatches), nameof(LoadGameMenu_performHoverAction_Postfix)))
        );
        harmony.TryPatch(
            monitor: Kokoro.Instance.Monitor,
            original: () => AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.draw), new Type[] { typeof(SpriteBatch) }),
            postfix: new HarmonyMethod(AccessTools.Method(typeof(LoadGameMenuPatches), nameof(IClickableMenu_draw_Postfix)))
        );
        harmony.TryPatch(
            monitor: Kokoro.Instance.Monitor,
            original: () => AccessTools.Method(typeof(LoadGameMenu), nameof(LoadGameMenu.receiveLeftClick)),
            prefix: new HarmonyMethod(AccessTools.Method(typeof(LoadGameMenuPatches), nameof(LoadGameMenu_receiveLeftClick_Prefix)))
        );
    }

	private static void LoadGameMenu_ctor_Postfix(LoadGameMenu __instance)
	{
        var modInfoButtons = __instance.slotButtons
            .Select(b => new ClickableTextureComponent("", new Rectangle(b.bounds.X + b.bounds.Width - 104, b.bounds.Y + 16, 48, 48), "", "", Game1.mouseCursors, new Rectangle(208, 320, 16, 16), 2.5f)
            {
                myID = b.myID + 100,
                region = b.region,
                leftNeighborImmutable = b.leftNeighborImmutable,
                leftNeighborID = b.leftNeighborID,
                downNeighborImmutable = b.downNeighborImmutable,
                downNeighborID = b.downNeighborID,
                upNeighborImmutable = b.upNeighborImmutable,
                upNeighborID = b.upNeighborID,
                rightNeighborImmutable = b.rightNeighborImmutable,
                rightNeighborID = b.rightNeighborID,
				visible = false
            })
            .ToList();
        ModInfoButtons.AddOrUpdate(__instance, modInfoButtons);

		static PropertyInfo GetMenuSlotsProperty(LoadGameMenu __instance)
		{
			var type = __instance.GetType();
			while (true)
			{
				var property = AccessTools.Property(type, "MenuSlots");
				if (property is not null)
					return property;
				if (type == typeof(LoadGameMenu))
					throw new InvalidOperationException();
			}
		}
		MenuSlotsGetters.AddOrUpdate(__instance, GetMenuSlotsProperty(__instance).EmitInstanceGetter<LoadGameMenu, List<LoadGameMenu.MenuSlot>>());
	}

	private static void LoadGameMenu_UpdateButtons_Postfix(LoadGameMenu __instance)
	{
		if (!ModInfoButtons.TryGetValue(__instance, out var modInfoButtons))
			return;
		if (!MenuSlotsGetters.TryGetValue(__instance, out var menuSlotGetter))
			return;

		var menuSlots = menuSlotGetter(__instance);
		var currentItemIndex = CurrentItemIndexGetter.Value(__instance);

		for (int i = 0; i < modInfoButtons.Count; i++)
		{
			var modInfoButton = modInfoButtons[i];
			if (i >= menuSlots.Count)
			{
				modInfoButton.visible = false;
				continue;
			}

			var menuSlot = menuSlots[i + currentItemIndex];
			if (menuSlot is not LoadGameMenu.SaveFileSlot saveFileSlot || !(menuSlot.GetType() == typeof(LoadGameMenu.SaveFileSlot) || menuSlot.GetType() == AccessTools.Inner(typeof(CoopMenu), "HostFileSlot")))
			{
				modInfoButton.visible = false;
				continue;
			}

			modInfoButton.visible = true;
			modInfoButton.hoverText = BuildTooltip(saveFileSlot.Farmer, TooltipContent.ListChanges, limited: true);
		}
	}

	private static void LoadGameMenu_gameWindowSizeChanged_Postfix(LoadGameMenu __instance)
	{
        if (!ModInfoButtons.TryGetValue(__instance, out var modInfoButtons))
            return;

		for (int i = 0; i < modInfoButtons.Count; i++)
		{
			var b = __instance.slotButtons[i];
			modInfoButtons[i].bounds = new Rectangle(b.bounds.X + b.bounds.Width - 104, b.bounds.Y + 16, 48, 48);
        }
    }

    private static void LoadGameMenu_performHoverAction_Postfix(LoadGameMenu __instance, int x, int y)
	{
        if (!ModInfoButtons.TryGetValue(__instance, out var modInfoButtons))
            return;

        foreach (var modInfoButton in modInfoButtons)
        {
            modInfoButton.tryHover(x, y, 0.2f);
            if (modInfoButton.containsPoint(x, y))
            {
				HoverTextSetter.Value(__instance, modInfoButton.hoverText);
                return;
            }
        }
    }

	private static void IClickableMenu_draw_Postfix(IClickableMenu __instance, SpriteBatch b)
	{
		if (__instance is not LoadGameMenu menu)
			return;
        if (!ModInfoButtons.TryGetValue(menu, out var modInfoButtons))
            return;

		foreach (var modInfoButton in modInfoButtons)
			modInfoButton.draw(b, Color.White * 0.75f, 1f);
    }

	private static bool LoadGameMenu_receiveLeftClick_Prefix(LoadGameMenu __instance, int x, int y, bool playSound)
    {
        if (__instance.deleteConfirmationScreen)
            return true;
        if (!ModInfoButtons.TryGetValue(__instance, out var modInfoButtons))
            return true;
		if (!MenuSlotsGetters.TryGetValue(__instance, out var menuSlotGetter))
			return true;
		if (TimerToLoadGetter.Value(__instance) > 0 || LoadingGetter.Value(__instance) || DeletingGetter.Value(__instance))
			return true;

        var menuSlots = menuSlotGetter(__instance);
        var currentItemIndex = CurrentItemIndexGetter.Value(__instance);

        for (int i = 0; i < modInfoButtons.Count; i++)
        {
            if (modInfoButtons[i].containsPoint(x, y) && i < menuSlots.Count)
            {
                var menuSlot = menuSlots[i + currentItemIndex];
                if (menuSlot is not LoadGameMenu.SaveFileSlot saveFileSlot)
                    continue;

                DesktopClipboard.SetText(BuildTooltip(saveFileSlot.Farmer, TooltipContent.ListMods, limited: false));
				Game1.addHUDMessage(new(Kokoro.Instance.Helper.Translation.Get("saveDescriptor.copiedToClipboard")));
                if (playSound)
                    Game1.playSound("select");
                return false;
            }
        }

        return true;
    }

    private static string BuildTooltip(Farmer player, TooltipContent content, bool limited)
	{
		var kokoro = Kokoro.Instance;
		StringBuilder sb = new();
		sb.AppendLine(kokoro.Helper.Translation.Get("saveDescriptor.gameVersion", new { Version = player.gameVersion }));

		var descriptor = kokoro.GetSaveFileDescriptor(player);
		if (descriptor is null)
		{
			sb.AppendLine(kokoro.Helper.Translation.Get("saveDescriptor.noInfo"));
		}
		else
		{
			sb.AppendLine(kokoro.Helper.Translation.Get("saveDescriptor.smapiVersion", new { Version = descriptor.SmapiVersion }));

			var descriptorMods = descriptor.Mods.OrderBy(kvp => kvp.Value.Name).ToList();
			var currentMods = CurrentStateModDictionary.Value.OrderBy(kvp => kvp.Value.Name).ToList();

			var pendingRemovalMods = descriptorMods
				.Where(kvp => !CurrentStateModDictionary.Value.ContainsKey(kvp.Key))
				.ToList();
			var pendingAdditionMods = currentMods
				.Where(kvp => !descriptor.Mods.ContainsKey(kvp.Key))
				.ToList();
			var pendingDowngradeMods = descriptorMods
				.Where(kvp => CurrentStateModDictionary.Value.TryGetValue(kvp.Key, out var currentModDescriptor) && currentModDescriptor.Version.IsOlderThan(kvp.Value.Version))
				.ToList();
			var pendingUpgradeMods = descriptorMods
				.Where(kvp => CurrentStateModDictionary.Value.TryGetValue(kvp.Key, out var currentModDescriptor) && currentModDescriptor.Version.IsNewerThan(kvp.Value.Version))
				.ToList();
			var matchingMods = descriptorMods
				.Where(kvp => !pendingRemovalMods.Any(m => m.Key == kvp.Key) && !pendingAdditionMods.Any(m => m.Key == kvp.Key) && !pendingDowngradeMods.Any(m => m.Key == kvp.Key) && !pendingUpgradeMods.Any(m => m.Key == kvp.Key))
				.ToList();

			int modSections = (pendingRemovalMods.Count == 0 ? 0 : 1) + (pendingAdditionMods.Count == 0 ? 0 : 1) + (pendingDowngradeMods.Count == 0 ? 0 : 1) + (pendingUpgradeMods.Count == 0 ? 0 : 1) + (matchingMods.Count == 0 ? 0 : 1);
            int maxModLines = limited ? 16 / modSections : int.MaxValue;

            void ListMods(string titleKey, List<KeyValuePair<string, SaveFileDescriptor.ModDescriptor>> mods, Func<KeyValuePair<string, SaveFileDescriptor.ModDescriptor>, string> lineBuilder)
            {
                if (mods.Count == 0)
                    return;
                sb.AppendLine();
                sb.AppendLine(kokoro.Helper.Translation.Get(titleKey, new { Count = mods.Count }));
				for (int i = 0; i < mods.Count; i++)
				{
					if (i == maxModLines - 1 && mods.Count != maxModLines)
					{
                        sb.AppendLine(kokoro.Helper.Translation.Get("saveDescriptor.andXMore", new { Count = mods.Count - i }));
                        break;
					}
					sb.AppendLine(lineBuilder(mods[i]));
				}
            }

            switch (content)
			{
				case TooltipContent.ListMods:
					ListMods("saveDescriptor.allMods", descriptorMods, mod => $" {BuildTooltip(mod)}");
                    break;
				case TooltipContent.ListChanges:
					if (pendingRemovalMods.Count == 0 && pendingAdditionMods.Count == 0 && pendingDowngradeMods.Count == 0 && pendingUpgradeMods.Count == 0)
					{
                        sb.AppendLine(kokoro.Helper.Translation.Get("saveDescriptor.allModsMatch", new { Count = descriptorMods.Count }));
                    }
					else
					{
                        ListMods("saveDescriptor.pendingRemovalMods", pendingRemovalMods, mod => $" - {BuildTooltip(mod)}");
                        ListMods("saveDescriptor.pendingAdditionMods", pendingAdditionMods, mod => $" + {BuildTooltip(mod)}");
                        ListMods("saveDescriptor.pendingDowngradeMods", pendingDowngradeMods, mod => $" \\ {BuildTooltip(mod, CurrentStateModDictionary.Value[mod.Key].Version)}");
                        ListMods("saveDescriptor.pendingUpgradeMods", pendingUpgradeMods, mod => $" / {BuildTooltip(mod, CurrentStateModDictionary.Value[mod.Key].Version)}");
                    }
                    break;
				case TooltipContent.ListChangesAndOtherMods:
                    if (pendingRemovalMods.Count == 0 && pendingAdditionMods.Count == 0 && pendingDowngradeMods.Count == 0 && pendingUpgradeMods.Count == 0)
                    {
                        ListMods("saveDescriptor.allMods", descriptorMods, mod => $" {BuildTooltip(mod)}");
                    }
                    else
                    {
                        ListMods("saveDescriptor.pendingRemovalMods", pendingRemovalMods, mod => $" - {BuildTooltip(mod)}");
                        ListMods("saveDescriptor.pendingAdditionMods", pendingAdditionMods, mod => $" + {BuildTooltip(mod)}");
                        ListMods("saveDescriptor.pendingDowngradeMods", pendingDowngradeMods, mod => $" \\ {BuildTooltip(mod, CurrentStateModDictionary.Value[mod.Key].Version)}");
                        ListMods("saveDescriptor.pendingUpgradeMods", pendingUpgradeMods, mod => $" / {BuildTooltip(mod, CurrentStateModDictionary.Value[mod.Key].Version)}");
                        ListMods("saveDescriptor.matchingMods", matchingMods, mod => $" {BuildTooltip(mod)}");
                    }
                    break;
            }
		}

		return sb.ToString().Trim();
	}

	private static string BuildTooltip(KeyValuePair<string, SaveFileDescriptor.ModDescriptor> mod, ISemanticVersion? newVersion = null)
	{
		if (newVersion is null)
			return $"{mod.Value.Name} {mod.Value.Version}";
		else
            return $"{mod.Value.Name}: {mod.Value.Version} -> {newVersion}";
    }
}