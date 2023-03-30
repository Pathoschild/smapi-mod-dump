/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.StringHandler;

using AtraCore.Framework.Caches;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;

namespace StopRugRemoval.Framework.Niceties;

/// <summary>
/// Detects and tries to fix up duplicate NPCs.
/// </summary>
internal static class DuplicateNPCDetector
{
    /// <inheritdoc cref="IGameLoopEvents.DayEnding"/>
    internal static void DayEnd()
    {
        if (Context.IsMainPlayer)
        {
            DetectDuplicateNPCs();
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
    internal static void DayStart()
    {
        if (!Context.IsMainPlayer)
        {
            return;
        }

        HashSet<string> found = new();
        bool leoMoved = Game1.MasterPlayer.mailReceived.Contains("LeoMoved");

        foreach (NPC? character in Utility.getAllCharacters())
        {
            found.Add(character.Name);

            if (character.Name == "Leo" && leoMoved && character.DefaultMap != "LeoTreeHouse")
            {
                ModEntry.ModMonitor.Log("Fixing Leo's move.", LogLevel.Info);

                try
                {
                    // derived from the OnRequestLeoMoveEvent.
                    character.DefaultMap = "LeoTreeHouse";
                    character.DefaultPosition = new Vector2(5f, 4f) * 64f;
                    character.faceDirection(2);
                    character.InvalidateMasterSchedule();
                    if (character.Schedule is not null)
                    {
                        character.Schedule = null;
                    }
                    character.controller = null;
                    character.temporaryController = null;
                    Game1.warpCharacter(character, Game1.getLocationFromName("LeoTreeHouse"), new Vector2(5f, 4f));
                    character.Halt();
                    character.ignoreScheduleToday = false;

                    // fix up his schedule too.
                    character.Schedule = character.getSchedule(Game1.dayOfMonth);
                }
                catch (Exception ex)
                {
                    ModEntry.ModMonitor.Log($"Failed while trying to fix Leo's move: \n\n{ex}");
                }
            }
        }

        foreach ((string name, string dispo) in Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions"))
        {
            if (found.Contains(name) || (Game1.year <= 1 && name == "Kent") || (name == "Leo" && !Game1.MasterPlayer.hasOrWillReceiveMail("addedParrotBoy")))
            {
                continue;
            }
            try
            {
                StreamSplit defaultpos = dispo.GetNthChunk('/', 10)
                                              .StreamSplit(null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (!defaultpos.MoveNext())
                {
                    ModEntry.ModMonitor.Log($"Badly formatted dispo for npc {name} - {dispo}", LogLevel.Warn);
                    continue;
                }

                string mapstring;
                if (name == "Leo" && leoMoved)
                {
                    mapstring = "LeoTreeHouse";
                }
                else
                {
                    mapstring = defaultpos.Current.ToString();
                }

                if (Game1.getLocationFromName(mapstring) is not GameLocation map)
                {
                    ModEntry.ModMonitor.Log($"{name} has a dispo entry for map {mapstring} which could not be found.", LogLevel.Warn);
                    continue;
                }

                int x, y;
                if (name == "Leo" && leoMoved)
                {
                    x = 5;
                    y = 4;
                }
                else
                {
                    if (!defaultpos.MoveNext() || int.TryParse(defaultpos.Current, out x))
                    {
                        ModEntry.ModMonitor.Log($"Badly formatted dispo for npc {name}  - {dispo}", LogLevel.Warn);
                        continue;
                    }

                    if (!defaultpos.MoveNext() || int.TryParse(defaultpos.Current, out y))
                    {
                        ModEntry.ModMonitor.Log($"Badly formatted dispo for npc {name}  - {dispo}", LogLevel.Warn);
                        continue;
                    }
                }

                ModEntry.ModMonitor.Log($"Found missing NPC {name}, adding");

                NPC npc = new(
                    sprite: new AnimatedSprite(@"Characters\" + NPC.getTextureNameForCharacter(name), 0, 16, 32),
                    position: new Vector2(x, y) * 64f,
                    defaultMap: mapstring,
                    facingDir: 0,
                    name: name,
                    schedule: null,
                    portrait: Game1.content.Load<Texture2D>(@"Portraits\" + NPC.getTextureNameForCharacter(name)),
                    eventActor: false);
                map.addCharacter(npc);
                try
                {
                    npc.Schedule = npc.getSchedule(Game1.dayOfMonth);
                }
                catch (Exception ex)
                {
                    ModEntry.ModMonitor.Log($"Failed to restore schedule for missing NPC {name}\n\n{ex}", LogLevel.Warn);
                }

                // TODO: may need to fix up their dialogue as well?
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"Failed to add missing npc {name}\n\n{ex}", LogLevel.Warn);
            }
        }

        DetectDuplicateNPCs();
    }

    private static Dictionary<string, NPC> DetectDuplicateNPCs()
    {
        Dictionary<string, NPC> found = new();
        foreach (GameLocation loc in Game1.locations)
        {
            for (int i = loc.characters.Count - 1; i >= 0; i--)
            {
                NPC character = loc.characters[i];
                if (!character.isVillager() || character.GetType() != typeof(NPC))
                {
                    continue;
                }

                // let's populate AtraCore's cache while we're at it.
                _ = NPCCache.TryInsert(character);

                if (!found.TryAdd(character.Name, character) && character.Name != "Mister Qi")
                {
                    ModEntry.ModMonitor.Log($"Found duplicate NPC {character.Name}", LogLevel.Info);
                    if (ReferenceEquals(character, found[character.Name]))
                    {
                        ModEntry.ModMonitor.Log("    These appear to be the same instance.", LogLevel.Info);
                    }

                    if (ModEntry.Config.RemoveDuplicateNPCs)
                    {
                        loc.characters.RemoveAt(i);
                        ModEntry.ModMonitor.Log("    Removing duplicate.", LogLevel.Info);
                    }
                }
            }
        }

        return found;
    }
}
