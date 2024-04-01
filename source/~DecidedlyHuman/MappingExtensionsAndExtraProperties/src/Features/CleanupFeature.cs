/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using DecidedlyShared.Ui;
using HarmonyLib;
using MappingExtensionsAndExtraProperties.Models.EventArgs;
using MappingExtensionsAndExtraProperties.Models.TileProperties;
using StardewValley;

namespace MappingExtensionsAndExtraProperties.Features;

/// <summary>
/// The purpose of this feature is to do cleanup of things that aren't strongly tied to any specific feature.
/// </summary>
public class CleanupFeature : Feature
{
    public override string FeatureId { get; init; }
    public override Harmony HarmonyPatcher { get; init; }
    public override bool AffectsCursorIcon { get; init; }
    public override int CursorId { get; init; }
    public override bool Enabled { get; internal set; }
    public override void Enable()
    {
        this.Enabled = true;
    }

    public override void Disable()
    {
        this.Enabled = false;
    }

    public CleanupFeature(string id)
    {
        this.FeatureId = id;
    }

    public override void RegisterCallbacks()
    {
        FeatureManager.OnLocationChangeCallback += this.OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, OnLocationChangeEventArgs e)
    {
        // We need to ensure we can kill relevant UIs if the player is warping.
        if (Game1.activeClickableMenu is MenuBase menu)
        {
            // If it's one of our menus, we close it.
            // This should be refactored use an owned-menu system at some point.
            if (menu.MenuName.Equals(CloseupInteractionImage.PropertyKey))
                Game1.exitActiveMenu();
        }
    }

    public override bool ShouldChangeCursor(GameLocation location, int tileX, int tileY, out int cursorId)
    {
        cursorId = default;

        return false;
    }
}
