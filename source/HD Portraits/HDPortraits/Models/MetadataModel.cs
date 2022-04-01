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

namespace HDPortraits.Models
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
        public readonly RLazy<Texture2D> overrideTexture;

        internal string defaultPath = null;
        private Texture2D savedDefault = null;
        private string portraitPath = null;

        public MetadataModel()
        {
            overrideTexture = new(GetPortrait);
        }

        public void Reload()
        {
            overrideTexture.Reset();
        }

        public Texture2D GetPortrait()
        {
            Animation?.Reset();
            if (portraitPath is not null)
            {
                try
                {
                    return ModEntry.helper.Content.Load<Texture2D>(portraitPath, ContentSource.GameContent);
                }
                catch (ContentLoadException)
                {
                    ModEntry.monitor.Log("Could not find image at game asset path: '" + portraitPath + "' .", LogLevel.Warn);
                }
            }
            if (defaultPath is not null)
            {
                try
                {
                    return ModEntry.helper.Content.Load<Texture2D>(defaultPath, ContentSource.GameContent);
                }
                catch (ContentLoadException)
                {
                    ModEntry.monitor.Log("Could not find default asset at path: '" + defaultPath + "'! An NPC is missing their portrait!", LogLevel.Error);
                }
            }
            return null;
        }
        public Texture2D GetDefault()
        {
            return savedDefault;
        }
    }
}
