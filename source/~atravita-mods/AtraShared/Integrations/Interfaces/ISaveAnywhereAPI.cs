/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Integrations.Interfaces;

/// <summary>
/// The API for Save Anywhere.
/// </summary>
/// <remarks>So. I actually grabbed this out of Solid Foundations and have no clue what any of this means.</remarks>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "API was like this.")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "API was like this.")]
public interface ISaveAnywhereApi
{
    event EventHandler BeforeSave;

    event EventHandler AfterSave;

    event EventHandler AfterLoad;

    void addBeforeSaveEvent(string ID, Action BeforeSave);

    void addAfterLoadEvent(string ID, Action AfterLoad);
}
