/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Interfaces.API;
using FashionSense.Framework.Models.Appearances;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FashionSense.Framework.Models.General
{
    internal class LayerData
    {
        public IApi.Type AppearanceType { get; set; }
        public AppearanceModel AppearanceModel { get; set; }
        public List<Color> Colors { get; set; }
        public bool IsVanilla { get; set; }
        public bool IsHidden { get; set; }

        public LayerData(IApi.Type type, AppearanceModel model, bool isVanilla = false)
        {
            AppearanceType = type;
            AppearanceModel = model;
            IsVanilla = isVanilla;
        }
    }
}
