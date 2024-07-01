/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace MobilePhone
{
    public class MobilePhoneApi
    {
        public event EventHandler<StardewModdingAPI.Events.RenderedWorldEventArgs> OnBeforeRenderScreen;
        public event EventHandler<StardewModdingAPI.Events.RenderedWorldEventArgs> OnAfterRenderScreen;

        public MobilePhoneApi()
        {
            PhoneVisuals.OnBeforeRenderScreen += CallBeforeRenderEvent;
            PhoneVisuals.OnAfterRenderScreen += CallAfterRenderEvent;
        }

        ~MobilePhoneApi()
        {
            PhoneVisuals.OnBeforeRenderScreen -= CallBeforeRenderEvent;
            PhoneVisuals.OnAfterRenderScreen -= CallAfterRenderEvent;
        }

        private void CallBeforeRenderEvent(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            this.OnBeforeRenderScreen?.Invoke(sender, e);
        }

        private void CallAfterRenderEvent(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            this.OnAfterRenderScreen?.Invoke(sender, e);
        }

        public bool AddApp(string id, string name, Action action, Texture2D icon)
        {
            if (ModEntry.apps.ContainsKey(name))
            {
                return false;
            }
            ModEntry.apps[id] = new MobileApp(name, action, icon);
            return true;
        }

        public Vector2 GetRawScreenPosition()
        {
            return PhoneUtils.GetScreenPosition();
        }

        public Vector2 GetRawScreenSize()
        {
            return PhoneUtils.GetScreenSize();
        }

        public Vector2 GetRawScreenSize(bool rotated)
        {
            return PhoneUtils.GetScreenSize(rotated);
        }

        public Rectangle GetRawPhoneRectangle()
        {
            return ModEntry.phoneRect;
        }

        public Rectangle GetRawScreenRectangle()
        {
            return ModEntry.screenRect;
        }

        public Vector2 GetScreenPosition()
        {
            return GetRawScreenPosition() * GetUIScale();
        }

        public Vector2 GetScreenSize()
        {
            return GetRawScreenSize() * GetUIScale();
        }

        public Vector2 GetScreenSize(bool rotated)
        {
            return GetRawScreenSize(rotated) * GetUIScale();
        }

        public Rectangle GetPhoneRectangle()
        {
            return PhoneUtils.ScaleRect(GetRawPhoneRectangle(), GetUIScale());
        }

        public Rectangle GetScreenRectangle()
        {
            return PhoneUtils.ScaleRect(GetRawScreenRectangle(), GetUIScale());
        }

        public bool AddOnPhoneRotated(EventHandler action)
        {
            ModEntry.OnScreenRotated += action;
            return true;
        }

        public float GetUIScale()
        {
            return Game1.options.zoomLevel != 1f ? 1f : 1f / Game1.options.uiScale;
        }

        public Texture2D GetBackgroundTexture(bool rotated)
        {
            return rotated ? ModEntry.backgroundRotatedTexture : ModEntry.backgroundTexture;
        }

        public bool GetPhoneRotated()
        {
            return ModEntry.phoneRotated;
        }

        public void SetPhoneRotated(bool value)
        {
            ModEntry.phoneRotated = value;
        }

        public bool GetPhoneOpened()
        {
            return ModEntry.phoneOpen;
        }

        public void SetPhoneOpened(bool value)
        {
            PhoneUtils.TogglePhone(value);
        }

        public bool GetAppRunning()
        {
            return ModEntry.appRunning;
        }

        public void SetAppRunning(bool value)
        {
            ModEntry.appRunning = value;
            if (!value)
            {
                ModEntry.runningApp = null;
            }
        }

        public string GetRunningApp()
        {
            return ModEntry.runningApp;
        }

        public void SetRunningApp(string value)
        {
            ModEntry.runningApp = value;
        }

        public void PlayRingTone()
        {
            PhoneUtils.PlayRingTone();
        }

        public void PlayNotificationTone()
        {
            PhoneUtils.PlayNotificationTone();
        }

        public NPC GetCallingNPC()
        {
            return ModEntry.callingNPC;
        }

        public bool IsCallingNPC()
        {
            return ModEntry.callingNPC != null;
        }
    }
}