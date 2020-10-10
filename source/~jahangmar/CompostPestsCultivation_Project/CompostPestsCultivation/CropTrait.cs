/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

using System;
namespace CompostPestsCultivation
{
    public enum CropTrait
    {
        PestResistanceNo, //for SeedsInfoMenu
        PestResistanceI,
        PestResistanceII,

        QualityNo, //for SeedsInfoMenu
        QualityI,
        QualityII,

        WaterNo, //for SeedsInfoMenu
        WaterI,
        WaterII,
        WaterIII, //for SeedsInfoMenu (with compost)

        SpeedNo, //for SeedsInfoMenu
        SpeedI,
        SpeedII,
        SpeedIII //for SeedsInfoMenu (with compost)
    }
}
