/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

global using DaLion.Core.Framework.Extensions;
global using DaLion.Shared.Reflection;
global using static DaLion.Core.CoreMod;

namespace DaLion.Core;

#region using directives

using System.Reflection;
using DaLion.Shared;
using DaLion.Shared.Commands;
using DaLion.Shared.Data;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;
using DaLion.Shared.Networking;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class CoreMod : Mod
{
    /// <summary>Gets the static <see cref="CoreMod"/> instance.</summary>
    public static CoreMod Instance { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="CoreConfig"/> instance.</summary>
    public static CoreConfig Config { get; set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="PerScreen{T}"/> <see cref="CoreState"/>.</summary>
    public static PerScreen<CoreState> PerScreenState { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="CoreState"/> of the local player.</summary>
    public static CoreState State
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    /// <summary>Gets the <see cref="Shared.Events.EventManager"/> instance.</summary>
    public static EventManager EventManager { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="ModDataManager"/> instance.</summary>
    internal static ModDataManager Data { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Broadcaster"/> instance.</summary>
    internal static Broadcaster Broadcaster { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Logger"/> instance.</summary>
    internal static Logger Log { get; private set; } = null!; // set in Entry;

    /// <summary>Gets the <see cref="IModHelper"/> API.</summary>
    internal static IModHelper ModHelper => Instance.Helper;

    /// <summary>Gets the <see cref="IManifest"/> API.</summary>
    internal static IManifest Manifest => Instance.ModManifest;

    /// <summary>Gets the unique ID for this mod.</summary>
    internal static string UniqueId => Manifest.UniqueID;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;
        Log = new Logger(this.Monitor);

        // pseudo-DRM for low-effort theft
        if (Manifest.Author != "DaLion" || UniqueId != this.GetType().Namespace)
        {
            Log.W(
                "Woops, looks like you downloaded a clandestine version of this mod! Please make sure to download from the official mod page at XXX.");
            return;
        }

        var assembly = Assembly.GetExecutingAssembly();
        Config = helper.ReadConfig<CoreConfig>();
        PerScreenState = new PerScreen<CoreState>(() => new CoreState());
        EventManager = new EventManager(helper.Events, helper.ModRegistry, Log).ManageInitial(assembly);
        Data = new ModDataManager(UniqueId, Log);
        Broadcaster = new Broadcaster(helper.Multiplayer, UniqueId);
        Harmonizer.ApplyAll(assembly, helper.ModRegistry, Log, UniqueId);
        CommandHandler.HandleAll(
            assembly,
            helper.ConsoleCommands,
            Log,
            UniqueId,
            "ars");
        this.ValidateMultiplayer();
    }
}
