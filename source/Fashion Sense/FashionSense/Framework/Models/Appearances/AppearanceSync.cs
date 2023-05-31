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

namespace FashionSense.Framework.Models.Appearances
{
    public class AppearanceSync
    {
        public IApi.Type TargetAppearanceType { get; set; }
        public AnimationModel.Type AnimationType { get; set; }
    }
}
