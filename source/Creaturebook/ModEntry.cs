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

namespace Creaturebook
{
    public class ModEntry : Mod
    {
        internal static new IModHelper Helper;

        string[] uniqueModIDs;
        static IManifest manifest;

        internal static ModConfig modConfig = new();
        internal static List<Chapter> Chapters = new();
        internal const string MyModID = "KediDili.Creaturebook";
        internal static IMonitor monitor;
        static int checksIfUsedCommand = 0;
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
            Helper.ConsoleCommands.Add("cb_reload", "Reloads all CB packs when entered.", ReloadPacks);
            Helper.ConsoleCommands.Add("cb_instantDiscover", "Instantly discovers a creature.\n\nUsage:cb_InstantDiscover <Content Pack ID> <CreatureNamePrefix> <Creature ID>\n Content Pack ID: Unique ID of a content pack.\nCreatureNamePrefix: The prefix for the chapter to be searched.\nCreature ID: ID of the creature to be searched", InstantDiscover);
            Helper.ConsoleCommands.Add("cb_instantTool","Instanly spawns the NotebookTool into your inventory", InstantToolSpawn);
        }
        
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            IContentPack[] contentPacks = Helper.ContentPacks.GetOwned().ToArray();
            uniqueModIDs = new string[contentPacks.Length];
            for (int i = 0; i < contentPacks.Length; i++)
            {
                monitor.Log($"Reading content pack: {contentPacks[i].Manifest.Name}, v{contentPacks[i].Manifest.Version}");

                if (!contentPacks[i].HasFile("chapter.json"))
                {
                    monitor.Log($"{contentPacks[i].Manifest.Name} seems to lack the 'chapter.json' file that is required. If you're the author please add the file or check your spelling in the filename, if you're a simple player please let the content pack author know of this error or reinstall the content pack. (If you read this at all, that is.)", LogLevel.Error);
                    continue;
                }
                Chapter[] chapterData = contentPacks[i].ReadJsonFile<Chapter[]>("chapter.json");

                if (chapterData == null)
                {
                    monitor.Log($"{contentPacks[i].Manifest.Name} seems to have the 'chapter.json', but it's empty. If I just wanted a null value I'd not ask for the file at all, right?", LogLevel.Warn);
                    continue;
                }
                
                for (int f = 0; f < chapterData.Length; f++)
                {
                    DirectoryInfo[] subfolders = new DirectoryInfo(Path.Combine(contentPacks[i].DirectoryPath, chapterData[f].Folder)).GetDirectories();
                    List<Creature> newCreatures = new();
                    if (subfolders.Length == 0)
                    {
                        monitor.Log($"{contentPacks[i].Manifest.Name} doesn't seem to have any creatures at all! O.o", LogLevel.Warn);
                        continue;
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
                        if (!File.Exists(Path.Combine(subfolders[s].FullName, "book-image_2.png")) && creatureData[0].HasExtraImages)
                        {
                            monitor.Log($"{contentPacks[i].Manifest.Name} seems to lack a 'book-image_2.png' under {subfolders[s].Name}.", LogLevel.Warn);
                            break;
                        }
                        creatureData[0].Directory = PathUtilities.NormalizePath(subfolders[s].FullName);
                        newCreatures.Add(creatureData[0]);
                    }
                    chapterData[f].Creatures = newCreatures.OrderBy(o => o.ID).ToArray();
                    Chapters.Add(chapterData[f]);
                }
                uniqueModIDs[i] = contentPacks[i].Manifest.UniqueID;
                if (checksIfUsedCommand is 0)
                    checksIfUsedCommand = 1;
            }
            Chapters = Chapters.OrderBy(o => o.Category).ToList();

            monitor.Log($"All content packs have been found, cleaned from invalid files and added into the Creaturebook!", LogLevel.Info);

            if (checksIfUsedCommand is 1)
            {
                var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
                var api = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
                var SCapi = Helper.ModRegistry.GetApi<IApi>("spacechase0.SpaceCore");

                HookToSpaceCoreAPI(SCapi);
                HooktoGMCMAPI(configMenu);
                HookToCPAPI(api);
                checksIfUsedCommand = 2;
            }
        }
        private static void HookToSpaceCoreAPI(IApi api)
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
                    return new[] { Convert.ToString(b) };

                return null;
            });

            cpAPI.RegisterToken(manifest, "AllCreatures", () =>
            {
                // save is loaded
                if (Context.IsWorldReady)
                    return new[] { Convert.ToString(a) };

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
            if(Context.IsWorldReady)
            Game1.player.addItemByMenuIfNecessary(new NotebookTool());
        }
        private void InstantDiscover(string command, string[] args)
        {
            if(Context.IsWorldReady)
            for (int i = 0; i < Chapters.Count; i++)
            {
                for (int l = 0; l < Chapters[i].Creatures.Length; l++)
                {
                    if (args[0] == Chapters[i].FromContentPack.Manifest.UniqueID && Chapters[i].CreatureNamePrefix == args[1] && args[2] == Chapters[i].Creatures[l].ID.ToString())
                    { 
                        Game1.player.modData[MyModID + "." + args[0] + "_" + args[1] + "_" + args[2]] = "true";
                        monitor.Log("The creature successfully has been instantly discovered.", LogLevel.Info);
                        return;
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
                e.LoadFromModFile<Texture2D>("assets/NotebookTexture.png", AssetLoadPriority.Medium);

            if (e.Name.IsEquivalentTo(Path.Combine(MyModID, "NoteItem")))
                e.LoadFromModFile<Texture2D>("assets/NoteItem.png", AssetLoadPriority.Medium);

            if (e.Name.IsEquivalentTo(Path.Combine(MyModID, "Stickies")))
                e.LoadFromModFile<Texture2D>("assets/Stickies.png", AssetLoadPriority.Medium);

            foreach (var chapter in Chapters)
            {
                for (int i = 0; i < chapter.Creatures.Length; i++)
                {
                    if (e.Name.IsEquivalentTo(Path.Combine(MyModID, chapter.FromContentPack.Manifest.UniqueID + "." + chapter.CreatureNamePrefix + "_" + chapter.Creatures[i].ID + "_Image1")))
                    {
                        Texture2D firstImage()
                        {
                            return chapter.FromContentPack.ModContent.Load<Texture2D>(PathUtilities.NormalizeAssetName(Path.Combine(chapter.Creatures[i].Directory, "book-image.png")));
                        }
                        e.LoadFrom(firstImage, AssetLoadPriority.Medium, onBehalfOf: chapter.FromContentPack.Manifest.UniqueID);
                        break;
                    }
                    if (e.Name.IsEquivalentTo(Path.Combine(MyModID, chapter.FromContentPack.Manifest.UniqueID + "." + chapter.CreatureNamePrefix + "_" + chapter.Creatures[i].ID + "_Image2")))
                    {
                        Texture2D secondImage()
                        {
                            return chapter.FromContentPack.ModContent.Load<Texture2D>(PathUtilities.NormalizeAssetName(Path.Combine(chapter.Creatures[i].Directory, "book-image_2.png")));
                        }
                        e.LoadFrom(secondImage, AssetLoadPriority.Medium, onBehalfOf: chapter.FromContentPack.Manifest.UniqueID);
                        break;
                    }
                    if (e.Name.IsEquivalentTo(Path.Combine(MyModID, chapter.FromContentPack.Manifest.UniqueID + "." + chapter.CreatureNamePrefix + "_" + chapter.Creatures[i].ID + "_Image3")))
                    {
                        Texture2D thirdImage()
                        {
                            return chapter.FromContentPack.ModContent.Load<Texture2D>(PathUtilities.NormalizeAssetName(Path.Combine(chapter.Creatures[i].Directory, "book-image_3.png")));
                        }
                        e.LoadFrom(thirdImage, AssetLoadPriority.Medium, onBehalfOf: chapter.FromContentPack.Manifest.UniqueID);
                        break;
                    }
                    /*if (e.Name.IsEquivalentTo(Path.Combine(MyModID, chapter.FromContentPack.Manifest.UniqueID + "." + chapter.CreatureNamePrefix + "_" + chapter.Creatures[i].ID + "_Data")))
                    {
                        Creature creatureData()
                        {
                            return chapter.FromContentPack.ModContent.Load<Creature[]>(PathUtilities.NormalizeAssetName(Path.Combine(chapter.Creatures[i].Directory, "creature.json")))[0];
                        }
                        e.LoadFrom(creatureData, AssetLoadPriority.Medium, onBehalfOf: chapter.FromContentPack.Manifest.UniqueID);
                        break;
                    }*/
                }
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset =>
                {
                    var editorDictionary = asset.AsDictionary<int, string>();

                    string ItemName = Helper.Translation.Get("CB.Notebook.Item.Name");
                    string ItemDesc = Helper.Translation.Get("CB.Notebook.Item.Desc");

                    editorDictionary.Data[31] = "Creaturebook//-300/Quest /" + ItemName + "/" + ItemDesc;
                });
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

                    editorDictionary.Data["70030005/e 70030004/i 31/H/t 0900 1700"] = code1 + text1 + code2 + text2 + code3 + text3 + code4 + text4 + code5 + text5 + code6 + text6 + code7;
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/mail") && modConfig.WayToGetNotebook == "Letter" && !Game1.player.eventsSeen.Contains(70030004) && !Game1.player.eventsSeen.Contains(70030005))
            {
                e.Edit(asset =>
                {
                    var editorDictionary = asset.AsDictionary<string,string>();

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
            if (Context.IsMainPlayer)
            {
                ModData c = Helper.Data.ReadSaveData<ModData>("KediDili.Creaturebook-DiscoveryProgress");

                if (c is not null)
                {
                    foreach (var item in c.DiscoveryDates)
                    {
                        if (item.Value is not null)
                        {
                            Game1.player.modData.Add(MyModID + "_" + item.Key, item.Value.DaysSinceStart.ToString());
                        }
                        else if (item.Value is null)
                        {
                            Game1.player.modData.Add(MyModID + "_" + item.Key, "null");
                        }
                    }
                    Game1.player.modData.Add(MyModID + "_IsNotebookObtained", c.IsNotebookObtained.ToString());
                }
                else if (c is null && !Game1.player.modData.ContainsKey(MyModID + "_IsNotebookObtained"))
                    Game1.player.modData.Add(MyModID + "_" + "IsNotebookObtained", "false");
                
                foreach (var chapter in Chapters)
                {
                    for (int i = 0; i < chapter.Creatures.Length; i++)
                    {
                        if (!Game1.player.modData.ContainsKey(MyModID + "_" + chapter.FromContentPack.Manifest.UniqueID + "." + chapter.CreatureNamePrefix + "_" + Convert.ToString(chapter.Creatures[i].ID)))
                            Game1.player.modData.Add(MyModID + "_" + chapter.FromContentPack.Manifest.UniqueID + "." + chapter.CreatureNamePrefix + "_" + chapter.Creatures[i].ID, "null");
                    }
                }
            }
        }
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            SDate date = SDate.Now();
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