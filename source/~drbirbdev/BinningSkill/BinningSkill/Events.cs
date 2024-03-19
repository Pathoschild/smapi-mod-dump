/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using BirbCore.Attributes;
using BirbShared;
using SpaceCore;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Constants;
using StardewValley.GameData.GarbageCans;
using xTile.Dimensions;
using xTile.Tiles;

namespace BinningSkill;

[SEvent]
internal class Events
{
    private static readonly Dictionary<string, List<GarbageCanEdit>> MAP_GARBAGE_CAN_EDITS = new();

    private class GarbageCanEdit
    {
        public int X { get; init; }
        public int Y { get; init; }
        public string Key { get; init; }
        public int SourceX { get; init; }
    }

    [SEvent.DayStarted]
    private void GameStarted(object sender, DayStartedEventArgs e)
    {
        ModEntry.UnderleveledCheckedGarbage.Value = new HashSet<string>();
    }

    [SEvent.GameLaunchedLate]
    private void GameLaunched(object sender, GameLaunchedEventArgs e)
    {
        BirbSkill.Register("drbirbdev.Binning", ModEntry.Assets.SkillTexture, ModEntry.Instance.Helper,
            new Dictionary<string, object>
            {
                { "Recycler", null },
                { "Sneak", null },
                { "Environmentalist", new
                    {
                        amount = (ModEntry.Config.EnvironmentalistRecyclingCountToGainFriendship),
                        extra = (ModEntry.Config.EnvironmentalistRecyclingFriendshipGain),
                        pExtra = (ModEntry.Config.EnvironmentalistRecyclingFriendshipGain + ModEntry.Config.EnvironmentalistPrestigeRecyclingFriendshipGain)
                    }
                },
                { "Salvager", null },
                { "Upseller", null },
                {
                    "Reclaimer", new
                    {
                        extra = (ModEntry.Config.ReclaimerExtraValuePercent * 100).ToString("0.0"),
                        pExtra = ((ModEntry.Config.ReclaimerPrestigeExtraValuePercent +
                                   ModEntry.Config.ReclaimerExtraValuePercent) * 100).ToString("0.0")
                    }
                }
            }, PerkText, HoverText);
    }

    private static List<string> PerkText(int level)
    {
        List<string> result =
        [
            ModEntry.Instance.I18N.Get("skill.perk.base",
                new { bonusPercent = (ModEntry.Config.PerLevelBaseDropChanceBonus * 100).ToString("0.0") }),
            ModEntry.Instance.I18N.Get("skill.perk.rare",
                new { bonusPercent = (ModEntry.Config.PerLevelRareDropChanceBonus * 100).ToString("0.0") })
        ];
        if (level == ModEntry.Config.MegaMinLevel)
        {
            result.Add(ModEntry.Instance.I18N.Get("skill.perk.mega_drops"));
        }

        if (level == ModEntry.Config.DoubleMegaMinLevel)
        {
            result.Add(ModEntry.Instance.I18N.Get("skill.perk.double_mega_drops"));
        }

        return result;
    }

    private static string HoverText(int level)
    {
        return ModEntry.Instance.I18N.Get("skill.perk.base",
            new { bonusPercent = (level * ModEntry.Config.PerLevelBaseDropChanceBonus * 100).ToString("0.0") });
    }

    [SEvent.AssetRequested]
    public static void AddGarbageCans(object _, AssetRequestedEventArgs e)
    {
        if (!e.Name.IsEquivalentTo("Data/GarbageCans"))
        {
            return;
        }

        e.Edit(asset =>
        {
            GarbageCanData garbageCanData = asset.GetData<GarbageCanData>();
            int garbageHatIndex = garbageCanData.BeforeAll.FindIndex(item => item.Id == "Base_GarbageHat");
            garbageCanData.BeforeAll.RemoveAt(garbageHatIndex);

            MAP_GARBAGE_CAN_EDITS.Clear();
            LoadGarbageCanEdits(garbageCanData);

            foreach (var can in garbageCanData.GarbageCans)
            {
                can.Value.Items ??= [];

                if (can.Value.CustomFields is null ||
                    !can.Value.CustomFields.TryGetValue("drbirbdev.BinningSkill_AddToMap", out string data))
                {
                    can.Value.Items.Insert(0, GetGarbageHatItemData("Default"));
                    continue;
                }

                string[] split = data.Split(" ");

                if (split.Length < 3)
                {
                    continue;
                }

                string level = "Default";
                if (split.Length > 3)
                {
                    level = split[3];
                }

                can.Value.Items.Insert(0, GetGarbageHatItemData(level));
                if (level == "Default")
                {
                    continue;
                }

                can.Value.CustomFields["drbirbdev.BinningSkill_MinLevel"] = level switch
                {
                    "Copper" => "2",
                    "Iron" => "4",
                    "Gold" => "6",
                    "Iridium" => "8",
                    "Radioactive" => "10",
                    "Prismatic" => "12",
                    _ => "2"
                };

                can.Value.CustomFields["drbirbdev.BinningSkill_AnimationTexture"] = level switch
                {
                    "Copper" => ModEntry.Assets.AnimationCopperAssetName.Name,
                    "Iron" => ModEntry.Assets.AnimationIronAssetName.Name,
                    "Gold" => ModEntry.Assets.AnimationGoldAssetName.Name,
                    "Iridium" => ModEntry.Assets.AnimationIridiumAssetName.Name,
                    "Radioactive" => ModEntry.Assets.AnimationRadioactiveAssetName.Name,
                    "Prismatic" => ModEntry.Assets.AnimationPrismaticAssetName.Name,
                    _ => ModEntry.Assets.AnimationCopperAssetName.Name
                };

                can.Value.CustomFields["drbirbdev.BinningSkill_NoiseLevel"] = level switch
                {
                    "Copper" => "7",
                    "Iron" => "8",
                    "Gold" => "9",
                    "Iridium" => "10",
                    "Radioactive" => "11",
                    "Prismatic" => "12",
                    _ => "7"
                };
            }
        });
    }

    private static GarbageCanItemData GetGarbageHatItemData(string level)
    {
        GarbageCanItemData item = new()
        {
            IgnoreBaseChance = true,
            IsDoubleMegaSuccess = true,
            AddToInventoryDirectly = true,
            Condition = "PLAYER_STAT Current trashCansChecked 20, RANDOM .002",
            Id = "drbirbdev.BinningSkill_GarbageHat",
            ItemId = level switch
            {
                "Default" => "(H)66",
                "Copper" => "(H)drbirbdev.BinningSkill_CopperGarbageHat",
                "Iron" => "(H)drbirbdev.BinningSkill_IronGarbageHat",
                "Gold" => "(H)drbirbdev.BinningSkill_GoldGarbageHat",
                "Iridium" => "(H)drbirbdev.BinningSkill_IridiumGarbageHat",
                "Radioactive" => "(H)drbirbdev.BinningSkill_RadioactiveGarbageHat",
                "Prismatic" => "(H)drbirbdev.BinningSkill_PrismaticGarbageHat",
                _ => "(H)drbirbdev.BinningSkill_CopperGarbageHat"
            }
        };

        return item;
    }

    [SEvent.StatChanged(StatKeys.PiecesOfTrashRecycled)]
    private void StatChanged_PiecesOfTrashRecycled(object sender, SEvent.StatChanged.EventArgs e)
    {
        Skills.AddExperience(Game1.player, "drbirbdev.Binning", ModEntry.Config.ExperienceFromRecycling * e.Delta);

        if (!Game1.player.HasProfession("Environmentalist"))
        {
            return;
        }

        for (uint i = e.OldStat + 1; i <= e.NewStat; i++)
        {
            if (i % ModEntry.Config.EnvironmentalistRecyclingCountToGainFriendship != 0)
            {
                continue;
            }

            int friendship = ModEntry.Config.EnvironmentalistRecyclingFriendshipGain;
            if (Game1.player.HasProfession("Environmentalist", true))
            {
                friendship += ModEntry.Config.EnvironmentalistPrestigeRecyclingFriendshipGain;
            }

            // TODO: figure out better region than hard-coding Town
            Utility.improveFriendshipWithEveryoneInRegion(Game1.player, friendship, "Town");
        }
    }

    [SEvent.StatChanged(StatKeys.TrashCansChecked)]
    private void StatChanged_TrashCansChecked(object sender, SEvent.StatChanged.EventArgs e)
    {
        Skills.AddExperience(Game1.player, "drbirbdev.Binning",
            ModEntry.Config.ExperienceFromCheckingTrash * e.Delta);
    }

    [SEvent.StatChanged("drbirbdev.BinningSkill_RecyclingBinsChecked")]
    private void StatChanged_RecyclingBinsChecked(object sender, SEvent.StatChanged.EventArgs e)
    {
        Skills.AddExperience(Game1.player, "drbirbdev.Binning",
            ModEntry.Config.ExperienceFromCheckingRecycling * e.Delta);
    }

    [SEvent.StatChanged("drbirbdev.BinningSkill_FoodComposted")]
    private void StatChanged_FoodComposted(object sender, SEvent.StatChanged.EventArgs e)
    {
        Skills.AddExperience(Game1.player, "drbirbdev.Binning",
            ModEntry.Config.ExperienceFromComposting * e.Delta);
    }

    [SEvent.AssetRequested]
    private static void EditMap(object sender, AssetRequestedEventArgs e)
    {
        if (e.DataType != typeof(xTile.Map))
        {
            return;
        }

        if (MAP_GARBAGE_CAN_EDITS.Count == 0)
        {
            LoadGarbageCanEdits(DataLoader.GarbageCans(Game1.content));
        }

        if (!MAP_GARBAGE_CAN_EDITS.TryGetValue(e.Name.ToString() ?? string.Empty, out List<GarbageCanEdit> edits))
        {
            return;
        }

        e.Edit(asset =>
        {
            var editor = asset.AsMap();

            Size tilesheetSize = new(ModEntry.Assets.TrashCanTilesheet.Width / 16,
                ModEntry.Assets.TrashCanTilesheet.Height / 16);

            editor.Data.AddTileSheet(new TileSheet("z_trashcans", editor.Data,
                ModEntry.Assets.TrashCanTilesheetAssetName.Name, tilesheetSize, new Size(16, 16)));

            foreach (GarbageCanEdit edit in edits)
            {
                Location bottomLocation = new(edit.X, edit.Y);
                xTile.Layers.Layer buildingLayer = editor.Data.GetLayer("Buildings");
                buildingLayer.Tiles[bottomLocation] = new StaticTile(buildingLayer,
                    editor.Data.GetTileSheet("z_trashcans"), BlendMode.Alpha, 7 + edit.SourceX);
                buildingLayer.Tiles[bottomLocation].Properties["Action"] = $"Garbage {edit.Key}";

                Location topLocation = new(edit.X, edit.Y - 1);
                xTile.Layers.Layer frontLayer = editor.Data.GetLayer("Front");
                frontLayer.Tiles[topLocation] = new StaticTile(frontLayer, editor.Data.GetTileSheet("z_trashcans"),
                    BlendMode.Alpha, edit.SourceX);
            }
        });
    }

    private static void LoadGarbageCanEdits(GarbageCanData garbageCanData)
    {
        foreach (KeyValuePair<string, GarbageCanEntryData> entry in garbageCanData.GarbageCans)
        {
            if (entry.Value.CustomFields is null)
            {
                continue;
            }

            if (!entry.Value.CustomFields.TryGetValue("drbirbdev.BinningSkill_AddToMap", out string data))
            {
                continue;
            }

            string[] split = data.Split(" ");
            if (split.Length < 3)
            {
                Log.Error($"AddToMap expected at least 3 arguments, <map> <x> <y> [level = Default], but got {data}");
            }

            string mapName = split[0];

            if (!int.TryParse(split[1], out int x) ||
                !int.TryParse(split[2], out int y))
            {
                Log.Error($"Could not get x and y coordinates from {data} for garbage can {entry.Key}");
                continue;
            }

            string level = "Default";
            if (split.Length > 3)
            {
                level = split[3];
            }

            if (!MAP_GARBAGE_CAN_EDITS.TryGetValue(mapName, out List<GarbageCanEdit> edits))
            {
                edits = [];
                MAP_GARBAGE_CAN_EDITS[mapName] = edits;
            }

            edits.Add(new GarbageCanEdit
            {
                X = x,
                Y = y,
                Key = entry.Key,
                SourceX = level switch
                {
                    "Default" => 0,
                    "Copper" => 1,
                    "Iron" => 2,
                    "Gold" => 3,
                    "Iridium" => 4,
                    "Radioactive" => 5,
                    "Prismatic" => 6,
                    _ => 1
                }
            });
        }
    }
}
