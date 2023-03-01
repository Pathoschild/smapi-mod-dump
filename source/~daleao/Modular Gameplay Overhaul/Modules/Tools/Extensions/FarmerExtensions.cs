/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Extensions;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions.Stardew;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class FarmerExtensions
{
    /// <summary>Checks whether the <paramref name="crop"/> can be harvested with a sickle.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="crop">The <see cref="Crop"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="farmer"/> is holding a sickle and the user's config settings allow sickle-harvesting the given <paramref name="crop"/>, otherwise <see langword="false"/>.</returns>
    internal static bool CanSickleHarvest(this Farmer farmer, Crop crop)
    {
        var tool = farmer.CurrentTool;
        if (tool?.IsScythe() != true)
        {
            return false;
        }

        if (crop.harvestMethod.Value == Crop.sickleHarvest)
        {
            return true;
        }

        var config = ToolsModule.Config.Scythe;
        if (config.GoldScytheOnly && tool.InitialParentTileIndex != Constants.GoldenScytheIndex)
        {
            return false;
        }

        if (crop.programColored.Value || crop.indexOfHarvest.Value == Constants.SunflowerIndex)
        {
            return config.HarvestFlowers;
        }

        if (crop.forageCrop.Value)
        {
            return config.HarvestForage;
        }

        return crop.harvestMethod.Value == Crop.grabHarvest && config.HarvestCrops;
    }

    /// <summary>
    ///     Temporarily sets up the <paramref name="farmer"/> to interact with a tile, then return it to the original
    ///     state.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="action">The action to perform.</param>
    internal static void TemporarilyFakeInteraction(this Farmer farmer, Action action)
    {
        // save current state
        var stamina = farmer.stamina;
        var position = farmer.Position;
        var facingDirection = farmer.FacingDirection;
        var currentToolIndex = farmer.CurrentToolIndex;
        var canMove = farmer.canMove; // fix player frozen due to animations when performing an action

        // perform action
        try
        {
            action();
        }
        finally
        {
            // restore previous state
            farmer.stamina = stamina;
            farmer.Position = position;
            farmer.FacingDirection = facingDirection;
            farmer.CurrentToolIndex = currentToolIndex;
            farmer.canMove = canMove;
        }
    }

    /// <summary>Cancels the <paramref name="farmer"/>'s current animation if it matches one of the given IDs.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="animationIds">The animation IDs to detect.</param>
    internal static void CancelAnimation(this Farmer farmer, params int[] animationIds)
    {
        var animationId = ModHelper.Reflection.GetField<int>(farmer.FarmerSprite, "currentSingleAnimation")
            .GetValue();
        if (animationIds.All(id => id != animationId))
        {
            return;
        }

        farmer.completelyStopAnimatingOrDoingAction();
        farmer.forceCanMove();
    }
}
