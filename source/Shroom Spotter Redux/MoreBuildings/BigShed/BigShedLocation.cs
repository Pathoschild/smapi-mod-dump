/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.Locations;

namespace MoreBuildings.BigShed
{
    public class BigShedLocation : DecoratableLocation, ISaveElement
    {
        public BigShedLocation()
        :   base("Maps\\Shed2_", "Shed2" )
        {
        }

        public override List<Rectangle> getFloors()
        {
            return new List<Rectangle> { new Rectangle(1, 3, 21, 20) };
        }

        
        public override List<Rectangle> getWalls()
        {
            return new List<Rectangle>{ new Rectangle(1, 1, 21, 3) };
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            var data = new Dictionary<string, string>();
            if (uniqueName.Value != null)
                data.Add("u", uniqueName.Value);

            return data;
        }

        public object getReplacement()
        {
            Shed shed = new Shed("Maps\\Shed2", "Shed2");
            foreach (Vector2 key in objects.Keys)
                shed.objects.Add(key, objects[key]);
            foreach (Vector2 key in terrainFeatures.Keys)
                shed.terrainFeatures.Add(key, terrainFeatures[key]);

            return shed;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Shed shed = (Shed)replacement;

            if (additionalSaveData.ContainsKey("u"))
                uniqueName.Value = additionalSaveData["u"];

            foreach (Vector2 key in shed.objects.Keys)
                objects.Add(key, shed.objects[key]);
            foreach (Vector2 key in terrainFeatures.Keys)
                terrainFeatures.Add(key, shed.terrainFeatures[key]);
        }
    }
}
