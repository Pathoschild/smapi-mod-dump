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
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RuneMagic.Source
{
    [XmlType("Mods_Skill")]
    public class Skill
    {
        public string Name { get; set; }
        public School School { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public Texture2D Icon { get; set; }
        public Tuple<Color, Color> Colors { get; set; }

        public Skill()
        { }

        public Skill(School school)
        {
            Name = school.ToString();
            School = school;
            Icon = RuneMagic.Textures[$"icon_{school.ToString().ToLower()}"];
            Experience = 0;
            Level = 0;
            switch (school)
            {
                case School.Abjuration:
                    Description = "";
                    Colors = new(new Color(200, 200, 200), new Color(175, 175, 175));
                    break;

                case School.Alteration:
                    Description = "";
                    Colors = new(new Color(0, 0, 200), new Color(0, 0, 175));
                    break;

                case School.Conjuration:
                    Description = "";
                    Colors = new(Color.Orange, Color.DarkOrange);
                    break;

                case School.Evocation:
                    Description = "";
                    Colors = new(new Color(200, 0, 0), new Color(175, 0, 0));
                    break;
            }
        }
    }
}