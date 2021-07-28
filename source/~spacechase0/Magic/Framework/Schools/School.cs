/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Magic.Framework.Spells;

namespace Magic.Framework.Schools
{
    internal class School
    {
        /*********
        ** Fields
        *********/
        private static Dictionary<string, School> Schools;


        /*********
        ** Accessors
        *********/
        public string Id { get; }


        /*********
        ** Public methods
        *********/
        public virtual Spell[] GetSpellsTier1() { return new Spell[0]; }
        public virtual Spell[] GetSpellsTier2() { return new Spell[0]; }
        public virtual Spell[] GetSpellsTier3() { return new Spell[0]; }

        public static void RegisterSchool(School school)
        {
            if (School.Schools == null)
                School.Init();

            School.Schools.Add(school.Id, school);
        }

        public static School GetSchool(string id)
        {
            if (School.Schools == null)
                School.Init();

            return School.Schools[id];
        }

        public static ICollection<string> GetSchoolList()
        {
            if (School.Schools == null)
                School.Init();

            return School.Schools.Keys;
        }


        /*********
        ** Protected methods
        *********/
        protected School(string id)
        {
            this.Id = id;
        }

        private static void Init()
        {
            School.Schools = new Dictionary<string, School>();
            School.RegisterSchool(new ArcaneSchool());
            School.RegisterSchool(new ElementalSchool());
            School.RegisterSchool(new NatureSchool());
            School.RegisterSchool(new LifeSchool());
            School.RegisterSchool(new EldritchSchool());
            School.RegisterSchool(new ToilSchool());
        }
    }
}
