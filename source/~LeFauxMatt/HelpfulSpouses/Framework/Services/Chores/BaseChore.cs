/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.HelpfulSpouses.Framework.Services.Chores;

using StardewMods.Common.Services;
using StardewMods.Common.Services.Integrations.FuryCore;
using StardewMods.HelpfulSpouses.Framework.Enums;
using StardewMods.HelpfulSpouses.Framework.Interfaces;

/// <inheritdoc cref="StardewMods.HelpfulSpouses.Framework.Interfaces.IChore" />
internal abstract class BaseChore<TChore> : BaseService<TChore>, IChore
    where TChore : class, IChore
{
    /// <summary>Initializes a new instance of the <see cref="BaseChore{TChore}" /> class.</summary>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    /// <param name="manifest">Dependency for accessing mod manifest.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    protected BaseChore(ILog log, IManifest manifest, IModConfig modConfig)
        : base(log, manifest) =>
        this.Config = modConfig;

    /// <summary>Gets the dependency used for accessing config data.</summary>
    protected IModConfig Config { get; }

    /// <inheritdoc />
    public abstract ChoreOption Option { get; }

    /// <inheritdoc />
    public virtual void AddTokens(Dictionary<string, object> tokens) { }

    /// <inheritdoc />
    public virtual bool IsPossibleForSpouse(NPC spouse) => false;

    /// <inheritdoc />
    public virtual bool TryPerformChore(NPC spouse) => false;
}