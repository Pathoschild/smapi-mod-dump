/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KediDili/Creaturebook
**
*************************************************/

using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Creaturebook.Framework.Models
{
    public class Attractor
    {
        public string Name;

        public IDictionary<string, string> ItemsNCreatures;
        //Key should be the creature's ID AND prefix and the other pack's ID if the creature isn't from your mod (like Flutter_32, but KediDili.ProjectStarlight.CB/Flutter_32 if its not yours). Value is names of items, seperated by commas

        public IDictionary<string, string> Probability;
        //Key is creature ID, mod ID AND prefix (like KediDili.ProjectStarlight.CB/lutter_32). double is the probability and can't be greater than 1.
        //Omitted if just want all of it to be 1.

        public Creature attracted;

        public object ReturnProb(string itemName, int x)
        {
            foreach (var item in Probability)
            {
                string[] strings = item.Value.Split('/');
                for (int i = 0; i < strings.Length; i++)
                {
                    string[] strings1 = strings[i].Split('/');
                    if (strings1[0].Contains(itemName))
                        return strings1[x];
                }
            }
            return 0;
        }
        public Creature GetCreature(string ID)
        {
            string[] splitted1 = ID.Split('/');
            for (int i = 0; i < ModEntry.Chapters.Count; i++)
            {
                string[] splitted2 = splitted1[1].Split('_');
                if (ModEntry.Chapters[i].FromContentPack.Manifest.UniqueID == splitted1[0] && ModEntry.Chapters[i].CreatureNamePrefix == splitted2[0])
                    for (int x = 0; x < ModEntry.Chapters[i].Creatures.Length; x++)
                        if (splitted2[1] == ModEntry.Chapters[i].Creatures[x].ID.ToString())
                            return ModEntry.Chapters[i].Creatures[x];                
            }
            return null;
        }
        public bool IsEligible(Vector2 @object, string @case, int i)
        {
            if (Game1.currentLocation.Objects[@object].Name == ModEntry.Attractors[i].Name)
            {
                switch (@case)
                {
                    case "Ready":
                        return Game1.currentLocation.Objects[@object].IsOn && Game1.currentLocation.Objects[@object].heldObject is null;
                    case "Empty":
                        return !Game1.currentLocation.Objects[@object].IsOn && Game1.currentLocation.Objects[@object].heldObject is null;
                    case "Worky":
                        return Game1.currentLocation.Objects[@object].IsOn && Game1.currentLocation.Objects[@object].heldObject is not null;
                }
            }
            return false;
        }
    }
}
