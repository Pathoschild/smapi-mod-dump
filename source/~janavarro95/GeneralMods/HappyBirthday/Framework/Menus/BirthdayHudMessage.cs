/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.HappyBirthday.Framework.Menus
{
    /// <summary>
    /// An override of the <see cref="StardewValley.HUDMessage"/> to be able to draw the hud message above the <see cref="HappyBirthdayModCore"/> menu.
    /// </summary>
    public class BirthdayHudMessage : StardewValley.HUDMessage
    {
        public BirthdayHudMessage(string message):base(message)
        {
            this.message = message;
            this.color = Color.SeaGreen;
            this.timeLeft = 3500f;
        }
    }

}
