/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Services;
#else
namespace StardewMods.Common.Services;
#endif

/// <summary>This abstract class serves as the base for all service classes.</summary>
/// <typeparam name="TService">The service type.</typeparam>
internal abstract class BaseService<TService>
    where TService : BaseService<TService>
{
    /// <summary>Initializes a new instance of the <see cref="BaseService{TService}" /> class.</summary>
    protected BaseService()
    {
        this.Id = typeof(TService).Name;
        this.UniqueId = Mod.Id + "/" + this.Id;
        this.Prefix = Mod.Id + "-" + this.Id + "-";
    }

    /// <summary>Gets a unique id for this service.</summary>
    protected string Id { get; }

    /// <summary>Gets a globally unique prefix for this service.</summary>
    protected string Prefix { get; }

    /// <summary>Gets a globally unique id for this service.</summary>
    protected string UniqueId { get; }
}