/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Magic.Spells;

namespace Magic.Schools
{
    public class School
    {
        public string Id { get; }

        public virtual Spell[] GetSpellsTier1() { return new Spell[0]; }
        public virtual Spell[] GetSpellsTier2() { return new Spell[0]; }
        public virtual Spell[] GetSpellsTier3() { return new Spell[0]; }

        protected School( string id )
        {
            Id = id;
        }

        private static Dictionary<string, School> schools;
        public static void registerSchool( School school )
        {
            if (schools == null)
                init();

            schools.Add(school.Id, school);
        }

        public static School getSchool( string id )
        {
            if (schools == null)
                init();

            return schools[id];
        }

        public static ICollection< string > getSchoolList()
        {
            if (schools == null)
                init();

            return schools.Keys;
        }

        private static void init()
        {
            schools = new Dictionary<string, School>();
            registerSchool(new ArcaneSchool());
            registerSchool(new ElementalSchool());
            registerSchool(new NatureSchool());
            registerSchool(new LifeSchool());
            registerSchool(new EldritchSchool());
            registerSchool(new ToilSchool());
        }
    }
}
