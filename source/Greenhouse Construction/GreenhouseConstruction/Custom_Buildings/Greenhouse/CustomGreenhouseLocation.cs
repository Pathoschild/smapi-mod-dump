/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/derslayr10/GreenhouseConstruction
**
*************************************************/

using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.Buildings;

namespace GreenhouseConstruction.Custom_Buildings.Greenhouse
{

    [XmlType("Mods_Derslayr_CustomGreenhouseLocation")]

    public class CustomGreenhouseLocation : GameLocation, ISaveElement
    {

        public CustomGreenhouseLocation() : base("Maps\\Greenhouse", "GreenhouseConstruction_SpecialGreenhouse") {

            this.IsGreenhouse = true;
        
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            var data = new Dictionary<string, string>();
            if (this.uniqueName.Value != null) {

                data.Add("greenhouse_construction", this.uniqueName.Value);
            
            }

            return data;

        }

        public object getReplacement()
        {

            Shed custom_Greenhouse = new Shed("Maps\\Greenhouse", "GreenhouseConstruction_SpecialGreenhouse");
            foreach (Vector2 key in this.objects.Keys) {

                custom_Greenhouse.objects.Add(key, this.objects[key]);
            
            }

            foreach (Vector2 key in this.terrainFeatures.Keys)
            {

                custom_Greenhouse.terrainFeatures.Add(key, this.terrainFeatures[key]);

            }

            return custom_Greenhouse;

        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {

            Shed custom_Greenhouse = (Shed)replacement;

            if (additionalSaveData.TryGetValue("greenhouse_construction", out string savedName)) {

                this.uniqueName.Value = savedName;
            
            }

            foreach (Vector2 key in custom_Greenhouse.objects.Keys) {

                this.objects.Add(key, custom_Greenhouse.objects[key]);
            
            }

            foreach (Vector2 key in custom_Greenhouse.terrainFeatures.Keys) {

                this.terrainFeatures.Add(key, custom_Greenhouse.terrainFeatures[key]);
            
            }

        }
    }
}
