/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/Think-n-Talk
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace SDV_BubbleGuy.SMAPIInt
{

    /// <summary>An API provided by Farm Expansion for other mods to use.</summary>
    public interface IModApi
    {
        void SetThinkBubble(Texture2D thinkbubble);
        Texture2D GetThinkBubble();
        void SetTalkBubble(Texture2D thinkbubble);
        Texture2D GetTalkBubble();
        void ShowThinkBubble(string text);
        void ShowTalkBubble(string text);
        void ClearBubble();
    }

}
