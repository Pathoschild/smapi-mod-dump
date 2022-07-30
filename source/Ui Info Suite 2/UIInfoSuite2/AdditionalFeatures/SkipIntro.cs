/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UiInfoSuite2
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace UIInfoSuite2.AdditionalFeatures
{
    public class SkipIntro
    {
        private readonly IModEvents _events;

        public SkipIntro(IModEvents events)
        {
            _events = events;

            events.Input.ButtonPressed += OnButtonPressed;
            events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private void OnSaveLoaded(object sender, EventArgs e)
        {
            _events.Input.ButtonPressed -= OnButtonPressed;
            _events.GameLoop.SaveLoaded -= OnSaveLoaded;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.activeClickableMenu is TitleMenu menu && (e.Button == SButton.Escape || e.Button == SButton.ControllerStart))
            {
                menu.skipToTitleButtons();
                _events.Input.ButtonPressed -= OnButtonPressed;
            }
        }
    }
}
