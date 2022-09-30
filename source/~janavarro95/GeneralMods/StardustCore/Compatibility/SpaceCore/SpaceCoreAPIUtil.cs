/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace Omegasis.StardustCore.Compatibility.SpaceCore
{
    public static class SpaceCoreAPIUtil
    {

        public static SpaceCoreAPI GetMappedSpaceCoreAPI()
        {
            var spaceCore = ModCore.ModHelper.ModRegistry.GetApi<SpaceCoreAPI>("spacechase0.SpaceCore");
            return spaceCore;
        }

        public static void RegisterCustomEventCommand(string Command, Action<Event,GameLocation,GameTime,string[]> Method)
        {
            SpaceCoreAPI spaceCore = GetMappedSpaceCoreAPI();
            if (spaceCore == null) return;
            spaceCore.AddEventCommand(Command, Method.Method);
        }

        public static void RegisterTypeForSerializer(Type t)
        {
            SpaceCoreAPI spaceCoreAPI = GetMappedSpaceCoreAPI();
            if (spaceCoreAPI == null) return;
            spaceCoreAPI.RegisterSerializerType(t);
        }

        public static void RegisterTypesForMod(IMod modBase)
        {
            SpaceCoreAPI spaceCoreAPI = GetMappedSpaceCoreAPI();
            if (spaceCoreAPI == null) return;
            foreach (Type t in modBase.GetType().Assembly.GetTypes())
            {
                if (Attribute.GetCustomAttribute(t, typeof(XmlTypeAttribute)) != null)
                {
                    RegisterTypeForSerializer(t);
                }

            }
        }

    }
}
