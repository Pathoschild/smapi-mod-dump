/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using CoreBoy.controller;
using StardewModdingAPI;

namespace GameboyArcade
{
    class GameboyController : IController, IDisposable
    {
        /// <summary>
        /// Map Event Poke data to game controls.
        /// Negative numbers release a button, and positive numbers press a button.
        /// Player control can be enabled with 9, and disabled with -9 (default is enabled).
        /// 0 ends the minigame.
        /// </summary>
        private static readonly Dictionary<int, Button> EventPokeControls = new()
        {
            // 0 is Power
            { 1, Button.Up },
            { 2, Button.Down },
            { 3, Button.Left },
            { 4, Button.Right },
            { 5, Button.A },
            { 6, Button.B },
            { 7, Button.Start },
            { 8, Button.Select },
            // 9 is enable player control
        };

        private IButtonListener Listener;
        private Dictionary<SButton, Button> Controls;
        private readonly GameboyMinigame Minigame;
        private bool DisablePlayerControls;

        public GameboyController(GameboyMinigame minigame)
        {
            this.Minigame = minigame;
        }

        public void SetButtonListener(IButtonListener listener)
        {
            this.Listener = listener;

            this.Controls = new Dictionary<SButton, Button>
            {
                {ModEntry.Config.Up, Button.Up },
                {ModEntry.Config.Down, Button.Down },
                {ModEntry.Config.Left, Button.Left },
                {ModEntry.Config.Right, Button.Right },
                {ModEntry.Config.A, Button.A },
                {ModEntry.Config.B, Button.B },
                {ModEntry.Config.Start, Button.Start },
                {ModEntry.Config.Select, Button.Select },
            };

            ModEntry.Instance.Helper.Events.Input.ButtonPressed += this.Input_ButtonPressed;
            ModEntry.Instance.Helper.Events.Input.ButtonReleased += this.Input_ButtonReleased;
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (this.DisablePlayerControls)
            {
                return;
            }
            if (this.Controls.ContainsKey(e.Button))
            {
                this.Listener.OnButtonPress(this.Controls[e.Button]);
            }
            else if (ModEntry.Config.Power.Equals(e.Button))
            {
                this.Minigame.unload();
            }
            else if (ModEntry.Config.Turbo.Equals(e.Button))
            {
                this.Minigame.TurboToggle();
            }
        }

        private void Input_ButtonReleased(object sender, StardewModdingAPI.Events.ButtonReleasedEventArgs e)
        {
            if (this.DisablePlayerControls)
            {
                return;
            }
            if (this.Controls.ContainsKey(e.Button))
            {
                this.Listener.OnButtonRelease(this.Controls[e.Button]);
            }
        }

        public void ReceiveEventPoke(int data)
        {
            int absData = Math.Abs(data);
            if (EventPokeControls.ContainsKey(absData))
            {
                if (data < 0)
                {
                    this.Listener.OnButtonRelease(EventPokeControls[absData]);
                }
                else
                {
                    this.Listener.OnButtonPress(EventPokeControls[absData]);
                }
            }
            else if (data == 9)
            {
                this.DisablePlayerControls = false;
            }
            else if (data == -9)
            {
                this.DisablePlayerControls = true;
                foreach (Button button in this.Controls.Values)
                {
                    this.Listener.OnButtonRelease(button);
                }
            }
            else if (data == 0)
            {
                this.Minigame.unload();
            }
        }

        public void Dispose()
        {
            ModEntry.Instance.Helper.Events.Input.ButtonPressed -= this.Input_ButtonPressed;
            ModEntry.Instance.Helper.Events.Input.ButtonReleased -= this.Input_ButtonReleased;
        }
    }
}
