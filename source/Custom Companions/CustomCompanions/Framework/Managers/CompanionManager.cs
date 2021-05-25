/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using CustomCompanions.Framework.Companions;
using CustomCompanions.Framework.Models;
using CustomCompanions.Framework.Models.Companion;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCompanions.Framework.Managers
{
    internal class BoundCompanions
    {
        public RingModel SummoningRing { get; set; }
        public List<Companion> Companions { get; set; }

        public BoundCompanions(RingModel ring, List<Companion> companions)
        {
            SummoningRing = ring;
            Companions = companions;
        }
    }

    internal class SceneryCompanions
    {
        public GameLocation Location { get; set; }
        public Vector2 Tile { get; set; }
        public List<Companion> Companions { get; set; }

        public SceneryCompanions(GameLocation location, Vector2 tile, List<Companion> companions)
        {
            Location = location;
            Tile = tile;
            Companions = companions;
        }

        internal void ReplaceAssociatedCompanions(List<Companion> companions, GameLocation location)
        {
            foreach (var companion in this.Companions)
            {
                location.characters.Remove(companion);
            }

            Companions = companions;
        }
    }

    internal class CompanionManager
    {
        internal static List<CompanionModel> companionModels;
        internal static List<BoundCompanions> activeCompanions;
        internal static List<SceneryCompanions> sceneryCompanions;

        internal static void SummonCompanions(CompanionModel model, int numberToSummon, RingModel summoningRing, Farmer who, GameLocation location)
        {
            if (location.characters is null)
            {
                CustomCompanions.monitor.Log($"Unable to summon {model.Name} due to the location {location.Name} not having an instantiated GameLocation.characters!");
                return;
            }

            List<Companion> companions = new List<Companion>();
            for (int x = 0; x < numberToSummon; x++)
            {
                Companion companion = new Companion(model, who);
                location.characters.Add(companion);
                companions.Add(companion);
            }

            activeCompanions.Add(new BoundCompanions(summoningRing, companions));
        }

        internal static void SummonCompanions(CompanionModel model, int numberToSummon, Vector2 tile, GameLocation location)
        {
            if (location.characters is null)
            {
                CustomCompanions.monitor.Log($"Unable to summon {model.Name} due to the location {location.Name} not having an instantiated GameLocation.characters!");
                return;
            }

            List<Companion> companions = new List<Companion>();
            for (int x = 0; x < numberToSummon; x++)
            {
                MapCompanion companion = new MapCompanion(model, tile, location);
                companions.Add(companion);
            }

            var sceneryCompanion = new SceneryCompanions(location, tile, companions);
            if (sceneryCompanions.Any(s => s.Tile == tile && s.Location == location))
            {
                // Clear out old companions, add in the new ones
                foreach (var scenery in sceneryCompanions.Where(s => s.Tile == tile && s.Location == location))
                {
                    scenery.ReplaceAssociatedCompanions(companions, location);
                }
            }
            else
            {
                sceneryCompanions.Add(sceneryCompanion);
            }

            // Ensures each collision based companion is moved to an empty tile
            companions.ForEach(c => location.characters.Add(c));
            foreach (var companion in companions.Where(c => c.collidesWithOtherCharacters))
            {
                companion.PlaceInEmptyTile();
            }
        }

        internal static void RespawnCompanions(RingModel summoningRing, Farmer who, GameLocation location, bool removeFromActive = true)
        {
            var boundCompanions = activeCompanions.FirstOrDefault(a => a.SummoningRing == summoningRing);
            if (boundCompanions is null)
            {
                CustomCompanions.monitor.Log($"Unable to find summoning ring match to {summoningRing.Name}, will be unable to respawn companions!");
                return;
            }

            foreach (var companion in boundCompanions.Companions)
            {
                if (location.characters != null && !location.characters.Contains(companion))
                {
                    companion.ResetForNewLocation(location, who.getTileLocation());
                    location.characters.Add(companion);
                }
            }
        }

        internal static void RemoveCompanions(RingModel summoningRing, GameLocation location, bool removeFromActive = true)
        {
            var boundCompanions = activeCompanions.FirstOrDefault(a => a.SummoningRing == summoningRing);
            if (boundCompanions is null)
            {
                CustomCompanions.monitor.Log($"Unable to find summoning ring match to {summoningRing.Name}, will be unable to despawn companions!");
                return;
            }

            foreach (var companion in boundCompanions.Companions)
            {
                if (location.characters != null && location.characters.Contains(companion))
                {
                    companion.PrepareForDeletion();
                    location.characters.Remove(companion);
                }
            }

            if (removeFromActive)
            {
                activeCompanions.Remove(boundCompanions);
            }
        }

        internal static void RemoveSceneryCompanionsAtTile(GameLocation location, Vector2 tile)
        {
            foreach (var sceneryCompanion in sceneryCompanions.Where(s => !(s.Location == location && s.Tile == tile)).ToList())
            {
                sceneryCompanions.Remove(sceneryCompanion);
            }
        }

        internal static bool UpdateCompanionModel(CompanionModel model)
        {
            var cachedModel = companionModels.FirstOrDefault(c => c.GetId() == model.GetId());
            if (cachedModel is null)
            {
                CustomCompanions.monitor.Log($"No companion match found for {model.GetId()}; Did a content patch update the name?", StardewModdingAPI.LogLevel.Trace);
                return false;
            }

            // Update our cached model
            companionModels[companionModels.IndexOf(cachedModel)] = model;

            // Update any existing companions using this model
            foreach (var activeCompanion in activeCompanions.SelectMany(c => c.Companions).Where(c => c.model.GetId() == model.GetId()))
            {
                // Do model update
                activeCompanion.UpdateModel(model);
            }

            foreach (var sceneryCompanion in sceneryCompanions.SelectMany(c => c.Companions).Where(c => c.model.GetId() == model.GetId()))
            {
                // Do model update
                sceneryCompanion.UpdateModel(model);
            }

            return true;
        }

        internal static void RefreshLights(GameLocation location)
        {
            foreach (var companion in location.characters.Where(c => IsSceneryCompanion(c) && (c as MapCompanion).light != null))
            {
                MapCompanion mapCompanion = companion as MapCompanion;
                if (!Game1.currentLightSources.Contains(mapCompanion.light))
                {
                    Game1.currentLightSources.Add(mapCompanion.light);
                }
            }
        }

        internal static bool IsCustomCompanion(Character follower)
        {
            if (follower != null && follower is Companion)
            {
                return true;
            }

            return false;
        }

        internal static bool IsSceneryCompanion(Character follower)
        {
            if (follower != null && follower is MapCompanion)
            {
                return true;
            }

            return false;
        }

        internal static bool IsOrphan(Character follower, GameLocation location)
        {
            if (follower != null && follower is MapCompanion)
            {
                MapCompanion mapCompanion = follower as MapCompanion;

                var companionTile = mapCompanion.targetTile.Value / 64f;
                var backLayer = location.map.GetLayer("Back");
                if (backLayer.Tiles.Array.GetLength(0) < companionTile.X || backLayer.Tiles.Array.GetLength(1) < companionTile.Y)
                {
                    return true;
                }

                var mapTile = backLayer.Tiles[(int)companionTile.X, (int)companionTile.Y];
                if (mapTile is null || !mapTile.Properties.ContainsKey("CustomCompanions"))
                {
                    return true;
                }

                if (String.IsNullOrEmpty(mapTile.Properties["CustomCompanions"]))
                {
                    return true;
                }

                string command = mapTile.Properties["CustomCompanions"].ToString();
                if (command.Split(' ')[0].ToUpper() != "SPAWN")
                {
                    return true;
                }

                string companionKey = command.Substring(command.IndexOf(' ') + 2).TrimStart();
                if (!Int32.TryParse(command.Split(' ')[1], out int amountToSummon))
                {
                    amountToSummon = 1;
                    companionKey = command.Substring(command.IndexOf(' ') + 1);
                }

                var companion = companionModels.FirstOrDefault(c => String.Concat(c.Owner, ".", c.Name) == companionKey);
                if (companion is null)
                {
                    return true;
                }

                if (companion.GetId() != mapCompanion.companionKey)
                {
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}
