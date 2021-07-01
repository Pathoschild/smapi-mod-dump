/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace CookingSkill
{
    class Util
    {
        // http://stackoverflow.com/a/22456034
        public static string serialize< T >( T obj )
        {
            using ( MemoryStream stream = new MemoryStream() )
            {
                XmlSerializer serializer = new XmlSerializer( obj.GetType() );
                serializer.Serialize(stream, obj);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        // http://stackoverflow.com/questions/3303126/how-to-get-the-value-of-private-field-in-c
        public static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }
    }
}
