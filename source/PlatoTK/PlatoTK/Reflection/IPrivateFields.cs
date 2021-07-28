/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

namespace PlatoTK.Reflection
{
    public interface IPrivateFields
    {
        object this[string name] { get; set; }

        T Get<T>(string name);
        void Set<T>(string name, T value);

    }
}
