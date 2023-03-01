/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static SpaceCore.Skills;

namespace RuneMagic.Source
{
    public class Spell
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Skill Skill { get; set; }
        public School School { get; set; }
        public dynamic Target { get; set; }
        public SpellEffect Effect { get; set; }
        public int Level { get; set; } = 1;
        public float CastingTime { get; set; } = 1;
        public Texture2D Icon { get; set; }

        public Spell(School school)
        {
            Name = GetType().Name;
            CastingTime = 1 + (Level / 10f) * 1.5f;
            School = school;
            Skill = RuneMagic.PlayerStats.Skills[School];
            SetGlyph(RuneMagic.Textures["glyph_0"]);
            if (GetType().GetMethod(nameof(Cast)).DeclaringType == typeof(Spell))
            {
                Description = "NOT IMPLEMENTED.\n";
            }
            else
            {
                Description = "";
            }
        }

        public virtual bool Cast()
        {
            RuneMagic.PlayerStats.Skills[School].Experience += 5;
            return true;
        }

        public void SetGlyph(Texture2D texture)
        {
            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i] == Color.White)
                    data[i] = Skill.Colors.Item1;
                if (data[i] == Color.Black)
                    data[i] = Skill.Colors.Item2;
            }
            texture.SetData(data);
            Icon = texture;
        }
    }
}