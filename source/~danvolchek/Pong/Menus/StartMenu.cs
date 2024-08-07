/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Pong.Framework.Menus;
using Pong.Framework.Menus.Elements;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.BellsAndWhistles;
using System.Collections.Generic;
using IDrawable = Pong.Framework.Common.IDrawable;

namespace Pong.Menus
{
    internal class StartMenu : Menu
    {
        public StartMenu()
        {
            this.InitDrawables();
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="e">The event arguments.</param>
        public override bool OnButtonPressed(ButtonPressedEventArgs e)
        {
            bool result = base.OnButtonPressed(e);

            if (this.CurrentModal != null)
                return result;

            switch (e.Button)
            {
                case SButton.Escape:
                    this.OnSwitchToNewMenu(null);
                    return true;
            }

            return result;
        }

        protected override IEnumerable<IDrawable> GetDrawables()
        {
            int centerHeight = SpriteText.getHeightOfString("Single Player Multi Player");

            yield return new StaticTextElement("Pong", ScreenWidth / 2, ScreenHeight / 2 - centerHeight * 5, true, true);
            yield return new StaticTextElement("By Cat", ScreenWidth / 2, ScreenHeight / 2 - centerHeight * 4, true, true);
            yield return new StaticTextElement("Single Player", ScreenWidth / 2, ScreenHeight / 2, true, false, () => this.OnSwitchToNewMenu(new GameMenu()));
            // No multiplayer support, yet. :(
            //yield return new StaticTextElement("Multi Player", ScreenWidth / 2 + ScreenWidth / 4, ScreenHeight / 2, true, false, () => this.OnSwitchToNewMenu(new ServerMenu()));

            int escHeight = SpriteText.getHeightOfString("Press Esc to exit");
            yield return new StaticTextElement("Press Esc to exit", 15, ScreenHeight - escHeight - 15, false, false, () => this.OnSwitchToNewMenu(null));

            if (ModEntry.Instance.Helper.ModRegistry.IsLoaded("Platonymous.ArcadePong"))
            {
                int routineWidth = SpriteText.getWidthOfString("< Arcade Pong <");
                int routineHeight = SpriteText.getHeightOfString("< Arcade Pong <");
                yield return new StaticTextElement("< Arcade Pong <", ScreenWidth - routineWidth - 15, ScreenHeight - routineHeight - 15, false, true);
            }
        }

        public override void Update()
        {
        }

        public override void Resize()
        {
        }
    }
}
