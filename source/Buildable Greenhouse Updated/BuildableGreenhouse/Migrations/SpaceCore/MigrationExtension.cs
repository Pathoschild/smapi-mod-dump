/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/BuildableGreenhouse
**
*************************************************/

using BuildableGreenhouse.ModExtension;
using Microsoft.Xna.Framework;
using Netcode;
using Newtonsoft.Json;
using SolidFoundations.Framework.Models.ContentPack;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace BuildableGreenhouse.Migrations.SpaceCore
{
    public class MigrationExtension
    {
        IMonitor Monitor;
        ISolidFoundationsApi SolidFoundationsApi;

        public MigrationExtension(IMonitor monitor, ISolidFoundationsApi solidFoundationsApi)
        {
            Monitor = monitor;
            SolidFoundationsApi = solidFoundationsApi;
        }

        public void apply(object sender, SaveLoadedEventArgs e)
        {
            BuildableGameLocation farm = Game1.getFarm();
            NetCollection<Building> buildings = (NetCollection<Building>)typeof(BuildableGameLocation).GetField("buildings").GetValue(farm);

            int GreenhouseBuilding_tileX = -1;
            int GreenhouseBuilding_tileY = -1;

            string friendlyName = SaveGame.FilterFileName(Game1.GetSaveGameName());
            string filenameNoTmpString = friendlyName + "_" + Game1.uniqueIDForThisGame;
            string save_directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves", filenameNoTmpString + Path.DirectorySeparatorChar);
            string fullFilePath = Path.Combine(save_directory, "spacecore-serialization.json");
            string backupFilePath = Path.Combine(save_directory, "spacecore-serialization-backup.json");

            List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();
            string fileText = File.ReadAllText(fullFilePath);
            var rawNodes = JsonConvert.DeserializeObject<KeyValuePair<string, string>[]>(fileText);
            OptimizedModNodeList modNodes = new OptimizedModNodeList(rawNodes.Select(pair => new OptimizedModNode(xmlNode: pair.Value, path: pair.Key)).ToArray());

            foreach (OptimizedModNode node in modNodes.ModNodes)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(node.XmlNode);

                var top = doc.FirstChild;
                if (top.Attributes[0].Value == "Mods_Yariazen_BuildableGreenhouseBuilding")
                {
                    XmlNode docNode = doc.DocumentElement;
                    foreach (XmlNode n in docNode.ChildNodes)
                    {
                        if (n.Name == "tileX")
                        {
                            GreenhouseBuilding_tileX = Int32.Parse(n.InnerText);
                        }
                        else if (n.Name == "tileY")
                        {
                            GreenhouseBuilding_tileY = Int32.Parse(n.InnerText);
                        }
                    }
                }
                else if (top.Attributes[0].Value == "Mods_Yariazen_BuildableGreenhouseLocation")
                {
                    continue;
                }
                else
                {
                    keyValuePairs.Add(new KeyValuePair<string, string>(node.Path, node.XmlNode));
                }
            }

            ExtendedBuildingModel model = SolidFoundationsApi.GetBuildingModel("BuildableGreenhouse.Greenhouse").Value;
            GenericBuilding building = new GenericBuilding(model, new BluePrint("Greenhouse"), new Vector2(GreenhouseBuilding_tileX, GreenhouseBuilding_tileY));
            buildings.Add(building);

            File.WriteAllText(backupFilePath, fileText);
            File.Delete(fullFilePath);
            File.WriteAllText(fullFilePath, JsonConvert.SerializeObject(keyValuePairs));
        }
    }
}
