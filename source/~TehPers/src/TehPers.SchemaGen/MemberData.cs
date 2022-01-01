/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Namotion.Reflection;

namespace TehPers.FishingOverhaul.SchemaGen
{
    internal record MemberData(
        ContextualAccessorInfo Accessor,
        bool IsStatic,
        bool IsFullyPublic
    )
    {
        public MemberData(ContextualFieldInfo field)
            : this(field, field.FieldInfo.IsStatic, field.FieldInfo.IsPublic)
        {
        }

        public MemberData(ContextualPropertyInfo property)
            : this(
                property,
                property.PropertyInfo.GetMethod?.IsStatic is true
                || property.PropertyInfo.SetMethod?.IsStatic is true,
                property.PropertyInfo.GetMethod?.IsPublic is not false
                && property.PropertyInfo.SetMethod?.IsPublic is not false
            )
        {
        }
    }
}