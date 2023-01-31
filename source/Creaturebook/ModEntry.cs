/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KediDili/Creaturebook
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Creaturebook.Framework.UI;
using Creaturebook.Framework.Tools;
using Creaturebook.Framework.Interfaces;
using Creaturebook.Framework.Models;

namespace Creaturebook
{
    public class ModEntry : Mod
    {
        internal static new IModHelper Helper;

        string[] uniqueModIDs;
        static IManifest manifest;

        internal static ModConfig modConfig = new();
        internal static List<Chapter> Chapters = new();
        internal static List<Attractor> Attractors = new();
        internal const string MyModID = "KediDili.Creaturebook";
        internal static IMonitor monitor;
        static int checksIfUsedCommand = 0;
        internal static IJsonAssetsAPI jsonAssetsAPI;
        List<StardewValley.Object> qualifyingObjects;

        public override void Entry(IModHelper helper)
        {
            manifest = ModManifest;
            Helper = helper;
            monitor = Monitor;

            modConfig = Helper.ReadConfig<ModConfig>();

            Helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.Saving += OnSaving;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.Content.AssetRequested += OnAssetRequested;
            Helper.Events.Player.Warped += OnWarped;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;

            Helper.ConsoleCommands.Add("cb_reload", "Reloads all CB packs when entered.", ReloadPacks);
            Helper.ConsoleCommands.Add("cb_instantDiscover", "Instantly discovers a creature.\n\nUsage:cb_InstantDiscover <Content Pack ID> <CreatureNamePrefix> <Creature ID>\n Content Pack ID: Unique ID of a content pack.\nCreatureNamePrefix: The prefix for the chapter to be searched.\nCreature ID: ID of the creature to be searched", InstantDiscover);
            Helper.ConsoleCommands.Add("cb_instantTool", "Instanly spawns the NotebookTool into your inventory", InstantToolSpawn);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            IContentPack[] contentPacks = Helper.ContentPacks.GetOwned().ToArray();
            uniqueModIDs = new string[contentPacks.Length];
            for (int i = 0; i < contentPacks.Length; i++)
            {
                monitor.Log($"Reading content pack: {contentPacks[i].Manifest.Name}, v{contentPacks[i].Manifest.Version}");

                if (!contentPacks[i].HasFile(Path.Combine("Chapters", "chapter.json")))
                {
                    monitor.Log($"{contentPacks[i].Manifest.Name} seems to lack the 'chapter.json' file that is required. If you're the author please add the file or check your spelling in the filename, if you're a simple player please let the content pack author know of this error or reinstall the content pack.", LogLevel.Error);
                    continue;
                }
                Chapter[] chapterData = contentPacks[i].ReadJsonFile<Chapter[]>(Path.Combine("Chapters", "chapter.json"));

                if (chapterData == null)
                {
                    monitor.Log($"{contentPacks[i].Manifest.Name} seems to have the 'chapter.json', but it's empty.", LogLevel.Warn);
                    continue;
                }
                Attractor[] attractors;
                for (int f = 0; f < chapterData.Length; f++)
                {
                    DirectoryInfo[] subfolders = new DirectoryInfo(Path.Combine(contentPacks[i].DirectoryPath, "Chapters", chapterData[f].Folder)).GetDirectories();
                    List<Creature> newCreatures = new();
                    if (subfolders.Length == 0)
                    {
                        monitor.Log($"{contentPacks[i].Manifest.Name} doesn't seem to have any creatures at all! O.o", LogLevel.Warn);
                        break;
                    }
                    for (int s = 0; s < subfolders.Length; s++)
                    {
                        if (!File.Exists(Path.Combine(subfolders[s].FullName, "creature.json")))
                        {
                            monitor.Log($"{contentPacks[i].Manifest.Name} seems to lack a 'creature.json' under {subfolders[s].Name}.", LogLevel.Warn);
                            break;
                        }
                        Creature[] creatureData = contentPacks[i].ReadJsonFile<Creature[]>(Path.Combine(subfolders[s].Parent.Name, subfolders[s].Name, "creature.json"));

                        if (!File.Exists(Path.Combine(subfolders[s].FullName, "book-image.png")))
                        {
                            monitor.Log($"{contentPacks[i].Manifest.Name} seems to lack a 'book-image.png' under {subfolders[s].Name}.", LogLevel.Warn);
                            break;
                        }
                        chapterData[f].FromContentPack = contentPacks[i];
                        chapterData[f] = new(chapterData[f]); //, chapterData[f].Category, chapterData[f].Title, chapterData[f].Folder, chapterData[f].CreatureNamePrefix, chapterData[f].Sets, chapterData[f].RewardItem, chapterData[f].Author, chapterData[f].Creatures, chapterData[f].EnableSets
                        if (!File.Exists(Path.Combine(subfolders[s].FullName, "book-image_2.png")) && creatureData[0].HasExtraImages)
                        {
                            monitor.Log($"{contentPacks[i].Manifest.Name} seems to lack a 'book-image_2.png' under {subfolders[s].Name}.", LogLevel.Warn);
                            break;
                        }
                        creatureData[0].Directory = PathUtilities.NormalizePath(subfolders[s].FullName);
                        creatureData[0].PackID = chapterData[f].PackID;
                        newCreatures.Add(creatureData[0]);
                    }
                    chapterData[f].Creatures = newCreatures.OrderBy(o => o.ID).ToArray();
                    Chapters.Add(chapterData[f]);
                }
                uniqueModIDs[i] = contentPacks[i].Manifest.UniqueID;
                if (checksIfUsedCommand is 0)
                    checksIfUsedCommand = 1;

                try
                {
                    attractors = contentPacks[i].ReadJsonFile<Attractor[]>(Path.Combine("Attractors", "attractor.json"));
                }
                catch (FileNotFoundException)
                {
                    attractors = null;
                    break;
                }
                if (attractors is not null)
                {
                    for (int x = 0; x < attractors.Length; x++)
                    {
                        DirectoryInfo[] subfolders = new DirectoryInfo(Path.Combine(contentPacks[i].DirectoryPath, "Attractors")).GetDirectories();

                        if (subfolders.Length == 0)
                        {
                            monitor.Log($"{contentPacks[i].Manifest.Name} has an Attractors folder, but doesn't add any craftables as Attractors! This is likely to be an author mistake or mistaken installation of the content pack.", LogLevel.Warn);
                            break;
                        }

                        if (string.IsNullOrEmpty(attractors[x].Name))
                        {
                            monitor.Log($"{contentPacks[i].Manifest.Name} assigns an Attractor lacking the required Name field.", LogLevel.Warn);
                            break;
                        }

                        if (attractors[x].ItemsNCreatures == null || attractors[x].ItemsNCreatures == new Dictionary<string, string>())
                        {
                            monitor.Log($"{contentPacks[i].Manifest.Name} assigns an Attractor lacking the required ItemsN_Creatures dictionary.", LogLevel.Warn);
                            break;
                        }

                        Attractors.Add(attractors[x]);
                    }
                }
            }
            Chapters = Chapters.OrderBy(o => o.Category).ToList();

            monitor.Log($"All content packs have been found, cleaned from invalid files and added into the Creaturebook!", LogLevel.Info);

            if (checksIfUsedCommand is 1)
            {
                var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
                var CPapi = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
                var SCapi = Helper.ModRegistry.GetApi<ISpaceCoreAPI>("spacechase0.SpaceCore");
                jsonAssetsAPI = Helper.ModRegistry.GetApi<IJsonAssetsAPI>("spacechase0.JsonAssets");

                HookToSpaceCoreAPI(SCapi);
                HooktoGMCMAPI(configMenu);
                HookToCPAPI(CPapi);

                checksIfUsedCommand = 2;
            }
        }
        private static void HookToSpaceCoreAPI(ISpaceCoreAPI api)
        {
            if (api == null)
                return;

            NotebookTool tool = new();

            api.RegisterSerializerType(tool.GetType());
        }
        private static void HookToCPAPI(IContentPatcherAPI cpAPI)
        {
            int a = 0;
            int b = 0;
            if (cpAPI == null)
                return;
            foreach (var item in Game1.player.modData.Pairs)
            {
                if (item.Key.StartsWith(MyModID))
                {
                    a++;
                    if (item.Value != "null" && item.Key != MyModID + "_IsNotebookObtained")
                    {
                        b++;
                    }
                }
            }

            cpAPI.RegisterToken(manifest, "AllDiscoveredCreatures", () =>
            {
                // save is loaded
                if (Context.IsWorldReady)
                    return new[] { b.ToString() };

                return null;
            });

            cpAPI.RegisterToken(manifest, "AllCreatures", () =>
            {
                // save is loaded
                if (Context.IsWorldReady)
                    return new[] { a.ToString() };

                return null;
            });

            //should be used as if int
            cpAPI.RegisterToken(manifest, "DiscoveredCreaturesFromAChapter", new DiscoveredCreaturesFromAChapter());

            //should be used as if bool
            cpAPI.RegisterToken(manifest, "IsCreatureDiscovered", new IsCreatureDiscovered());
        }
        private static void HooktoGMCMAPI(IGenericModConfigMenuApi gmcmAPI)
        {
            if (gmcmAPI is null)
            {
                monitor.Log($"It appears either you haven't got Generic Mod Config Menu installed or an error occured while trying to hook up into its API. If the case is the first one, install it so that you can configure Creaturebook easier!", LogLevel.Info);
                return;
            }

            gmcmAPI.Register(
                mod: manifest,
                reset: () => modConfig = new ModConfig(),
                save: () => Helper.WriteConfig(modConfig)
            );

            // add some config options
            gmcmAPI.AddBoolOption(
                mod: manifest,
                name: () => Helper.Translation.Get("CB.GMCM.ShowScientificNames.Name"),
                tooltip: () => Helper.Translation.Get("CB.GMCM.ShowScientificNames.Desc"),
                getValue: () => modConfig.ShowScientificNames,
                setValue: value => modConfig.ShowScientificNames = value
            );

            gmcmAPI.AddBoolOption(
                mod: manifest,
                name: () => Helper.Translation.Get("CB.GMCM.ShowDiscoveryDates.Name"),
                tooltip: () => Helper.Translation.Get("CB.GMCM.ShowDiscoveryDates.Desc"),
                getValue: () => modConfig.ShowDiscoveryDates,
                setValue: value => modConfig.ShowDiscoveryDates = value
            );

            gmcmAPI.AddBoolOption(
                mod: manifest,
                name: () => Helper.Translation.Get("CB.GMCM.EnableStickies.Name"),
                tooltip: () => Helper.Translation.Get("CB.GMCM.EnableStickies.Desc"),
                getValue: () => modConfig.EnableStickies,
                setValue: value => modConfig.EnableStickies = value
            );

            gmcmAPI.AddKeybindList(
                mod: manifest,
                name: () => Helper.Translation.Get("CB.GMCM.OpenMenuKeybind.Name"),
                tooltip: () => Helper.Translation.Get("CB.GMCM.OpenMenuKeybind.Desc"),
                getValue: () => modConfig.OpenMenuKeybind,
                setValue: value => modConfig.OpenMenuKeybind = value
            );

            string Option1 = Helper.Translation.Get("CB.GMCM.WayToGetNotebook.Options.1");
            string Option2 = Helper.Translation.Get("CB.GMCM.WayToGetNotebook.Options.2");
            string Option3 = Helper.Translation.Get("CB.GMCM.WayToGetNotebook.Options.3");

            gmcmAPI.AddTextOption(
                mod: manifest,
                name: () => Helper.Translation.Get("CB.GMCM.WayToGetNotebook.Name"),
                tooltip: () => Helper.Translation.Get("CB.GMCM.WayToGetNotebook.Desc"),
                getValue: () => modConfig.WayToGetNotebook,
                setValue: value => modConfig.WayToGetNotebook = value,
                allowedValues: new string[] { Option1, Option2, Option3 }
            );
        }
        private void ReloadPacks(string command, string[] args)
        {
            OnGameLaunched(sender: null, e: null);
        }
        private void InstantToolSpawn(string command, string[] args)
        {
            if (Context.IsWorldReady)
                Game1.player.addItemByMenuIfNecessary(new NotebookTool());
        }
        private void InstantDiscover(string command, string[] args)
        {
            if (Context.IsWorldReady)
            {
                for (int i = 0; i < Chapters.Count; i++)
                {
                    for (int l = 0; l < Chapters[i].Creatures.Length; l++)
                    {
                        if (args[0] == Chapters[i].PackID && Chapters[i].CreatureNamePrefix == args[1] && args[2] == Chapters[i].Creatures[l].ID.ToString())
                        {
                            Game1.player.modData[MyModID + "_" + args[0] + "." + args[1] + "_" + args[2]] = "true";
                            monitor.Log("The creature successfully has been instantly discovered.", LogLevel.Info);
                            return;
                        }
                    }
                }
            }
        }
        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (!Context.IsPlayerFree)
                return;

            if (modConfig.OpenMenuKeybind.JustPressed() && uniqueModIDs.Length > 0)
                Game1.activeClickableMenu = new NotebookMenu();
            else if (modConfig.OpenMenuKeybind.JustPressed() && uniqueModIDs.Length == 0)
                Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("CB.noContentPacks"), 2));
        }
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo(Path.Combine(MyModID, "NotebookTexture")))
                e.LoadFromModFile<Texture2D>("assets/NotebookTexture.png", AssetLoadPriority.Exclusive);

            if (e.Name.IsEquivalentTo(Path.Combine(MyModID, "NoteItem")))
                e.LoadFromModFile<Texture2D>("assets/NoteItem.png", AssetLoadPriority.Exclusive);

            if (e.Name.IsEquivalentTo(Path.Combine(MyModID, "Stickies")))
                e.LoadFromModFile<Texture2D>("assets/Stickies.png", AssetLoadPriority.Exclusive);

            for (int x = 0; x < Chapters.Count; x++)
            {
                for (int i = 0; i < Chapters[x].Creatures.Length; i++)
                {
                    if (e.Name.IsEquivalentTo(Path.Combine(MyModID, Chapters[x].PackID + "." + Chapters[x].CreatureNamePrefix + "_" + i + "_Image1")))
                    {
                        Texture2D firstImage()
                        {
                            return Chapters[x].FromContentPack.ModContent.Load<Texture2D>(PathUtilities.NormalizeAssetName(Path.Combine(Chapters[x].Creatures[i].Directory, "book-image.png")));
                        }
                        e.LoadFrom(firstImage, AssetLoadPriority.Exclusive, onBehalfOf: Chapters[x].PackID);
                        return;
                    }
                    if (e.Name.IsEquivalentTo(Path.Combine(MyModID, Chapters[x].PackID + "." + Chapters[x].CreatureNamePrefix + "_" + i + "_Image2")))
                    {
                        Texture2D secondImage()
                        {
                            return Chapters[x].FromContentPack.ModContent.Load<Texture2D>(PathUtilities.NormalizeAssetName(Path.Combine(Chapters[x].Creatures[i].Directory, "book-image_2.png")));
                        }
                        e.LoadFrom(secondImage, AssetLoadPriority.Exclusive, onBehalfOf: Chapters[x].PackID);
                        return;
                    }
                    if (e.Name.IsEquivalentTo(Path.Combine(MyModID, Chapters[x].PackID + "." + Chapters[x].CreatureNamePrefix + "_" + i + "_Image3")))
                    {
                        Texture2D thirdImage()
                        {
                            return Chapters[x].FromContentPack.ModContent.Load<Texture2D>(PathUtilities.NormalizeAssetName(Path.Combine(Chapters[x].Creatures[i].Directory, "book-image_3.png")));
                        }
                        e.LoadFrom(thirdImage, AssetLoadPriority.Exclusive, onBehalfOf: Chapters[x].PackID);
                        return;
                    }
                    if (e.Name.IsEquivalentTo(Path.Combine(MyModID, Chapters[x].PackID + "." + Chapters[x].CreatureNamePrefix + "_" + i + "_CreatureData")))
                    {
                        Creature creatureData()
                        {
                            return Chapters[x].FromContentPack.ModContent.Load<Creature[]>(PathUtilities.NormalizeAssetName(Path.Combine(Chapters[x].Creatures[i].Directory, "creature.json")))[0];
                        }
                        e.LoadFrom(creatureData, AssetLoadPriority.Exclusive, onBehalfOf: Chapters[x].PackID);
                        return;
                    }
                    if (e.Name.IsEquivalentTo(Path.Combine(MyModID, Chapters[x].PackID + "." + Chapters[x].CreatureNamePrefix + "_ChapterData")))
                    {
                        Chapter chapterData()
                        {
                            Chapter[] chapters = Chapters[x].FromContentPack.ModContent.Load<Chapter[]>(PathUtilities.NormalizeAssetName(Path.Combine(Chapters[x].FromContentPack.DirectoryPath, "chapter.json")));
                            for (int i = 0; i < chapters.Length; i++)
                            {
                                if (chapters[i] == Chapters[x])
                                    return chapters[i];
                            }
                            return chapters[0];
                        }
                        e.LoadFrom(chapterData, AssetLoadPriority.Exclusive, onBehalfOf: Chapters[x].PackID);
                        return;
                    }
                }
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/springobjects"))
            {
                e.Edit(asset =>
                {
                    var editorImage = asset.AsImage();

                    Texture2D sourceImage = Helper.ModContent.Load<Texture2D>("assets/NoteItem.png");
                    editorImage.PatchImage(sourceImage, targetArea: new Rectangle(112, 16, 16, 16));
                });
            }
            if (e.Name.IsEquivalentTo(Path.Combine("KediDili.Creaturebook", "SearchButton")))
                e.LoadFromModFile<Texture2D>("assets/SearchButton.png", AssetLoadPriority.Medium);

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Mountain") && modConfig.WayToGetNotebook == "Events")
            {
                e.Edit(asset =>
                {
                    var editorDictionary = asset.AsDictionary<string, string>();

                    string code1 = "none/16 31/farmer 15 39 0/addObject 12 31 31/skippable/pause 1000/move farmer 0 -8 3 false/emote farmer 16/pause 750/move farmer -2 0 3 false/message \"";
                    string code2 = "\"/pause 1000/removeObject 12 31/message \"";
                    string code3 = "\"/pause 750 /end warpOut";

                    string text1 = Helper.Translation.Get("CB.ObtainNotebook.Event_1-1");
                    string text2 = Helper.Translation.Get("CB.ObtainNotebook.Event_1-2");

                    editorDictionary.Data["70030004/j 7/t 2000 2600/w rainy/H/Hl giveOutCreaturebook/c 1"] = code1 + text1 + code2 + text2 + code3;
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo(Path.Combine("TileSheets", "tools")) && Game1.player.ActiveObject is not null)
            {
                if (Game1.player.ActiveObject.Name == "NotebookTool")
                {
                    e.Edit(asset =>
                    {
                        var editImage = asset.AsImage();

                        Texture2D sourceImage = Helper.ModContent.Load<Texture2D>("assets/toolfix.png");
                        editImage.PatchImage(sourceImage, targetArea: new Rectangle(0, 0, 336, 384));
                    }
                    );
                }
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/ScienceHouse") && modConfig.WayToGetNotebook == "Events")
            {
                e.Edit(asset =>
                {
                    var editorDictionary = asset.AsDictionary<string, string>();

                    string code1 = "continue/6 17/Robin 8 18 2 farmer 6 23 0 Demetrius 22 21 1/skippable/move farmer 0 -3 0 false/move farmer 2 0 0 false/emote Robin 16/pause 500/speak Robin \"";
                    string code2 = "\"/emote farmer 40/pause 500/speak Robin \"";
                    string code3 = "\"/pause 500/emote farmer 32/move farmer 11 0 1 false/viewport move 3 0 4000/pause 1300/speak Demetrius \"";
                    string code4 = "\"/faceDirection Demetrius 3/emote Demetrius 16/pause 750/speak Demetrius \"";
                    string code5 = "\"/emote farmer 40/pause 750/addObject 20 20 31/pause 1500/speak Demetrius \"";
                    string code6 = "\"/emote farmer 60/removeObject 20 20 31/pause 250/faceDirection farmer 3/faceDirection Demetrius 1/move farmer -11 0 3 true/emote Demetrius 40/message \"";
                    string code7 = "\"/end warpOut";

                    string text1 = Helper.Translation.Get("CB.ObtainNotebook.Event_2-1");
                    string text2 = Helper.Translation.Get("CB.ObtainNotebook.Event_2-2");
                    string text3 = Helper.Translation.Get("CB.ObtainNotebook.Event_2-3");
                    string text4 = Helper.Translation.Get("CB.ObtainNotebook.Event_2-4");
                    string text5 = Helper.Translation.Get("CB.ObtainNotebook.Event_2-5");
                    string text6 = Helper.Translation.Get("CB.ObtainNotebook.Event_2-6");

                    editorDictionary.Data["70030005/e 70030004/H/t 0900 1700"] = code1 + text1 + code2 + text2 + code3 + text3 + code4 + text4 + code5 + text5 + code6 + text6 + code7;
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/mail") && modConfig.WayToGetNotebook == "Letter" && !Game1.player.eventsSeen.Contains(70030004) && !Game1.player.eventsSeen.Contains(70030005))
            {
                e.Edit(asset =>
                {
                    var editorDictionary = asset.AsDictionary<string, string>();

                    string text = Helper.Translation.Get("CB.ObtainNotebook.Mail");

                    editorDictionary.Data["giveOutCreaturebook"] = text;
                });
            }
        }
        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                ModData c = Helper.Data.ReadSaveData<ModData>("KediDili.Creaturebook-DiscoveryProgress");
                List<string> s = Helper.Data.ReadSaveData<List<string>>("KediDili.Creaturebook-PreviouslyDownloadedPacks");

                if (s != null && c != null)
                {
                    foreach (string item in s)
                    {
                        foreach (var key in Game1.player.modData.Keys)
                        {
                            if (key.Contains(MyModID + "_" + item) && !Helper.ModRegistry.IsLoaded(item))
                                Game1.player.modData.Remove(key);
                        }
                    }
                    for (int i = 0; i < uniqueModIDs.Length; i++)
                        Game1.player.modData[MyModID + "_" + "PreviouslyDownloadedPacks" + i.ToString()] = uniqueModIDs[i];
                }
                Helper.Data.WriteSaveData<ModData>("KediDili.Creaturebook-PreviouslyDownloadedPacks", null);
                Helper.Data.WriteSaveData<ModData>("KediDili.Creaturebook-DiscoveryProgress", null);
            }
        }
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Context.IsMainPlayer) // Converts old ModData custom class data to Game1.player.modData
            {
                ModData c = Helper.Data.ReadSaveData<ModData>("KediDili.Creaturebook-DiscoveryProgress");

                if (c is not null)
                {
                    foreach (var item in c.DiscoveryDates)
                    {
                        if (item.Value is not null)
                            Game1.player.modData.Add(MyModID + "_" + item.Key, item.Value.DaysSinceStart.ToString());

                        else if (item.Value is null)
                            Game1.player.modData.Add(MyModID + "_" + item.Key, "null");
                    }
                    Game1.player.modData.Add(MyModID + "_IsNotebookObtained", c.IsNotebookObtained.ToString());
                }
                else if (c is null && !Game1.player.modData.ContainsKey(MyModID + "_IsNotebookObtained"))
                    Game1.player.modData.Add(MyModID + "_" + "IsNotebookObtained", "false");

                //Daily update to all chapters and creature data
                for (int f = 0; f < Chapters.Count; f++)
                {
                    Chapter freshChapter = Helper.GameContent.Load<Chapter>(Path.Combine("KediDili.Creaturebook", Chapters[f].PackID + "." + Chapters[f].CreatureNamePrefix + "_ChapterData"));

                    if (!Chapters[f].DetailedEquality(freshChapter))
                        Chapters[f] = freshChapter;

                    for (int a = 0; a < Chapters[f].Sets.Length; a++)
                        if (!Chapters[f].Sets[a].DetailedEquality(freshChapter.Sets[f]))
                            Chapters[f].Sets[a] = freshChapter.Sets[a];

                    for (int i = 0; i < Chapters[f].Creatures.Length; i++)
                    {
                        if (!Game1.player.modData.ContainsKey(MyModID + "_" + Chapters[f].PackID + "." + Chapters[f].CreatureNamePrefix + "_" + i))
                            Game1.player.modData.Add(MyModID + "_" + Chapters[f].PackID + "." + Chapters[f].CreatureNamePrefix + "_" + i, "null");

                        Creature freshCreature = Helper.GameContent.Load<Creature>(Path.Combine("KediDili.Creaturebook", Chapters[f].PackID + "." + Chapters[f].CreatureNamePrefix + "_" + Chapters[f].Creatures[i].ID + "_CreatureData"));
                        if (!Chapters[i].Creatures[i].DetailedEquality(freshCreature))
                            Chapters[i].Creatures[i] = freshCreature;
                    }
                }
            }
        }
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            SDate date = SDate.Now(); //Checks stuff to maka correct additions to savedata and inventory
            if (Game1.player.modData.ContainsKey(MyModID + "_" + "IsNotebookObtained"))
            {
                if (Context.IsMainPlayer && Game1.player.modData[MyModID + "_" + "IsNotebookObtained"] == "false")
                {
                    if (modConfig.WayToGetNotebook == "Inventory")
                    {
                        Game1.player.addItemByMenuIfNecessary(new NotebookTool());
                        Game1.player.modData[MyModID + "_" + "IsNotebookObtained"] = "true";
                    }
                    else if (modConfig.WayToGetNotebook == "Letter" && !Game1.player.hasOrWillReceiveMail("giveOutCreaturebook") && date.DaysSinceStart > 7)
                    {
                        Game1.addMailForTomorrow("giveOutCreaturebook");
                        Game1.player.modData[MyModID + "_" + "IsNotebookObtained"] = "true";
                    }
                }
            }

            //Finds all eligible machines to become Attractors (except those in barns coops etc.)
            qualifyingObjects = new();
            foreach (GameLocation loc in Game1.locations)
                foreach (StardewValley.Object item in loc.Objects.Values)
                    for (int c = 0; c < Attractors.Count; c++)
                        if (Attractors[c].Name == item.Name && item.bigCraftable.Value)
                            qualifyingObjects.Add(item);

            for (int i = 0; i < qualifyingObjects.Count; i++)
                if (qualifyingObjects[i].heldObject is not null)
                    qualifyingObjects[i].IsOn = true;
        }
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button.IsActionButton())
            {
                Vector2 cursorTile = e.Cursor.GrabTile;
                for (int i = 0; i < Attractors.Count; i++)
                    if (Game1.currentLocation.Objects.ContainsKey(cursorTile) && Game1.currentLocation.Objects[cursorTile].Name == Attractors[i].Name)
                        if (Attractors[i].IsEligible(cursorTile, "Empty", i))
                            YeetItem(cursorTile, i);
            }
        }
        public static void YeetItem(Vector2 itemKey, int i)
        {
            for (int x = 0; x < Attractors[i].ItemsNCreatures.Count; x++)
            {
                if (Attractors[i].Probability is null || Probabilities(i))
                {
                    Game1.currentLocation.Objects[itemKey].heldObject.Value = Game1.player.ActiveObject;
                    Game1.player.ActiveObject = null;
                }
            }
        }
        public static bool Probabilities(int i)
        {
            foreach (var prob in Attractors[i].Probability)
                if (Game1.random.NextDouble() <= Convert.ToDouble(Attractors[i].ReturnProb(prob.Key, 1)))
                    return true;
            
            return false;
        }
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.Player.eventsSeen.Contains(70030004) && Game1.player.modData[MyModID + "_" + "IsNotebookObtained"] == "false")
            {
                if (modConfig.WayToGetNotebook == "Events" && Context.IsMainPlayer)
                {
                    Game1.player.addItemByMenuIfNecessary(new NotebookTool());
                    Game1.player.modData[MyModID + "_" + "IsNotebookObtained"] = "true";
                }
            }
        }
    }
}