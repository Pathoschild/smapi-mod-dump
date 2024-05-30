/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/OutfitDisable
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Characters;

namespace OutfitDisable
{
    internal sealed class ModEntry : Mod
    {
        readonly static string[] modImplementedOutfits = { "Winter", "Beach" };
        static IModHelper modHelper;
        static IMonitor monitor;
        static readonly Dictionary<string, OutfitOverrideList> configLists = new();
        static readonly Dictionary<string, AnimationsReplaceList> animationReplaceData = new();

        public override void Entry(IModHelper helper)
        {
            modHelper = helper;
            monitor = Monitor;
            var harmony = new Harmony(ModManifest.UniqueID);
            AddJsonToDictionary(configLists);
            LoadAnimationReplacer(animationReplaceData);

            helper.Events.Content.AssetRequested += Content_AssetRequested;
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Locations.IslandSouth), nameof(StardewValley.Locations.IslandSouth.HasIslandAttire)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(BeachOutfitCheck))
                );
        }

        private void Content_AssetRequested(object? sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            // animations replace
            if (e.Name.IsEquivalentTo("Data/animationDescriptions"))
            {
                e.Edit(asset =>
                {
                    var originalData = asset.AsDictionary<string, string>().Data;

                    foreach (string outfitType in modImplementedOutfits)
                    {

                        animationReplaceData.TryGetValue(outfitType, out AnimationsReplaceList? animReplaceList);
                        if (animReplaceList != null)
                        {
                            foreach (string characterName in animReplaceList.Targets.Keys)
                            {
                                configLists.TryGetValue(outfitType, out OutfitOverrideList? configListOneType);
                                if (configListOneType != null)
                                {
                                    if (ShouldDisableOutfit(characterName, outfitType))
                                    {
                                        // apply reference animations
                                        Dictionary<string, string>? list = animReplaceList.Targets[characterName].referenceAnimations;
                                        if (list != null)
                                            foreach (string targetKey in list.Keys)
                                            {
                                                list.TryGetValue(targetKey, out string? keyForReference);
                                                if (keyForReference != null)
                                                {
                                                    originalData.TryGetValue(keyForReference, out string? referencedString);
                                                    if (referencedString != null)
                                                        originalData[targetKey] = referencedString;
                                                }

                                            }
                                        // apply manual edit
                                        list = animReplaceList.Targets[characterName].manualAnimations;
                                        if (list != null)
                                            foreach (string targetKey in list.Keys.ToList())
                                            {
                                                list.TryGetValue(targetKey, out string? stringForReplace);
                                                if (stringForReplace != null)
                                                {
                                                    originalData[targetKey] = stringForReplace;
                                                }
                                            }
                                    }
                                }
                            }
                        }
                    }
                },
                AssetEditPriority.Late
                );

            }

            // winter outfit remove
            if (e.Name.IsEquivalentTo("Data/Characters"))
            {
                if (configLists.TryGetValue("Winter", out _))
                {
                    e.Edit(asset =>
                    {
                        var entireData = asset.AsDictionary<string, CharacterData>().Data;
                        ICollection<string> names = entireData.Keys;
                        foreach (string name in names)
                        {
                            if (ShouldDisableOutfit(name, "Winter"))
                            {
                                var oneCharacterData = entireData[name];
                                foreach (var item in oneCharacterData.Appearance)
                                {
                                    if (item.Id == "Winter")
                                    {
                                        oneCharacterData.Appearance.Remove(item);
                                        break;
                                    }
                                }
                            }
                        }
                    },
                    AssetEditPriority.Late
                    );
                }
            }
        }

        // For unknown reasons, argument reference cannot be made, so all arguments are referenced.
        public static void BeachOutfitCheck(object[] __args, ref bool __result)
        {
            if (__args[0] != null)
            {
                if (__args[0].GetType() == typeof(NPC))
                {
                    // data only apply if beach attire is found
                    if (__result == true)
                        __result = !ShouldDisableOutfit(((NPC)__args[0]).Name, "Beach");
                    monitor.Log(((NPC)__args[0]).Name + ": beach outfit: " + __result, LogLevel.Trace);
                }
                else
                {
                    monitor.Log("BeachOutfitCheck: The NPC's information couldn't be referenced.\nTarget NPC's outfit won't be disabled.", LogLevel.Error);
                }
            }
        }

        private static bool ShouldDisableOutfit(string name, string outfitType)
        {
            if (configLists.ContainsKey(outfitType))
            {
                var list = configLists[outfitType];
                if (IsException(name, list))
                {
                    list.ExceptionsList.TryGetValue(name, out bool value);
                    return value;
                }
                else
                {
                    return GetDefault(outfitType);
                }
            }
            else
            {
                monitor.Log("Couldn't find config data of " + outfitType.ToString() + ".\nTarget NPC's outfit won't be disabled.", LogLevel.Error);
                return false;
            }
        }

        private static bool IsException(string name, OutfitOverrideList list)
        {
            if (list.ExceptionsList.ContainsKey(name))
                return true;
            else
                return false;
        }

        private static bool GetDefault(string outfitType)
        {
            configLists.TryGetValue(outfitType, out OutfitOverrideList? list);
            if (list != null)
            {
                monitor.LogOnce(outfitType.ToString() + ": default: " + list.Default, LogLevel.Trace);
                return list.Default;
            }
            else
            {
                monitor.Log("Couldn't find config data of " + outfitType.ToString() + ".\nEvery NPC's outfit won't be disabled.", LogLevel.Error);
                return false;
            }
        }

        // add outfit configs
        private static void AddJsonToDictionary(Dictionary<string, OutfitOverrideList> configLists)
        {
            foreach (string outfitType in modImplementedOutfits)
            {
                var data = modHelper.Data.ReadJsonFile<OutfitOverrideList>("config/disable_" + outfitType + ".json");
                if (data == null)
                {
                    data = new OutfitOverrideList();
                    modHelper.Data.WriteJsonFile("config/disable_" + outfitType + ".json", data);
                    monitor.Log("Wrote \"config/disable_" + outfitType + ".json\"", LogLevel.Info);
                }
                else
                {
                    monitor.Log("Added \"config/disable_" + outfitType + ".json\"", LogLevel.Trace);
                }
                configLists.Add(outfitType, data);
            }
            foreach (string s in configLists.Keys)
            {
                monitor.Log("configLists contains " + s);
            }
        }

        // add animations replacer
        private static void LoadAnimationReplacer(Dictionary<string, AnimationsReplaceList> replacerLists)
        {
            foreach (string outfitType in modImplementedOutfits)
            {
                var data = modHelper.Data.ReadJsonFile<AnimationsReplaceList>("data/animationsReplace_" + outfitType + ".json");
                if (data != null)
                {
                    replacerLists.Add(outfitType, data);
                }
                else
                {
                    if (outfitType == "Beach")
                    {
                        string url = "https://github.com/idermailer/OutfitDisable/blob/master/OutfitDisable/data/animationsReplace_Beach_default.txt";
                        monitor.Log("If you deleted \"data/animationsReplace_" + outfitType + ".json\", you can copy from this mod's GitHub repository,\n" +
                            url + ",\nand save as json file.", LogLevel.Warn);
                    }
                }
            }
        }
    }
}