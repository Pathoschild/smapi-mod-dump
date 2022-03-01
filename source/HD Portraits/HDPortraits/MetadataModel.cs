/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HDPortraits
**
*************************************************/

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;

namespace HDPortraits
{
    public class MetadataModel
    {
        public int Size { set; get; } = 64;
        public AnimationModel Animation { get; set; } = null;
        public string Portrait { 
            get { return portraitPath; }
            set {
                portraitPath = value;
                Reload();
            }
        }
        public Texture2D overrideTexture
        {
            get {
                return stored;
            }
        }
        private Texture2D stored = null;
        private string portraitPath = null;
        public void Reload()
        {
            Animation?.Reset();
            if (portraitPath != null)
            {
                try
                {
                    stored = ModEntry.helper.Content.Load<Texture2D>(portraitPath, ContentSource.GameContent);
                }
                catch (ContentLoadException e)
                {
                    ModEntry.monitor.Log("Could not find image at game asset path: '" + portraitPath + "' .", LogLevel.Warn);
                    ModEntry.monitor.Log(e.StackTrace, LogLevel.Warn);
                }
            }
        }
    }
}
