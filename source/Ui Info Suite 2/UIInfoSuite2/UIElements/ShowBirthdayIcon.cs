/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UiInfoSuite2
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using UIInfoSuite2.Infrastructure;
using UIInfoSuite2.Infrastructure.Extensions;

namespace UIInfoSuite2.UIElements
{
    internal class ShowBirthdayIcon : IDisposable
    {
        #region Properties
        private readonly PerScreen<List<NPC>> _birthdayNPCs = new(() => new());
        private readonly PerScreen<List<ClickableTextureComponent>> _birthdayIcons = new(() => new());

        private bool Enabled { get; set; }
        private bool HideBirthdayIfFullFriendShip { get; set; }
        private readonly IModHelper _helper;
        #endregion


        #region Life cycle
        public ShowBirthdayIcon(IModHelper helper)
        {
            _helper = helper;
        }

        public void Dispose()
        {
            ToggleOption(false);
        }

        public void ToggleOption(bool showBirthdayIcon)
        {
            Enabled = showBirthdayIcon;

            _helper.Events.GameLoop.DayStarted -= OnDayStarted;
            _helper.Events.Display.RenderingHud -= OnRenderingHud;
            _helper.Events.Display.RenderedHud -= OnRenderedHud;
            _helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;

            if (showBirthdayIcon)
            {
                CheckForBirthday();
                _helper.Events.GameLoop.DayStarted += OnDayStarted;
                _helper.Events.Display.RenderingHud += OnRenderingHud;
                _helper.Events.Display.RenderedHud += OnRenderedHud;
                _helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            }
        }

        public void ToggleDisableOnMaxFriendshipOption(bool hideBirthdayIfFullFriendShip)
        {
            HideBirthdayIfFullFriendShip = hideBirthdayIfFullFriendShip;
            ToggleOption(Enabled);
        }

        #endregion


        #region Event subscriptions
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (e.IsOneSecond)
            {
                CheckForGiftGiven();
            }
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            CheckForBirthday();
        }

        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            if (!Game1.eventUp)
            {
                DrawBirthdayIcon();
            }
        }


        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            if (!Game1.eventUp)
            {
                DrawHoverText();
            }
        }
        #endregion


        #region Logic
        private void CheckForGiftGiven()
        {
            List<NPC> npcs = _birthdayNPCs.Value;
            // Iterate from the end so that removing items doesn't affect indices
            for (int i = npcs.Count - 1; i >= 0; i--)
            {
                Friendship? friendship = GetFriendshipWithNPC(npcs[i].Name);
                if (friendship != null && friendship.GiftsToday > 0)
                {
                    npcs.RemoveAt(i);
                }
            }
        }

        private void CheckForBirthday()
        {
            _birthdayNPCs.Value.Clear();
            foreach (var location in Game1.locations)
            {
                foreach (var character in location.characters)
                {
                    if (character.isBirthday(Game1.currentSeason, Game1.dayOfMonth))
                    {
                        Friendship? friendship = GetFriendshipWithNPC(character.Name);
                        if (friendship != null)
                        {
                            if (HideBirthdayIfFullFriendShip && friendship.Points >= Utility.GetMaximumHeartsForCharacter(character) * NPC.friendshipPointsPerHeartLevel)
                                continue;

                            _birthdayNPCs.Value.Add(character);
                        }

                    }
                }
            }
        }

        private static Friendship? GetFriendshipWithNPC(string name)
        {
            try
            {
                if (Game1.player.friendshipData.TryGetValue(name, out Friendship friendship))
                    return friendship;
                else
                    return null;
            }
            catch (Exception ex)
            {
                ModEntry.MonitorObject.LogOnce("Error while getting information about the birthday of " + name, LogLevel.Error);
                ModEntry.MonitorObject.Log(ex.ToString());
            }

            return null;
        }

        private void DrawBirthdayIcon()
        {
            _birthdayIcons.Value.Clear();
            foreach (var npc in _birthdayNPCs.Value)
            {
                Rectangle headShot = npc.GetHeadShot();
                Point iconPosition = IconHandler.Handler.GetNewIconPosition();
                float scale = 2.9f;

                Game1.spriteBatch.Draw(
                    Game1.mouseCursors,
                    new Vector2(iconPosition.X, iconPosition.Y),
                    new Rectangle(228, 409, 16, 16),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    1f);

                var birthdayIcon = new ClickableTextureComponent(
                    npc.Name,
                    new Rectangle(
                        iconPosition.X - 7,
                        iconPosition.Y - 2,
                        (int)(16.0 * scale),
                        (int)(16.0 * scale)),
                    null,
                    npc.Name,
                    npc.Sprite.Texture,
                    headShot,
                    2f);

                birthdayIcon.draw(Game1.spriteBatch);
                _birthdayIcons.Value.Add(birthdayIcon);
            }
        }

        private void DrawHoverText()
        {
            var icons = _birthdayIcons.Value;
            var npcs = _birthdayNPCs.Value;
            if (icons.Count != npcs.Count)
            {
                ModEntry.MonitorObject.LogOnce($"{this.GetType().Name}: The number of tracked npcs and icons do not match", LogLevel.Error);
                return;
            }

            for (int i = 0; i < icons.Count; i++)
            {
                if (icons[i].containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                {
                    string hoverText = string.Format(_helper.SafeGetString(LanguageKeys.NpcBirthday), npcs[i].displayName);
                    IClickableMenu.drawHoverText(
                        Game1.spriteBatch,
                        hoverText,
                        Game1.dialogueFont);
                }
            }
        }
        #endregion
    }
}
