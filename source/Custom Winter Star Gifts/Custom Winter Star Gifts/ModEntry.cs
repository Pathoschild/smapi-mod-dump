/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/XxHarvzBackxX/Custom-Winter-Star-Gifts
**
*************************************************/

using System;
using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    public static List<ModData> Data = new List<ModData>();
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(this.ModManifest.UniqueID);

        // example patch, you'll need to edit this for your patch
        harmony.Patch(
           original: AccessTools.Method(typeof(Utility), nameof(Utility.getGiftFromNPC)),
           prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.getGiftFromNPC_Prefix))
        );
        foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
        {
            this.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
            if (!contentPack.HasFile("content.json"))
            {
                Monitor.Log($"{contentPack.Manifest.Name} {contentPack.Manifest.Version} is missing a \"content.json\" file.");
            }
            else
            {
                ModData modData = contentPack.ReadJsonFile<ModData>("content.json");
                Data.Add(modData);
            }
        }
        ObjectPatches.Initialize(Monitor, helper);
    }

    public class ModData
    {
        public NPCGifts[] NPCGifts { get; set; }
        public ModData()
        {
            NPCGifts n = new NPCGifts();
            NPCGifts = new NPCGifts[] { n };
        }
    }
    public class NPCGifts
    {
        public string NameOfNPC { get; set; } = "Robin";
        public string[] ItemNames { get; set; } = new string[] { "Parsnip" };
        public string Mode { get; set; } = "Overwrite"; // possibles: "Overwrite" or "Add"
        public NPCGifts()
        {
            NameOfNPC = "Robin";
            ItemNames = new string[] { "Parsnip" };
            Mode = "Overwrite";
        }
    }
    public class ObjectPatches
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        public static void Initialize(IMonitor monitor, IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;
        }

        public static bool getGiftFromNPC_Prefix(NPC who, ref Item __result)
        {
            try
            {
                Random r = new Random((int)Game1.uniqueIDForThisGame / 2 + Game1.year + Game1.dayOfMonth + Utility.getSeasonNumber(Game1.currentSeason) + who.getTileX());
                List<Item> possibleObjects = new List<Item>();
                foreach (ModData m in Data)
                {
                    foreach (NPCGifts g in m.NPCGifts)
                    {
                        if (g.Mode == "Add")
                        {
                            switch (who.Name)
                            {
                                case "Clint":
                                    possibleObjects.Add(new Object(337, 1));
                                    possibleObjects.Add(new Object(336, 5));
                                    possibleObjects.Add(new Object(r.Next(535, 538), 5));
                                    break;
                                case "Marnie":
                                    possibleObjects.Add(new Object(176, 12));
                                    break;
                                case "Robin":
                                    possibleObjects.Add(new Object(388, 99));
                                    possibleObjects.Add(new Object(390, 50));
                                    possibleObjects.Add(new Object(709, 25));
                                    break;
                                case "Willy":
                                    possibleObjects.Add(new Object(690, 25));
                                    possibleObjects.Add(new Object(687, 1));
                                    possibleObjects.Add(new Object(703, 1));
                                    break;
                                case "Evelyn":
                                    possibleObjects.Add(new Object(223, 1));
                                    break;
                                default:
                                    if (who.Age == 2)
                                    {
                                        possibleObjects.Add(new Object(330, 1));
                                        possibleObjects.Add(new Object(103, 1));
                                        possibleObjects.Add(new Object(394, 1));
                                        possibleObjects.Add(new Object(r.Next(535, 538), 1));
                                        break;
                                    }
                                    possibleObjects.Add(new Object(608, 1));
                                    possibleObjects.Add(new Object(651, 1));
                                    possibleObjects.Add(new Object(611, 1));
                                    possibleObjects.Add(new Ring(517));
                                    possibleObjects.Add(new Object(466, 10));
                                    possibleObjects.Add(new Object(422, 1));
                                    possibleObjects.Add(new Object(392, 1));
                                    possibleObjects.Add(new Object(348, 1));
                                    possibleObjects.Add(new Object(346, 1));
                                    possibleObjects.Add(new Object(341, 1));
                                    possibleObjects.Add(new Object(221, 1));
                                    possibleObjects.Add(new Object(64, 1));
                                    possibleObjects.Add(new Object(60, 1));
                                    possibleObjects.Add(new Object(70, 1));
                                    break;
                            }
                        }
                        if (g.NameOfNPC == who.Name || g.NameOfNPC == "All")
                        {
                            foreach (string itemName in g.ItemNames)
                            {
                                foreach (KeyValuePair<int, string> kvp in Game1.objectInformation)
                                {
                                    if (kvp.Value.Split('/')[4] == itemName)
                                    {
                                        possibleObjects.Add(new Object(kvp.Key, 1));
                                    }
                                }
                            }
                        }
                    }
                }
                __result = possibleObjects[r.Next(possibleObjects.Count)];
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(getGiftFromNPC_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}