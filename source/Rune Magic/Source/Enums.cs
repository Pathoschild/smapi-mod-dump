/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

namespace RuneMagic.Source
{
    public enum School
    {
        Abjuration, //protects against magic (warding, protection)
        Alteration, //alters reality (teleportation, tranformation of objects)
        Conjuration, //creates objects from nothing (summons, portals)
        Evocation, //alters the threads of magic itself (fire, lightning, arcane )
    }

    public enum Duration
    { Instant, Short, Medium, Long, Permanent }
}