/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using SAML.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Utilities
{
    /// <summary>
    /// A wrapper class for <see cref="IKeyboardSubscriber"/>
    /// </summary>
    /// <remarks>
    /// Use this for textbox or chatbox will open and force focus
    /// </remarks>
    internal class DummyKeyboardSubscriber : IKeyboardSubscriber
    {
        public bool Selected
        {
            get => true;
            set { }
        }

        public void RecieveCommandInput(char command)
        {
        }

        public void RecieveSpecialInput(Keys key)
        {
        }

        public void RecieveTextInput(char inputChar)
        {
        }

        public void RecieveTextInput(string text)
        {
        }
    }
}
