/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using PlatoTK.Utils;
using StardewValley;

namespace PlatoTK.Lua
{
    internal class LuaHelper : InnerHelper, ILuaHelper
    {
        private readonly Dictionary<string, object> AddedGlobalObjects = new Dictionary<string, object>();
        private bool initialRegistry = false;
        public LuaHelper(IPlatoHelper helper)
            : base(helper)
        {

        }

        public void AddGlobalObject(string name, object obj)
        {
            if (AddedGlobalObjects.ContainsKey(name))
                AddedGlobalObjects[name] = obj;
            else
                AddedGlobalObjects.Add(name, obj);
        }

        private Dictionary<string,object> GetDefaultObjects()
        {
            var dict = new Dictionary<string, object>();

            dict.Add("Game1", MoonSharp.Interpreter.UserData.CreateStatic<Game1>());
            dict.Add("Utility", MoonSharp.Interpreter.UserData.CreateStatic<Utility>());
            dict.Add("Plato", new BasicUtils(Plato));
            MoonSharp.Interpreter.UserData.RegisterExtensionType(typeof(TMXTile.TMXExtensions));


            foreach (var entry in AddedGlobalObjects)
                dict.Add(entry.Key, entry.Value);

            if (!initialRegistry) {
                List<Type> types = new List<Type>();
                types.AddRange(dict.Values.SelectMany(v => v.GetType().Assembly.GetTypes()));
                types.AddRange(typeof(TMXTile.TMXTile).Assembly.GetTypes());
                types.AddRange(typeof(xTile.Tiles.Tile).Assembly.GetTypes());
                types.AddRange(typeof(Netcode.NetInt).Assembly.GetTypes());
                foreach (Type t in types)
                    try
                    {
                        if (!MoonSharp.Interpreter.UserData.GetRegisteredTypes().Contains(t))
                            MoonSharp.Interpreter.UserData.RegisterType(t, MoonSharp.Interpreter.InteropAccessMode.Reflection);
                    }
                    catch
                    {

                    }
            }

            initialRegistry = true;
            return dict;
        }

        public void CallLua(string code, Dictionary<string, object> objects = null, bool addDefaults = true)
        {
            LoadLuaCode(code, objects, addDefaults);
        }

        public MoonSharp.Interpreter.Script LoadLuaCode(string code, Dictionary<string, object> objects = null, bool addDefaults = true)
        {
            MoonSharp.Interpreter.UserData.DefaultAccessMode = MoonSharp.Interpreter.InteropAccessMode.Reflection;
            MoonSharp.Interpreter.UserData.RegistrationPolicy = new AllowAllRegistrationPolicy();


            MoonSharp.Interpreter.Script lua = new MoonSharp.Interpreter.Script();
            objects = objects ?? new Dictionary<string, object>();

            var defaults = GetDefaultObjects();

            foreach (var obj in objects)
                lua.Globals[obj.Key] = obj.Value;
            
            if (addDefaults)
                foreach (var obj in defaults)
                    lua.Globals[obj.Key] = obj.Value;

            lua.DoString(code);
            return lua;
        }

        public T CallLua<T>(string code, Dictionary<string, object> objects = null, bool addDefaults = true)
        {
                var lua = LoadLuaCode("resultValue = " + code,objects,addDefaults);
                return (T) lua.Globals["resultValue"];
        }
    }

    internal class AllowAllRegistrationPolicy : MoonSharp.Interpreter.Interop.RegistrationPolicies.AutomaticRegistrationPolicy
    {
        public override bool AllowTypeAutoRegistration(Type type)
        {
            return true;
        }
    }

}
