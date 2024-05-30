/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Additions;
using ItemExtensions.Models;
using ItemExtensions.Models.Contained;
using ItemExtensions.Models.Items;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;

namespace ItemExtensions.Events;

public static class Assets
{
    private static IModHelper Helper => ModEntry.Help;
    private static string Id => ModEntry.Id;
    public static void OnInvalidate(object sender, AssetsInvalidatedEventArgs e)
    {
        if (e.NamesWithoutLocale.Any(a => a.Name.Equals($"Mods/{Id}/Resources")))
        {
            var clumps = Helper.GameContent.Load<Dictionary<string, ResourceData>>($"Mods/{Id}/Resources");
            Parser.Resources(clumps);
        }
        
        if(e.NamesWithoutLocale.Any(a => a.Name.Equals($"Mods/{Id}/Mines/Terrain")))
        {
            var trees = Helper.GameContent.Load<Dictionary<string, TerrainSpawnData>>($"Mods/{Id}/Mines/Terrain");
            Parser.Terrain(trees);
        }

        //don't reload if on title screen
        if (!Context.IsWorldReady)
            return;
        
        //drops
        if (e.NamesWithoutLocale.Any(a => a.Name.Equals($"Mods/{Id}/Panning")))
        {
            var panData = Helper.GameContent.Load<Dictionary<string, PanningData>>($"Mods/{Id}/Panning");
            Parser.Panning(panData);
        }

        //drops
        if (e.NamesWithoutLocale.Any(a => a.Name.Equals($"Mods/{Id}/Treasure")))
        {
            ModEntry.Treasure = Helper.GameContent.Load<Dictionary<string, ExtraSpawn>>($"Mods/{Id}/Treasure");
        }

        if (e.NamesWithoutLocale.Any(a => a.Name.Equals($"Mods/{Id}/Train")))
        {
            var trainData = Helper.GameContent.Load<Dictionary<string, TrainDropData>>($"Mods/{Id}/Train");
            Parser.Train(trainData);
        }

        if (e.NamesWithoutLocale.Any(a => a.Name.Equals($"Mods/{Id}/MixedSeeds")))
        {
            //get menu actions
            var seeds = Helper.GameContent.Load<Dictionary<string, List<MixedSeedData>>>($"Mods/{Id}/MixedSeeds");
            Parser.MixedSeeds(seeds);
        }

        //etc
        if (e.NamesWithoutLocale.Any(a => a.Name.Equals($"Mods/{Id}/Data")))
        {
            var objectData = Helper.GameContent.Load<Dictionary<string, ItemData>>($"Mods/{Id}/Data");
            Parser.ObjectData(objectData);
        }
        
        if (e.NamesWithoutLocale.Any(a => a.Name.Equals($"Mods/{Id}/EatingAnimations")))
        {
            //get menu actions
            var animations = Helper.GameContent.Load<Dictionary<string, FarmerAnimation>>($"Mods/{Id}/EatingAnimations");
            Parser.EatingAnimations(animations);
        }
    }

    public static void OnRequest(object sender, AssetRequestedEventArgs e)
    {
        //for adding hover reqs to UI stuff, in case another mod wants the string
        if (e.NameWithoutLocale.IsEquivalentTo("Strings/UI"))
        {
            e.Edit(asset =>
            {
                var dictionary = asset.AsDictionary<string, string>();
                dictionary.Data.Add("ItemHover_Requirements_Extra", ModEntry.Help.Translation.Get("ItemHover_Requirements_Extra"));
            });
        }
        
        //for adding node data
        if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
        {
            e.Edit(asset =>
            {
                var dictionary = asset.AsDictionary<string, ObjectData>();
                foreach (var (itemId, data) in ModEntry.Ores)
                {
                    if(string.IsNullOrWhiteSpace(itemId))
                        continue;
                    
                    //check if vanilla, skip if so
                    if (Parser.IsVanilla(itemId))
                    {
                        continue;
                    }

                    if(data.Width > 1 || data.Height > 1)
                        continue;

                    //var customFields = data.CustomFields ?? new Dictionary<string, string>();
                    //customFields.Add(ModKeys.Resource, "true");
                    
                    var objectData = new ObjectData
                    {
                        Name = data.Name ?? itemId,
                        DisplayName = data.DisplayName ?? "[LocalizedText Strings\\Objects:Stone_Node_Name]",
                        Description = data.Description ?? "[LocalizedText Strings\\Objects:Stone_Node_Description]",
                        Type = "Litter",
                        Category = -999,
                        Price = 0,
                        Texture = data.Texture,
                        SpriteIndex = data.SpriteIndex,
                        Edibility = -300,
                        IsDrink = false,
                        Buffs = null,
                        GeodeDropsDefaultItems = false,
                        GeodeDrops = null,
                        ArtifactSpotChances = null,
                        ExcludeFromFishingCollection = true,
                        ExcludeFromShippingCollection = true,
                        ExcludeFromRandomSale = true,
                        ContextTags = data.ContextTags,
                        CustomFields = data.CustomFields
                    };
                    if(dictionary.Data.TryAdd(itemId, objectData) == false)
                        dictionary.Data[itemId] = objectData;
                }
            });
        }
        
        //item extensibility
        if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{Id}/Data", true))
        {
            e.LoadFrom(
                () => new Dictionary<string, ItemData>(),
                AssetLoadPriority.Low);
        }
        
        //animation
        if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{Id}/EatingAnimations", true))
        {
            e.LoadFrom(DefaultContent.GetAnimations, AssetLoadPriority.Low);
        }
        
        //menu actions / object behavior
        if (e.NameWithoutLocale.StartsWith($"Mods/{Id}/MenuActions/"))
        {
            e.LoadFrom(
                () => new Dictionary<string, MenuBehavior>
                {
                    { "None", new() }
                },
                AssetLoadPriority.Low);
        }
        
        //seeds
        if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{Id}/MixedSeeds", true))
        {
            e.LoadFrom(
                () => new Dictionary<string, List<MixedSeedData>>(),
                AssetLoadPriority.Low);
        }
        
        //panning
        if(e.NameWithoutLocale.IsEquivalentTo($"Mods/{Id}/Panning", true))
        {
            e.LoadFrom(
                () => new Dictionary<string, PanningData>(),
                AssetLoadPriority.Low);
        }

        //fishing treasure
        if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{Id}/Treasure", true))
        {
            e.LoadFrom(
                () => new Dictionary<string, ExtraSpawn>(),
                AssetLoadPriority.Low);
        }

        //train
        if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{Id}/Train", true))
        {
            e.LoadFrom(
                () => new Dictionary<string, TrainDropData>(),
                AssetLoadPriority.Low);
        }

        //resources
        if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{Id}/Resources", true))
        {
            e.LoadFrom(
                () => new Dictionary<string, ResourceData>(),
                AssetLoadPriority.Low);
        }

        if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{Id}/Mines/Terrain", true))
        {
            e.LoadFrom(
                () => new Dictionary<string, TerrainSpawnData>(),
                AssetLoadPriority.Low);
        }

        //texture
        if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{Id}/Textures/Drink", true))
        {
            e.LoadFromModFile<Texture2D>("assets/Drink.png", AssetLoadPriority.Low);
        }
        if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{Id}/Textures/Grass", true))
        {
            e.LoadFromModFile<Texture2D>("assets/Grass.png", AssetLoadPriority.Low);
        }
        if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{Id}/Textures/Stump", true))
        {
            e.LoadFromModFile<Texture2D>("assets/Stump.png", AssetLoadPriority.Low);
        }
    }

    internal static void WriteTemplates()
    {
        if(Helper.Data.ReadJsonFile<ItemData>("Templates/Item/Model.json") is not null)
            return;
        
        ModEntry.Mon.Log("Writing file templates to mod folder...", LogLevel.Info);
        Helper.Data.WriteJsonFile("Templates/Item/Model.json", new ItemData());
        Helper.Data.WriteJsonFile("Templates/Item/LightData.json", new LightData());
        Helper.Data.WriteJsonFile("Templates/Item/OnBehavior.json", new OnBehavior());
        Helper.Data.WriteJsonFile("Templates/MixedSeeds.json", new Dictionary<string, List<MixedSeedData>>
        {
            { "ItemId", new(){ new MixedSeedData()}}
        });
        Helper.Data.WriteJsonFile("Templates/Resources/Model.json", new Dictionary<string, ResourceData>
        {
            { "ItemId", new() }
        });
        Helper.Data.WriteJsonFile("Templates/Resources/ExtraSpawn.json", new List<ExtraSpawn>
        {
            new()
        });
        Helper.Data.WriteJsonFile("Templates/MenuBehavior.json", new Dictionary<string, List<MenuBehavior>>
        {
            { "QualifiedItemId", new(){ new MenuBehavior()}}
        });
        Helper.Data.WriteJsonFile("Templates/EatingAnimation.json", new Dictionary<string,FarmerAnimation>
        {
            {"NameOfAnimation", new() { Animation = new[]{ new FarmerFrame() }, Food = new FoodAnimation()}}
        });
    }
}
