/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;

namespace PersonalEffects
{
    public class NPCs
    {
        //attempt to return the name of every character in the game in a list
        public HashSet<string> GetAllCharacterNames(bool onlyDateable = false, bool onlyVillager = false, GameLocation onlyThisLocation = null)
        {
            HashSet<string> characters = new HashSet<string>(); //hashset ensures only unique values exist
            if (onlyThisLocation != null)
            {
                foreach (var c in onlyThisLocation.characters)
                {
                    if (!string.IsNullOrWhiteSpace(c.Name))
                    {
                        if (!onlyVillager || c.isVillager())
                            if (!onlyDateable || c.datable.Value) characters.Add(c.Name);
                    }
                }
                return characters; //only checking the one location
            }
            //start with NPCDispositions
            Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
            foreach (string s in dictionary.Keys)
            {
                var c = Game1.getCharacterFromName(s, onlyVillager);
                if (c != null) //simple nullcheck to ensure they weren't removed
                {
                    if (!onlyDateable || c.datable.Value)
                        if (!string.IsNullOrWhiteSpace(c.Name))
                            characters.Add(c.Name);
                }
            }
            //iterate locations for mod-added NPCs that aren't in the data
            foreach (var loc in Game1.locations)
                foreach (var c in loc.characters)
                {
                    if (!string.IsNullOrWhiteSpace(c.Name))
                    {
                        if (!onlyVillager || c.isVillager())
                            if (!onlyDateable || c.datable.Value) characters.Add(c.Name);
                    }
                }

            //return the list
            return characters;
        }

        public void Warp(NPC npc, string l, Point p)
        {
            Warp(npc, Game1.getLocationFromName(l), p.X, p.Y);
        }

        public void Warp(NPC npc, string l, int tileX, int tileY)
        {
            Warp(npc, Game1.getLocationFromName(l), tileX, tileY);
        }

        public void Warp(NPC npc, GameLocation l, Point p)
        {
            Warp(npc, l, p.X, p.Y);
        }

        public void Warp(NPC npc, GameLocation l, int tileX, int tileY)
        {
            if (l != npc.currentLocation)
            {
                if (npc.currentLocation != null) npc.currentLocation.characters.Remove(npc);
                l.characters.Add(npc);
                npc.currentLocation = l;
            }
            npc.setTilePosition(tileX, tileY);
            //Mod.Monitor.Log("Warped " + npc.Name + " to " + l.Name + " at " + tileX + ", " + tileY);
        }

        public bool IsPathing(NPC npc)
        {
            if (npc == null) return false;
            if (npc.controller == null) return false;
            if (npc.controller.endPoint == null) return false;
            if (npc.controller.endPoint == Utility.Vector2ToPoint(npc.Position)) return false;
            return true;
        }

        public Rectangle GetActivationBoundingBox(NPC npc)
        {
            if (npc.Sprite == null) return Rectangle.Empty;
            return new Rectangle((int)npc.Position.X + 8, (int)npc.Position.Y, npc.Sprite.SpriteWidth * 4 * 3 / 4, 48);
        }

        public Rectangle GetCollisionBoundingBox(NPC npc)
        {
            if (npc.Sprite == null) return Rectangle.Empty;
            return new Rectangle((int)npc.Position.X + 8, (int)npc.Position.Y + 16, npc.Sprite.SpriteWidth * 4 * 3 / 4, 32);
        }

        public bool IsChild(NPC npc)
        {

            if (npc is StardewValley.Characters.Child) return true; //should get vanilla player-children
            var dispositions = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
            if (dispositions.ContainsKey(npc.Name))
            {
                if (dispositions[npc.Name].Split('/')[0] == "child") return true;
                else return false;
            }
            //this npc doesn't exist in dispositions? perhaps a child, or other mod-added NPC (e.g. a Moongate)
            return npc.Age == 2; //should get any remaining NPC children
        }
    }
}
