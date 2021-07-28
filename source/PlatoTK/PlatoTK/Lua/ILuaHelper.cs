/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System.Collections.Generic;

namespace PlatoTK.Lua
{
    public interface ILuaHelper
    {
        void CallLua(string code, Dictionary<string, object> objects = null, bool addDefaults = true);

        T CallLua<T>(string code, Dictionary<string, object> objects = null, bool addDefaults = true);

        MoonSharp.Interpreter.Script LoadLuaCode(string code, Dictionary<string, object> objects = null, bool addDefaults = true);

        void AddGlobalObject(string name, object obj);
    }
}