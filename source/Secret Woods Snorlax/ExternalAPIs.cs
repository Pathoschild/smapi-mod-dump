/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/SecretWoodsSnorlax
**
*************************************************/

using System.Reflection;

namespace JsonAssets
{
    public interface IApi
    {
        void LoadAssets(string path);
        int GetObjectId(string name);
    }
}

namespace SpaceCore
{
    public interface IApi
    {
        void AddEventCommand(string command, MethodInfo info);
    }
}
