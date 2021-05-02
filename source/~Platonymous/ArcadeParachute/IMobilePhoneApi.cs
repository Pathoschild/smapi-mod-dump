/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ArcadeParachute
{
    public interface IMobilePhoneApi
    {
        bool AddApp(string id, string name, Action action, Texture2D icon);

        Vector2 GetScreenPosition();
        Vector2 GetScreenSize();
        bool GetPhoneRotated();
        void SetPhoneRotated(bool value);
        bool GetPhoneOpened();
        void SetPhoneOpened(bool value);
        bool GetAppRunning();
        void SetAppRunning(bool value);
    }
}
