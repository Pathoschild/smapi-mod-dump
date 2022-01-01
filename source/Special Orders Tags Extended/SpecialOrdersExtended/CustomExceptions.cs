/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/SpecialOrdersExtended
**
*************************************************/

namespace SpecialOrdersExtended;

public class UnexpectedEnumValueException<T> : Exception
{
    public UnexpectedEnumValueException(T value)
        : base($"Enum {typeof(T).Name} recieved unexpected value {value}")
    {
    }
}

public class SaveNotLoadedError : Exception
{
    public SaveNotLoadedError() :
        base("Save not loaded")
    {
    }
}
