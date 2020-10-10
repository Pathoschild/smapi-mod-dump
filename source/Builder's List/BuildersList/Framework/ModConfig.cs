/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bmarquismarkail/SV_BuildersList
**
*************************************************/

using System;
namespace SB_Builderslist
{
    public class ModConfig
    {
        public bool isActive { get; set; } = true;
        public string currentRecipe { get; set; } = null;
        public bool isCooking { get; set; } = false;

    }
}
