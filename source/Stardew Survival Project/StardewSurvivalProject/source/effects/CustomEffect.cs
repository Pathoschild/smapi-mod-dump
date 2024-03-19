/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSurvivalProject.source.effects
{
    public class CustomEffect
    {
        public string id;

        public string displayName;

        public string description;

        public int duration;

        public Texture2D iconTexture;

        public BuffEffects effects;

        public bool isDebuff = false;

        public CustomEffect(string id, string displayName, string description, Texture2D iconTexture, int duration, BuffEffects effects = null, bool isDebuff = false)
        {
            this.id = id;
            this.displayName = displayName;
            this.description = description;
            this.iconTexture = iconTexture;
            this.duration = duration;
            this.effects = effects;
            this.isDebuff = isDebuff;
        }
    }
}
