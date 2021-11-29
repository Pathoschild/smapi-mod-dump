/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Models.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionSense.Framework.Models.Accessory
{
    public class AccessoryModel : AppearanceModel
    {
        internal enum Type
        {
            Unknown,
            Primary,
            Secondary,
            Tertiary
        }

        public Position HeadPosition { get; set; } = new Position() { X = 0, Y = 0 };
        public Size AccessorySize { get; set; }
        public bool DrawBeforeHair { get; set; }
        public bool DrawAfterPlayer { get; set; }

        // Old property, has been renamed to DrawAfterPlayer
        [Obsolete("Has been renamed to DrawAfterPlayer.")]
        public bool DrawBeforePlayer { set { DrawAfterPlayer = value; } }

        internal Type Priority { get; set; }
    }
}
