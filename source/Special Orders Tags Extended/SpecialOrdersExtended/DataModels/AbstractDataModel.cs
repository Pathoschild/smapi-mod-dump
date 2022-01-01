/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/SpecialOrdersExtended
**
*************************************************/

namespace SpecialOrdersExtended.DataModels;

internal abstract class AbstractDataModel
{
    public string Savefile { get; set; }

    public virtual void Save(string identifier)
    {
        ModEntry.DataHelper.WriteGlobalData(Savefile + identifier, this);
    }

}
