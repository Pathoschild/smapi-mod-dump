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

namespace FashionSense.Framework.Models.Appearances.Generic
{
    public class DrawOrder
    {
        public enum Order
        {
            Unknown,
            Before,
            After
        }

        public Order Preposition { get; set; }
        public IApi.Type AppearanceType { get; set; }

        public bool IsValid()
        {
            return Preposition is not Order.Unknown && AppearanceType is not IApi.Type.Unknown;
        }
    }
}
