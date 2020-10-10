/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Pong.Framework.Common;
using Pong.Framework.Menus;
using Pong.Framework.Menus.Elements;
using StardewModdingAPI;
using System.Collections.Generic;

namespace Pong.Menus
{
    internal class ServerMenu : Menu
    {
        public override void Update()
        {
            if (Context.IsWorldReady)
                this.OnSwitchToNewMenu(new PlayerMenu());
            this.InitDrawables();
        }

        public override void Resize()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<IDrawable> GetDrawables()
        {
            yield return new StaticTextElement("Server Menu!", ScreenWidth / 2, ScreenHeight / 2 - 50, true, true);
        }
    }
}
