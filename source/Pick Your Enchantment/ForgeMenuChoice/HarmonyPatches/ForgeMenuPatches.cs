/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/ForgeMenuChoice
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley.Menus;

namespace ForgeMenuChoice.HarmonyPatches;

/// <summary>
/// Holds patches against the forge menu.
/// </summary>
/// <remarks>Also used to patch SpaceCore's forge menu.</remarks>
[HarmonyPatch(typeof(ForgeMenu))]
internal static class ForgeMenuPatches
{
    private static readonly PerScreen<List<BaseEnchantment>> PossibleEnchantmentPerscreen = new(() => new());
    private static readonly PerScreen<ForgeSelectionMenu?> MenuPerscreen = new();

    /// <summary>
    /// Gets the current selected enchantment from the menu, if the menu exists.
    /// </summary>
    public static BaseEnchantment? CurrentSelection
        => Menu?.CurrentSelectedOption;

    private static List<BaseEnchantment> PossibleEnchantments => PossibleEnchantmentPerscreen.Value;

    private static ForgeSelectionMenu? Menu
    {
        get => MenuPerscreen.Value;
        set => MenuPerscreen.Value = value;
    }

    /// <summary>
    /// Exits and trashes the minimenu.
    /// </summary>
    internal static void TrashMenu()
    {
        Menu?.exitThisMenu(false);
        Menu = null;
    }

    /// <summary>
    /// Prefix before exiting menu - this closes the minimenu.
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch("cleanupBeforeExit")]
    internal static void PrefixBeforeExit() => TrashMenu();

    /// <summary>
    /// Prefixes IsValidCraft to gather possible enchantments.
    /// </summary>
    /// <param name="__0">Left item (tool slot).</param>
    /// <param name="__1">Right item (possibly prismatic).</param>
    /// <param name="__result">Result to feed to original function.</param>
    /// <returns>True to continue to original function, false otherwise.</returns>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyPatch(nameof(ForgeMenu.IsValidCraft))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    internal static bool PrefixIsValidCraft(Item __0, Item __1, ref bool __result)
    {
        try
        {
            // 74 - prismatic shard.
            if (__0 is Tool tool && Utility.IsNormalObjectAtParentSheetIndex(__1, 74))
            {
                PossibleEnchantments.Clear();
                foreach (BaseEnchantment enchantment in BaseEnchantment.GetAvailableEnchantments())
                {
                    if (enchantment.CanApplyTo(tool) && !tool.enchantments.Any((enchantOnTool) => enchantOnTool.GetType() == enchantment.GetType()))
                    {
                        PossibleEnchantments.Add(enchantment);
                    }
                }
                if (PossibleEnchantments.Count > 0)
                {
                    Menu ??= new(options: PossibleEnchantments);
                    __result = true;
                    return false;
                }
            }
            TrashMenu();
            return true;
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Error in postfixing IsValidCraft:\n{ex}", LogLevel.Error);
        }
        return true;
    }

    /// <summary>
    /// Postfixes the forge menu's draw call to draw the smol menu.
    /// </summary>
    /// <param name="b">Spritebatch to draw with.</param>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ForgeMenu.draw))]
    internal static void PostfixDraw(SpriteBatch b)
        => Menu?.draw(b);

    /// <summary>
    /// Postfixes the forge menu's left click to also process left clicks for the smol menu.
    /// </summary>
    /// <param name="x">X location clicked.</param>
    /// <param name="y">Y location clicked.</param>
    /// <param name="playSound">Whether or not to play sounds.</param>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ForgeMenu.receiveLeftClick))]
    internal static void PostFixLeftClick(int x, int y, bool playSound)
        => Menu?.receiveLeftClick(x, y, playSound);

    /// <summary>
    /// Postfixes the forge menu's right click to also process right clicks for the smol menu.
    /// </summary>
    /// <param name="x">X location clicked.</param>
    /// <param name="y">Y location clicked.</param>
    /// <param name="playSound">Whether or not to play sounds.</param>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ForgeMenu.receiveRightClick))]
    internal static void PostfixRightClick(int x, int y, bool playSound)
        => Menu?.receiveRightClick(x, y, playSound);

    /// <summary>
    /// Postfixes the forge menu's resizing to also move the smol menu.
    /// </summary>
    /// <param name="oldBounds">Old boundaries.</param>
    /// <param name="newBounds">New boundaries.</param>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ForgeMenu.gameWindowSizeChanged))]
    internal static void PostfixGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        => Menu?.gameWindowSizeChanged(oldBounds, newBounds);

    /// <summary>
    /// Postfixes the forge menu's hovering to handle hovering in the smol menu.
    /// </summary>
    /// <param name="x">Pixel hovered over (X).</param>
    /// <param name="y">Pixel hovered over (Y).</param>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ForgeMenu.performHoverAction))]
    internal static void PostfixPerformHoverAction(int x, int y)
        => Menu?.performHoverAction(x, y);
}