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

    public static Dictionary<string, List<GarbageCanEdit>> MapGarbageCanEdits = new();

    public class GarbageCanEdit
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Key { get; set; }
        public int SourceX { get; set; }
    }

    [SEvent.GameLaunchedLate]
    private void GameLaunched(object sender, GameLaunchedEventArgs e)
    {
        BirbSkill.Register("drbirbdev.Binning", ModEntry.Assets.SkillTexture, ModEntry.Instance.Helper, new Dictionary<string, object>()
        {
            {"Recycler", null },
            {"Sneak", null },
            {"Environmentalist", null },
            {"Salvager", null },
            {"Upseller", null },
            {"Reclaimer", new {
                extra = (ModEntry.Config.ReclaimerExtraValuePercent * 100).ToString("0.0"),
                pExtra = ((ModEntry.Config.ReclaimerPrestigeExtraValuePercent + ModEntry.Config.ReclaimerExtraValuePercent) * 100).ToString("0.0")
            } }
        }, PerkText, HoverText);
    }

    [SEvent.AssetRequested]
    public static void AddGarbageCans(object sender, AssetRequestedEventArgs e)
    {
        if (!e.Name.IsEquivalentTo("Data/GarbageCans"))
        {
            return;
        }

        e.Edit(asset =>
        {
            GarbageCanData garbageCanData = asset.GetData<GarbageCanData>();
            MapGarbageCanEdits.Clear();
            LoadGarbageCanEdits(garbageCanData);

            foreach (var can in garbageCanData.GarbageCans)
            {
                
                if (!can.Value.CustomFields.TryGetValue("drbirbdev.BinningSkill_AddToMap", out string data))
                {
                    continue;
                }

                string[] split = data.Split(" ");

                if (split.Length < 4)
                {
                    continue;
                }

                string level = split[3];
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

    private static List<string> PerkText(int level)
    {
        List<string> result = new()
        {
            ModEntry.Instance.I18n.Get("skill.perk.base", new { bonusPercent = (ModEntry.Config.PerLevelBaseDropChanceBonus * 100).ToString("0.0") }),
            ModEntry.Instance.I18n.Get("skill.perk.rare", new { bonusPercent = (ModEntry.Config.PerLevelRareDropChanceBonus * 100).ToString("0.0") })
        };
        if (level == ModEntry.Config.MegaMinLevel)
        {
            result.Add(ModEntry.Instance.I18n.Get("skill.perk.mega_drops"));
        }
        if (level == ModEntry.Config.DoubleMegaMinLevel)
        {
            result.Add(ModEntry.Instance.I18n.Get("skill.perk.double_mega_drops"));
        }

        return result;
    }

    private static string HoverText(int level)
    {
        return ModEntry.Instance.I18n.Get("skill.perk.base", new { bonusPercent = (level * ModEntry.Config.PerLevelBaseDropChanceBonus * 100).ToString("0.0") });
    }

    private uint PreviousTrashCansChecked;
    private int PiecesOfTrashUntilFriendshipIncrease;
    private uint PreviousPiecesOfTrashRecycled;
    private uint PreviousRecyclingBinsChecked;
    private uint PreviousFoodComposted;
    [SEvent.TimeChanged]
    private void TimeChanged(object sender, TimeChangedEventArgs e)
    {
        int recycled = (int)(Game1.stats.Get(StatKeys.PiecesOfTrashRecycled) - this.PreviousPiecesOfTrashRecycled);
        this.PreviousPiecesOfTrashRecycled = Game1.stats.Get(StatKeys.PiecesOfTrashRecycled);
        if (recycled > 0)
        {
            Skills.AddExperience(Game1.player, "drbirbdev.Binning", ModEntry.Config.ExperienceFromRecycling * recycled);
        }

        this.PiecesOfTrashUntilFriendshipIncrease -= recycled;
        if (Game1.player.HasProfession("Environmentalist") && this.PiecesOfTrashUntilFriendshipIncrease < 0)
        {
            this.PiecesOfTrashUntilFriendshipIncrease += ModEntry.Config.RecyclingCountToGainFriendship;

            int friendship = ModEntry.Config.RecyclingFriendshipGain;
            if (Game1.player.HasProfession("Environmentalist", true))
            {
                friendship += ModEntry.Config.RecyclingPrestigeFriendshipGain;
            }
            // TODO: figure out better region than hard-coding Town
            Utility.improveFriendshipWithEveryoneInRegion(Game1.player, friendship, "Town");
        }

        int trashChecked = (int)(Game1.player.stats.Get(StatKeys.TrashCansChecked) - this.PreviousTrashCansChecked);
        this.PreviousTrashCansChecked = Game1.stats.Get(StatKeys.TrashCansChecked);
        if (trashChecked > 0)
        {
            Skills.AddExperience(Game1.player, "drbirbdev.Binning", ModEntry.Config.ExperienceFromCheckingTrash * trashChecked);
        }

        int recyclingChecked = (int)(Game1.stats.Get("drbirbdev.BinningSkill_RecyclingBinsChecked") - this.PreviousRecyclingBinsChecked);
        this.PreviousRecyclingBinsChecked = Game1.stats.Get("drbirbdev.BinningSkill_RecyclingBinsChecked");
        if (recyclingChecked > 0)
        {
            Skills.AddExperience(Game1.player, "drbirbdev.Binning", ModEntry.Config.ExperienceFromCheckingRecycling * recyclingChecked);
        }

        int compostingChecked = (int)(Game1.stats.Get("drbirbdev.BinningSkill_FoodComposted") - this.PreviousFoodComposted);
        this.PreviousFoodComposted = Game1.stats.Get("drbirbdev.BinningSkill_FoodComposted");
        if (compostingChecked > 0)
        {
            Skills.AddExperience(Game1.player, "drbirbdev.Binning", ModEntry.Config.ExperienceFromComposting * compostingChecked);
        }
    }

    [SEvent.AssetRequested]
    private static void EditMap(object sender, AssetRequestedEventArgs e)
    {
        if (e.DataType != typeof(xTile.Map))
        {
            return;
        }

        if (MapGarbageCanEdits.Count == 0)
        {
            LoadGarbageCanEdits(Game1.content.Load<GarbageCanData>("Data/GarbageCans"));
        }

        if (!MapGarbageCanEdits.TryGetValue(e.Name.ToString(), out List<GarbageCanEdit> edits))
        {
            return;
        }

        e.Edit(asset =>
        {
            var editor = asset.AsMap();

            editor.Data.AddTileSheet(new TileSheet("z_trashcans", editor.Data, ModEntry.Assets.TrashCanTilesheetAssetName.Name, new Size(5, 2), new Size(16, 16)));
            
            foreach (GarbageCanEdit edit in edits) {
                Location botLocation = new Location(edit.X, edit.Y);
                xTile.Layers.Layer buildingLayer = editor.Data.GetLayer("Buildings");
                buildingLayer.Tiles[botLocation] = new StaticTile(buildingLayer, editor.Data.GetTileSheet("z_trashcans"), BlendMode.Alpha, 5 + edit.SourceX);
                buildingLayer.Tiles[botLocation].Properties["Action"] = $"Garbage {edit.Key}";

                Location topLocation = new Location(edit.X, edit.Y - 1);
                xTile.Layers.Layer frontLayer = editor.Data.GetLayer("Front");
                frontLayer.Tiles[topLocation] = new StaticTile(frontLayer, editor.Data.GetTileSheet("z_trashcans"), BlendMode.Alpha, edit.SourceX);
            }
        });
    }

    private static void LoadGarbageCanEdits(GarbageCanData garbageCanData)
    {
        foreach (KeyValuePair<string, GarbageCanEntryData> entry in garbageCanData.GarbageCans)
        {
            if (!entry.Value.CustomFields.TryGetValue("drbirbdev.BinningSkill_AddToMap", out string data))
            {
                continue;
            }
            string[] split = data.Split(" ");
            if (split.Length < 3)
            {
                Log.Error($"AddToMap expectes at least 3 arguments, <map> <x> <y> [level = Default], but got {data}");
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
            if (!MapGarbageCanEdits.TryGetValue(mapName, out List<GarbageCanEdit> edits))
            {
                edits = new List<GarbageCanEdit>();
                MapGarbageCanEdits[mapName] = edits;
            }
            edits.Add(new GarbageCanEdit()
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
