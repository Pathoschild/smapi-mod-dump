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

namespace RuneMagic.Source
{
    public class Spell
    {
        public static readonly List<Spell> List = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.Namespace == "RuneMagic.Source.Spells" && t.IsSubclassOf(typeof(Spell)) && !t.IsAbstract)
            .Select(t =>
            {
                var spell = (Spell)Activator.CreateInstance(t);
                //create a variable index that is equal to the index of spell, until it reaches 10 then starts at 0

                spell.SetGlyph(RuneMagic.Textures[$"glyph_0"]);

                return spell;
            })
            .OrderBy(s => s.School?.Name) // Use the OrderBy method to sort by school name
            .ToList();

        public string Name { get; set; }
        public string Description { get; set; }
        public School School { get; set; }
        public SpellEffect Effect { get; set; }
        public int Level { get; set; } = 1;
        public float CastingTime { get; set; } = 1;
        public Texture2D Icon { get; set; }

        public Spell(School school)
        {
            Name = GetType().Name;
            School = school;
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
            Player.MagicStats.ActiveSchool.Experience += 5;
            return true;
        }

        public void SetGlyph(Texture2D originalTexture)
        {
            var glyphTexture = new Texture2D(Game1.graphics.GraphicsDevice, originalTexture.Width, originalTexture.Height);
            var data = new Color[originalTexture.Width * originalTexture.Height];
            originalTexture.GetData(data);
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i] == Color.White)
                    data[i] = School.Colors.Item1;
                if (data[i] == Color.Black)
                    data[i] = School.Colors.Item2;
            }
            glyphTexture.SetData(data);
            Icon = glyphTexture;
        }
    }
}