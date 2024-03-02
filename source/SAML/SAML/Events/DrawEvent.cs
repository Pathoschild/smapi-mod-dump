/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Events
{
    public delegate void DrawEventHandler(object sender, DrawEventArgs e);

    public class DrawEventArgs(SpriteBatch batch) : EventArgs
    {
        /// <summary>
        /// Framework helper class to draw text and sprites to the screen
        /// </summary>
        public SpriteBatch Batch { get; } = batch;
    }
}
