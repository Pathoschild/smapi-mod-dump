/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Services.Patchers.Items;

using HarmonyLib;
using StardewMods.Common.Enums;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Models;
using StardewMods.Common.Services.Integrations.FuryCore;
using StardewMods.SpritePatcher.Framework.Enums.Patches;
using StardewMods.SpritePatcher.Framework.Services.Factory;
using StardewValley.Objects;

/// <summary>Patches for <see cref="Ring" /> draw methods.</summary>
internal sealed class RingPatcher : BasePatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingPatcher" /> class.</summary>
    /// <param name="configManager">Dependency used for managing config data.</param>
    /// <param name="eventSubscriber">Dependency used for subscribing to events.</param>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    /// <param name="spriteFactory">Dependency used for getting managed objects.</param>
    /// <param name="manifest">Dependency for accessing mod manifest.</param>
    /// <param name="patchManager">Dependency used for managing patches.</param>
    public RingPatcher(
        ConfigManager configManager,
        IEventSubscriber eventSubscriber,
        ILog log,
        SpriteFactory spriteFactory,
        IManifest manifest,
        IPatchManager patchManager)
        : base(configManager, eventSubscriber, log, spriteFactory, manifest, patchManager) =>
        this.Patches.Add(
            this.Id,
            new SavedPatch(
                AccessTools.DeclaredMethod(typeof(Ring), nameof(Ring.drawInMenu)),
                AccessTools.DeclaredMethod(typeof(BasePatcher), nameof(BasePatcher.DrawInMenu)),
                PatchType.Transpiler));

    /// <inheritdoc />
    public override AllPatches Type => AllPatches.PatchedRing;
}