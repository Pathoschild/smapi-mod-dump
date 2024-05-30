/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.GUI.DragNDrop;
using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Util;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells
{
    public class SpellPartDraggableJson
    {
        [JsonProperty("Spell Part")]
        public string spellPartId { get; set; }
    }

    public class ShapeGroupAreaJson
    {
        [JsonProperty("Contents")]
        public List<SpellPartDraggableJson> spellPartDraggables { get; set; }
    }

    public class SpellPageJson
    {
        [JsonProperty("Shape Groups")]
        public List<ShapeGroupAreaJson> shapeGroups { get; set; }

        [JsonProperty("Spell Grammar")]
        public List<SpellPartDraggableJson> spellGrammerList { get; set; }

        [JsonProperty("Name")]
        public string name { get; set; }
    }

    public class SpellBookJson
    {
        [JsonProperty("Mana Cost Reduction Amount")]
        public int manaCostReductionAmount { get; set; }

        [JsonProperty("Current Spell Page Index")]
        public int currentSpellPageIndex { get; set; }

        [JsonProperty("Current Spell Index")]
        public int currentSpellIndex { get; set; }

        [JsonProperty("Spell Pages")]
        public List<SpellPageJson> spellPageJsons { get; set; }
    }

    /// <summary>A raw wrapper around the player's <see cref="Character.modData"/> fields.</summary>
    public class SpellBookData
    {

        /// <summary>The underlying player.</summary>
        public Farmer Player { get; }

        public int ManaCostReductionAmount { get; set; }

        //public IDictionary<string, SpellPage> SpellPages { get; set; }

        /// <summary>The player's spell pages.</summary>
        public List<SpellPage> SpellPages { get; set; }

        public SpellPage CurrentSpellPage { get; set; }

        /// <summary>The currently selected spell page, as an index.</summary>
        public int CurrentSpellPageIndex { get; set; }

        /// <summary>The currently selected spell, as an index.</summary>
        public int CurrentSpellIndex { get; set; }

        /// <summary>Construct an instance.</summary>
        /// <param name="player">The underlying player.</param>
        public SpellBookData(Farmer player)
        {
            this.Player = player;
        }

        /// <summary>Save the cached metadata to the player's <see cref="Character.modData"/> field.</summary>
        public void Save(ModEntry modEntry)
        {
            SaveSpellBook(modEntry);
        }

        public SavedSpellBook ReadSpellBook(ModEntry modEntry)
        {
            //Path.Combine(modEntry.helper.DirectoryPath + "Saves", $"{Constants.SaveFolderName}spellbook_data.json")
            //string localConfigPath = Path.Combine(modEntry.helper.DirectoryPath + "/Saves", $"{new DirectoryInfo(Game1.player.slotName)}_spellbook_data.json");
            string filePath = Path.Combine(modEntry.Helper.DirectoryPath + "/Saves", $"{Constants.SaveFolderName}_spellbook_data.json");

            SpellBookJson spellBookJson = JsonConvert.DeserializeObject<SpellBookJson>(File.ReadAllText(filePath));

            //modEntry.Monitor.Log(File.ReadAllText(modEntry.helper.DirectoryPath + "/data.json"), LogLevel.Info);

            int manaCostReductionAmount = spellBookJson.manaCostReductionAmount;
            List<SpellPageJson> deserializedSpellPages = spellBookJson.spellPageJsons;
            int currentPageIndex = spellBookJson.currentSpellPageIndex;
            int currentSpellIndex = spellBookJson.currentSpellIndex;

            SavedSpellPage[] spellPageList = new SavedSpellPage[10];

            for(int i = 0; i < deserializedSpellPages.Count; i++)
            {
                SpellPageJson spellPageJson = deserializedSpellPages[i];
                spellPageList[i] = ReadSpellPage(spellPageJson, modEntry);
            }

            SavedSpellBook spellbook = new SavedSpellBook();
            spellbook.SetManaCostReductionAmount(manaCostReductionAmount);
            spellbook.SetPages(spellPageList);

            spellbook.SetCurrentSpellPageIndex(currentPageIndex);
            spellbook.TurnToSpellPage();

            spellbook.SetCurrentSpellIndex(currentSpellIndex);
            spellbook.TurnToSpell();

            return spellbook;
        }

        public SavedSpellPage ReadSpellPage(SpellPageJson spellPageJson, ModEntry modEntry)
        {
            List<SavedShapeGroupArea<SpellPartDraggable>> shapeGroupsAreas = new List<SavedShapeGroupArea<SpellPartDraggable>>();

            for (int i = 0; i < spellPageJson.shapeGroups.Count; i++)
            {
                shapeGroupsAreas.Add(new SavedShapeGroupArea<SpellPartDraggable>());
            }

            List<SpellPartDraggable> spellGrammerList = new List<SpellPartDraggable>();

            //modEntry.Monitor.Log(JsonConvert.SerializeObject(spellPageJson.shapeGroups, Formatting.Indented), LogLevel.Info);

            for (int i = 0; i < spellPageJson.shapeGroups.Count; i++)
            {
                ShapeGroupAreaJson shapeGroupAreaJson = spellPageJson.shapeGroups[i];

                List<SpellPartDraggableJson> areaSpellPartDraggableJsons = shapeGroupAreaJson.spellPartDraggables;
                List<SpellPartDraggable> areaSpellDraggables = new List<SpellPartDraggable>();

                //modEntry.Monitor.Log(JsonConvert.SerializeObject(shapeGroupAreaJson, Formatting.Indented), LogLevel.Info);

                foreach (SpellPartDraggableJson spellPartDraggableJson in areaSpellPartDraggableJsons)
                {
                    SpellPartDraggable spellPartDraggable = new SpellPartDraggable(modEntry.spellPartManager.spellParts[spellPartDraggableJson.spellPartId], modEntry);
                    areaSpellDraggables.Add(spellPartDraggable);
                }

                SavedShapeGroupArea<SpellPartDraggable> shapeGroupArea = new SavedShapeGroupArea<SpellPartDraggable>();
                shapeGroupArea.SetAll(areaSpellDraggables);

                shapeGroupsAreas[i] = shapeGroupArea;
            }

            foreach (SpellPartDraggableJson spellPartDraggableJson in spellPageJson.spellGrammerList)
            {
                SpellPartDraggable spellPartDraggable = new SpellPartDraggable(modEntry.spellPartManager.spellParts[spellPartDraggableJson.spellPartId], modEntry);
                spellGrammerList.Add(spellPartDraggable);
            }

            SavedShapeGroupArea<SpellPartDraggable>[] shapeGroupsAreasArray = new SavedShapeGroupArea<SpellPartDraggable>[5];

            for (int i = 0; i < shapeGroupsAreasArray.Length; i++)
            {
                 shapeGroupsAreasArray[i] = shapeGroupsAreas[i];
            }

            SavedSpellPage spellPage = new SavedSpellPage();

            spellPage.SetSpellShapes(shapeGroupsAreasArray);
            spellPage.SetSpellGrammerList(spellGrammerList);
            spellPage.SetName(spellPageJson.name);

            return spellPage;
        }

        public void SaveSpellBook(ModEntry modEntry)
        {
            List<SpellPageJson> json = new List<SpellPageJson>();

            for (int i = 0; i < SpellPages.Count; i++)
            {
                json.Add(new SpellPageJson());
            }

            //modEntry.Monitor.Log(json.Count.ToString(), LogLevel.Info);

            for (int j = 0; j < json.Count; j++)
            {
                SpellPage page = this.SpellPages[j];

                json[j] = SaveSpellSpage(page);
            }

            SpellBookJson spellBookJson = new SpellBookJson()
            {
                manaCostReductionAmount = ManaCostReductionAmount,
                spellPageJsons = json,
                currentSpellPageIndex = CurrentSpellPageIndex,
                currentSpellIndex = CurrentSpellIndex
            };

            string jsonString = JsonConvert.SerializeObject(spellBookJson, Formatting.Indented);

            //modEntry.Monitor.Log(jsonString, LogLevel.Info);

            if (Constants.SaveFolderName != null)
            {
                //Path.Combine("Saves", $"{Constants.SaveFolderName}_SkinToneConfig.json")
                //File.WriteAllText(modEntry.helper.DirectoryPath + "/data.json", jsonString);

                string filePath = Path.Combine(modEntry.Helper.DirectoryPath + "/Saves", $"{Constants.SaveFolderName}_spellbook_data.json");
                //string filePath = Path.Combine(modEntry.helper.DirectoryPath + "/Saves", $"{new DirectoryInfo(Game1.player.slotName)}_spellbook_data.json");

                if(!Directory.Exists(modEntry.Helper.DirectoryPath + "/Saves"))
                {
                    Directory.CreateDirectory(modEntry.Helper.DirectoryPath + "/Saves");
                }
                else
                    File.WriteAllText(filePath, jsonString);
            }
        }

        public SpellPageJson SaveSpellSpage(SpellPage spellPage)
        {
            List<ShapeGroupAreaJson> shapeGroups = new List<ShapeGroupAreaJson>();

            for (int i = 0; i < spellPage.GetSpellShapeAreas().Length; i++)
            {
                shapeGroups.Add(new ShapeGroupAreaJson());
            }

            List<SpellPartDraggableJson> spellGrammerList = new List<SpellPartDraggableJson>();

            //spellPage.GetSpellShapes().Count
            for (int i = 0; i < spellPage.GetSpellShapeAreas().Length; i++)
            {
                ShapeGroupArea area = spellPage.GetSpellShapeAreas()[i];

                List<SpellPartDraggable> areaSpellDraggables = area.GetAll();

                List<SpellPartDraggableJson> spellPartDraggables = new List<SpellPartDraggableJson>();

                foreach (SpellPartDraggable spellDraggable in areaSpellDraggables)
                {
                    SpellPartDraggableJson spellPartDraggableJson = new SpellPartDraggableJson
                    {
                        spellPartId = spellDraggable.GetPart().GetId()
                    };

                    spellPartDraggables.Add(spellPartDraggableJson);
                }

                ShapeGroupAreaJson shapeGroupAreaJson = new ShapeGroupAreaJson
                {
                    spellPartDraggables = spellPartDraggables
                };

                shapeGroups[i] = shapeGroupAreaJson;
            }

            foreach (SpellPartDraggable spellDraggable in spellPage.GetSpellGrammerList())
            {

                SpellPartDraggableJson spellPartDraggableJson = new SpellPartDraggableJson
                {
                    spellPartId = spellDraggable.GetPart().GetId()
                };

                spellGrammerList.Add(spellPartDraggableJson);
            }

            SpellPageJson spellPageJson = new SpellPageJson
            {
                shapeGroups = shapeGroups,
                spellGrammerList = spellGrammerList,
                name = spellPage.GetName()
            };

            return spellPageJson;
        }
    }
}
