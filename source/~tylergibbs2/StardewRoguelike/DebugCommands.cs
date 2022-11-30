/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Text;
using StardewModdingAPI;
using StardewRoguelike.UI;
using StardewValley;
using StardewValley.Locations;
using System.Reflection;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Network;
using StardewRoguelike.Bosses;
using System.Text.Json;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using StardewRoguelike.Extensions;
using StardewValley.TerrainFeatures;
using System.Linq;
using System.IO;

namespace StardewRoguelike
{
    internal class DebugCommands
    {
        public static float ForcedStoneChance = 0.0f;

        public static float ForcedGemChance = 0.0f;

        public static float ForcedMonsterChance = 0.0f;

        public static float ForcedDarkChance = 0.0f;

        public static int ForcedBossIndex = -1;

        public static int ForcedChallengeIndex = -1;

        public static bool ForcedForgeFloor = false;

        public static bool ForcedChestFloor = false;

        public static float ForcedDifficulty = 0f;

        public static bool ForcedFortuneTeller = false;

        public static bool ForcedGil = false;

        public static void Parse(string command, string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            switch (args[0])
            {
                case "help":
                    PrintHelp();
                    break;
                case "perk":
                    if (args.Length == 1)
                    {
                        var values = Enum.GetValues<Perks.PerkType>();
                        for (int i = 0; i < values.Length; i++)
                            ModEntry.ModMonitor.Log($"{i} = {values[i]}", LogLevel.Info);
                    }
                    else
                    {
                        Perks.PerkType perk = (Perks.PerkType)int.Parse(args[1]);
                        Perks.AddPerk(perk);
                    }
                    break;
                case "perkpick":
                    Game1.activeClickableMenu = new PerkMenu();
                    break;
                case "perks":
                    for (int i = 0; i < Enum.GetValues<Perks.PerkType>().Length; i++)
                        Perks.AddPerk(Perks.GetRandomUniquePerk()!.Value);
                    break;
                case "clearperks":
                    Perks.RemoveAllPerks();
                    break;
                case "curse":
                    if (args.Length == 1)
                    {
                        var values = Enum.GetValues<CurseType>();
                        for (int i = 0; i < values.Length; i++)
                            ModEntry.ModMonitor.Log($"{i} = {values[i]}", LogLevel.Info);
                    }
                    else
                    {
                        var values = Enum.GetValues<CurseType>();
                        CurseType curse = values[int.Parse(args[1])];
                        Curse.AddCurse(curse);
                    }
                    break;
                case "curses":
                    var allCurses = Enum.GetValues<CurseType>();
                    for (int i = 0; i < allCurses.Length; i++)
                        Curse.AddCurse(allCurses[i]);
                    break;
                case "clearcurses":
                    Curse.RemoveAllCurses();
                    break;
                case "ladder":
                    ForceLadder(args);
                    break;
                case "reset":
                    ModEntry.MultiplayerHelper.SendMessage(
                        "GameOver",
                        "GameOver"
                    );
                    Roguelike.GameOver();
                    break;
                case "spectate":
                    SpectatorMode.EnterSpectatorMode();
                    break;
                case "unspectate":
                    SpectatorMode.ExitSpectatorMode();
                    break;
                case "seed":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log($"Floor generation seed: {Roguelike.FloorRngSeed}", LogLevel.Info);
                    else
                    {
                        Roguelike.FloorRngSeed = int.Parse(args[1]);
                        Roguelike.FloorRng = new(Roguelike.FloorRngSeed);
                        ChallengeFloor.History.Clear();
                        Roguelike.SeenMineMaps.Clear();
                    }
                    break;
                case "stonechance":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log($"Stone chance is currently: {ForcedStoneChance}", LogLevel.Info);
                    else
                        ForcedStoneChance = float.Parse(args[1]);
                    break;
                case "gemchance":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log($"Gem chance is currently: {ForcedGemChance}", LogLevel.Info);
                    else
                        ForcedGemChance = float.Parse(args[1]);
                    break;
                case "monsterchance":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log($"Monster chance is currently: {ForcedMonsterChance}", LogLevel.Info);
                    else
                        ForcedMonsterChance = float.Parse(args[1]);
                    break;
                case "darkchance":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log($"Dark chance is currently: {ForcedDarkChance}", LogLevel.Info);
                    else
                        ForcedDarkChance = float.Parse(args[1]);
                    break;
                case "hardmode":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log($"Hard mode is currently: {Roguelike.HardMode}", LogLevel.Info);
                    else
                    {
                        Roguelike.HardMode = bool.Parse(args[1]);
                        ModEntry.ActiveStats.HardMode = bool.Parse(args[1]);
                    }
                    break;
                case "printstats":
                    ModEntry.ModMonitor.Log(JsonSerializer.Serialize(ModEntry.ActiveStats), LogLevel.Info);
                    break;
                case "uploadstats":
                    ModEntry.ActiveStats.Upload();
                    break;
                case "forceboss":
                    if (args.Length == 1)
                    {
                        ModEntry.ModMonitor.Log($"-1 = disabled", LogLevel.Info);
                        var bosses = BossManager.GetFlattenedBosses();
                        for (int i = 0; i < bosses.Count; i++)
                            ModEntry.ModMonitor.Log($"{i} = {bosses[i]}", LogLevel.Info);
                    }
                    else
                        ForcedBossIndex = int.Parse(args[1]);
                    break;
                case "forcechallenge":
                    if (args.Length == 1)
                    {
                        ModEntry.ModMonitor.Log($"-1 = disabled", LogLevel.Info);
                        var values = Enum.GetValues<ChallengeFloor.ChallengeType>();
                        for (int i = 0; i < values.Length; i++)
                            ModEntry.ModMonitor.Log($"{i} = {values[i]}", LogLevel.Info);
                    }
                    else
                        ForcedChallengeIndex = int.Parse(args[1]);
                    break;
                case "forceforge":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log($"Forge is currently: {ForcedForgeFloor}", LogLevel.Info);
                    else
                        ForcedForgeFloor = bool.Parse(args[1]);
                    break;
                case "forcechest":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log($"Force chest is currently: {ForcedChestFloor}", LogLevel.Info);
                    else
                        ForcedChestFloor = bool.Parse(args[1]);
                    break;
                case "forcefortune":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log($"Forced fortune is currently: {ForcedFortuneTeller}", LogLevel.Info);
                    else
                        ForcedFortuneTeller = bool.Parse(args[1]);
                    break;
                case "forcegil":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log($"Forced gil is currently: {ForcedGil}", LogLevel.Info);
                    else
                        ForcedGil = bool.Parse(args[1]);
                    break;
                case "difficulty":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log("Invalid usage: difficulty float", LogLevel.Error);
                    else
                        ForcedDifficulty = float.Parse(args[1]);
                    break;
                case "level":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log($"Level is currently: {Roguelike.CurrentLevel}", LogLevel.Info);
                    else
                        Roguelike.CurrentLevel = int.Parse(args[1]);
                    break;
                case "stamina":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log($"Stamina is currently: {Game1.player.Stamina}", LogLevel.Info);
                    else
                        Game1.player.Stamina = int.Parse(args[1]);
                    break;
                case "genmines":
                    if (args.Length == 1)
                        ModEntry.ModMonitor.Log($"Mines in memory: active={MineShaft.activeMines.Count}", LogLevel.Info);
                    else
                    {
                        int highestLevel = Roguelike.GetHighestMineShaftLevel();
                        if (highestLevel == 0)
                        {
                            ModEntry.ModMonitor.Log($"Enter the first level before using this command.", LogLevel.Error);
                            return;
                        }

                        int toGen = int.Parse(args[1]);
                        for (int i = highestLevel + 1; i <= highestLevel + toGen; i++)
                            MineShaft.GetMine($"UndergroundMine1/{i}");
                    }
                    break;
                case "gamba":
                    Game1.activeClickableMenu = new GambaMenu();
                    break;
                case "skills":
                    ModEntry.ModMonitor.Log($"luck: base={Game1.player.luckLevel.Value} added={Game1.player.addedLuckLevel} daily={Game1.player.DailyLuck} average={Game1.player.team.AverageLuckLevel()}", LogLevel.Info);
                    ModEntry.ModMonitor.Log($"speed: base={Game1.player.speed} added={Game1.player.addedSpeed} result={Game1.player.getMovementSpeed()}", LogLevel.Info);
                    break;
                case "food":
                    ModEntry.ModMonitor.Log($"food: {Game1.buffsDisplay.food}", LogLevel.Info);
                    ModEntry.ModMonitor.Log($"drink: {Game1.buffsDisplay.drink}", LogLevel.Info);
                    break;
                case "boulder":
                    MineShaft mine = (MineShaft)Game1.player.currentLocation;
                    int whichClump = (Game1.random.NextDouble() < 0.5) ? 752 : 754;
                    if (mine.getMineArea() == 40)
                    {
                        if (mine.GetAdditionalDifficulty() > 0)
                        {
                            whichClump = 600;
                            if (Roguelike.FloorRng.NextDouble() < 0.1)
                                whichClump = 602;
                        }
                        else
                            whichClump = ((Roguelike.FloorRng.NextDouble() < 0.5) ? 756 : 758);
                    }
                    mine.resourceClumps.Add(new ResourceClump(whichClump, 2, 2, Game1.currentCursorTile));
                    break;
                case "exportmap":
                    ExportMap();
                    break;
                case "hatboard":
                    Game1.activeClickableMenu = new HatBoard();
                    break;
                default:
                    ModEntry.ModMonitor.Log("Invalid command.", LogLevel.Error);
                    break;
            };
        }

        public static void PrintHelp()
        {
            StringBuilder help = new();

            help.AppendLine("Debug Commands:");
            help.AppendLine("perk [int:which] : gives yourself a specific perk");
            help.AppendLine("perkpick : opens up the perk picking menu");
            help.AppendLine("perks : gives yourself all perks");
            help.AppendLine("clearperks : clears all perks");
            help.AppendLine("curse [int:which] : gives yourself a specific curse");
            help.AppendLine("curses : gives yourself all curses");
            help.AppendLine("clearcurses : clears all curses");
            help.AppendLine("ladder <int:tile x> <int:tile y> : forces a ladder to spawn");
            help.AppendLine("reset : force resets the run");
            help.AppendLine("spectate : enter spectator mode");
            help.AppendLine("unspectate : exit spectator mode");
            help.AppendLine("seed [int:seed] : sets/views the seed for floor generation");
            help.AppendLine("stonechance <float:chance> : sets the chance for stone spawns");
            help.AppendLine("gemchance <float:chance> : sets the chance for gem spawns");
            help.AppendLine("monsterchance <float:chance> : sets the chance for monster spawns");
            help.AppendLine("darkchance <float:chance> : sets the chance for dark floors on hard mode");
            help.AppendLine("hardmode [bool:status] : enable or disable hard mode");
            help.AppendLine("printstats : prints the stats upload payload");
            help.AppendLine("uploadstats : forces a stats upload");
            help.AppendLine("forceboss [int:which] : forces boss floors to appear when possible");
            help.AppendLine("forcechallenge [int:which] : forces challenge floors to appear when possible");
            help.AppendLine("forceforge [bool:status] : forces forge floors to appear when possible");
            help.AppendLine("forcechest [bool:status] : forces chest floors to appear when possible");
            help.AppendLine("forcefortune [bool:status] : force the fortune teller to spawn");
            help.AppendLine("forcegil [bool:status] : force gil to spawn");
            help.AppendLine("difficulty <float:new> : forces the difficulty to be at a certain value");
            help.AppendLine("level <int:new> : sets the current level");
            help.AppendLine("stamina [int:new] : sets the player's stamina");
            help.AppendLine("genmines <int:amount> : generates amount mines and stores in memory");
            help.AppendLine("gamba : opens the gamba wheel menu");
            help.AppendLine("skills : displays the player's luck and speed skills");
            help.AppendLine("food : displays active food and drink buffs");
            help.AppendLine("boulder : spawns a resource clump (boulder/log) at cursor tile");
            help.AppendLine("exportmap : exports the current location to a tmx file");
            help.AppendLine("hatboard : opens the hat board menu");

            ModEntry.ModMonitor.Log(help.ToString(), LogLevel.Info);
        }

        public static void ButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.RightAlt)
                ForceLadder(new string[] { "", $"{Game1.currentCursorTile.X}", $"{Game1.currentCursorTile.Y}" });
            else if (e.Button == SButton.RightControl)
            {
                MineShaft mine = (MineShaft)Game1.player.currentLocation;
                mine.SpawnLocalChest(new(Game1.currentCursorTile.X, Game1.currentCursorTile.Y));
            }
        }

        public static void ForceLadder(string[] args)
        {
            if (args.Length != 3)
            {
                ModEntry.ModMonitor.Log("Invalid usage: ladder x y", LogLevel.Error);
                return;
            }
            else if (Game1.player.currentLocation is not MineShaft)
            {
                ModEntry.ModMonitor.Log("You are not in a mineshaft.", LogLevel.Error);
                return;
            }

            MineShaft mine = (MineShaft)Game1.player.currentLocation;

            int x = int.Parse(args[1]);
            int y = int.Parse(args[2]);

            NetVector2Dictionary<bool, NetBool> createLadderEvent = (NetVector2Dictionary<bool, NetBool>)mine.GetType().GetField("createLadderAtEvent", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(mine)!;
            createLadderEvent[new Vector2(x, y)] = true;
        }

        public static void ExportMap()
        {
            xTile.Map map = Game1.player.currentLocation.Map;

            TMXTile.TMXFormat Format = new(Game1.tileSize / Game1.pixelZoom, Game1.tileSize / Game1.pixelZoom, Game1.pixelZoom, Game1.pixelZoom);
            TMXTile.TMXMap tmxMap = Format.Store(map);

            foreach (var objectGroup in tmxMap.Objectgroups)
            {
                for (int i = 0; i < objectGroup.Objects.Count; i++)
                {
                    var tmxObject = objectGroup.Objects[i];
                    tmxObject.Properties = tmxObject.Properties.Skip(2).ToArray();
                    if (tmxObject.Properties.Length == 0)
                        objectGroup.Objects[i] = null;
                }
            }

            foreach (var tileset in tmxMap.Tilesets)
                tileset.Image.Source = Path.GetFileName(tileset.Image.Source);

            var parser = new TMXTile.TMXParser();
            parser.Export(tmxMap, "_map.tmx", TMXTile.DataEncodingType.XML);

            Console.WriteLine("Map successfully exported.");
        }
    }
}
