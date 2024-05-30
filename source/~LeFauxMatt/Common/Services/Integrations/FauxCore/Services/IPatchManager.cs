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
namespace StardewMods.FauxCore.Common.Services.Integrations.FauxCore;
#else
namespace StardewMods.Common.Services.Integrations.FauxCore;
#endif

/// <summary>Manages Harmony patches.</summary>
public interface IPatchManager
{
    /// <summary>Adds a patch to the specified id.</summary>
    /// <param name="id">The id to associate the patch with.</param>
    /// <param name="patches">The patch object to add.</param>
    public void Add(string id, params ISavedPatch[] patches);

    /// <summary>Applies the specified patches.</summary>
    /// <param name="id">The id of saved patches.</param>
    public void Patch(string id);

    /// <summary>Removes the specified patches.</summary>
    /// <param name="id">The id of the saved patches.</param>
    public void Unpatch(string id);
}