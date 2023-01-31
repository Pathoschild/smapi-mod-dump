/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KediDili/Creaturebook
**
*************************************************/

using Microsoft.Xna.Framework;
using System;

namespace Creaturebook.Framework.Models
{
    public class Set : IEquatable<Set>
    {
        public string InternalName;

        public string DisplayNameKey;

        public int[] CreaturesBelongingToThisSet;

        public int DiscoverWithThisItem;

        public Vector2[] OffsetsInMenu;

        public float[] ScalesInMenu;
        #nullable enable
        public string? RewardItem;
        #nullable disable
        public static bool operator ==(Set right, Set left)
        {
            if (right.InternalName == left.InternalName)
                return true;
            return false;
        }

        public static bool operator !=(Set right, Set left)
        {
            if (right.InternalName == left.InternalName)
                return false;
            return true;
        }

        public bool Equals(Set other)
        {
            if (InternalName == other.InternalName)
                return true;
            return false;
        }

        public bool DetailedEquality(Set other)
        {
            if (this == other)
                if (DisplayNameKey == other.DisplayNameKey && CreaturesBelongingToThisSet == other.CreaturesBelongingToThisSet && DiscoverWithThisItem == other.DiscoverWithThisItem && OffsetsInMenu == other.OffsetsInMenu && ScalesInMenu == other.ScalesInMenu)
                    return true;
            return false;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Set);
        }
    }
}
