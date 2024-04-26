/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

namespace NermNermNerm.Junimatic
{
    public enum JunimoType
    {
        /* Careful with the ordering, there's code in GameMachine.IsCompatibleWithJunimo that depends on the order */

        MiningProcessing,
        Animals,
        CropProcessing,
        Fishing,
        Forestry
    };
}
