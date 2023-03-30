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
using System;
using System.Collections.Generic;

using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RuneMagic.Source
{
    public class School
    {
        public static readonly School Abjuration = new("Abjuration") { Colors = new(new Color(200, 200, 200), new Color(175, 175, 175)) };
        public static readonly School Alteration = new("Alteration") { Colors = new(new Color(0, 0, 200), new Color(0, 0, 175)) };
        public static readonly School Conjuration = new("Conjuration") { Colors = new(new Color(200, 0, 200), new Color(175, 0, 175)) };
        public static readonly School Evocation = new("Evocation") { Colors = new(new Color(200, 0, 0), new Color(175, 0, 0)) };

        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; } = 0;
        public Texture2D Icon { get; set; }
        public Tuple<Color, Color> Colors { get; set; }

        public School(string name)
        {
            Name = name;
            Icon = RuneMagic.Textures[$"icon_{Name.ToString().ToLower()}"];
            Experience = 0;
            Level = 0;
        }

        public static IEnumerable<School> All
        {
            get
            {
                yield return Abjuration;
                yield return Alteration;
                yield return Conjuration;
                yield return Evocation;
            }
        }

        public void SetLevel()
        {
            if (Experience < 100)
                Level = 0;
            else if (Experience < 300)
                Level = 1;
            else if (Experience < 600)
                Level = 2;
            else if (Experience < 1000)
                Level = 3;
            else if (Experience < 1500)
                Level = 4;
            else if (Experience < 2100)
                Level = 5;
            else if (Experience < 2800)
                Level = 6;
            else if (Experience < 3600)
                Level = 7;
            else if (Experience < 4500)
                Level = 8;
            else if (Experience < 5500)
                Level = 9;
            else if (Experience < 6600)
                Level = 10;
            else if (Experience < 7800)
                Level = 11;
            else if (Experience < 9100)
                Level = 12;
            else if (Experience < 10500)
                Level = 13;
            else if (Experience < 12000)
                Level = 14;
            else if (Experience >= 12000)
                Level = 15;
        }
    }
}