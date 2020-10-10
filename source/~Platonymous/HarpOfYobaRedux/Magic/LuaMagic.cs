/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using PyTK.Lua;
using StardewModdingAPI;
using System.IO;

namespace HarpOfYobaRedux
{
    class LuaMagic : IMagic
    {
        IModHelper helper;

        public LuaMagic(IModHelper helper)
        {
            this.helper = helper;
        }


        public void doMagic(bool playedToday)
        {
            PyLua.loadScriptFromFile(Path.Combine(helper.DirectoryPath, "Assets", "luamagic.lua"), "luaMagic");
            PyLua.callFunction("luaMagic", "doMagic", playedToday);
        }

       
    }
}
