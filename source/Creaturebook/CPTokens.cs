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
using StardewModdingAPI;
using StardewValley;

namespace Creaturebook
{
    internal class DiscoveredCreaturesFromAChapter
    {
        public string previousPrefix;
        public string b;
        public int c;
        public int d;

        public bool AllowsInput()
        {
            return true;
        }
        public bool CanHaveMultipleValues(string input = null)
        {
            return false;
        }

        public bool IsReady()
        {
            return Context.IsWorldReady;
        }

        public bool UpdateContext()
        {
            if (IsReady())
            {
                foreach (var data in Game1.player.modData.Pairs)
                {
                    foreach (var item in ModEntry.Chapters)
                    {
                        if (item.FromContentPack.Manifest.UniqueID + "_" + item.CreatureNamePrefix == previousPrefix && data.Value != "null")
                        {
                            if (data.Key.Contains(previousPrefix))
                            {
                                d++;
                            }
                        }
                    }
                }
                return c != d;
            }
            else           
                return false;
        }

        /// <summary>Get the current values.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
        public IEnumerable<string> GetValues(string input)
        {
            b = input;
            if (IsReady() && input != "" && input != null)
            {
                foreach (var data in Game1.player.modData.Pairs)
                {
                    foreach (var chapter in ModEntry.Chapters)
                    {
                        if (chapter.CreatureNamePrefix == input && data.Value != "null")
                        {
                            if (data.Key.Contains(previousPrefix))
                            {
                                c++;
                            }
                        }
                    }
                }
                yield return Convert.ToString(c);
            }
            else
            {
                yield return null;
            }
        }
    }
    internal class IsCreatureDiscovered 
    {
        public string previousKey; 
        public bool a_Boolean;
        public bool b_Boolean;

        public bool AllowsInput()
        {
            return true;
        }
        public bool CanHaveMultipleValues(string input = null)
        {
            return false;
        }

        public bool IsReady()
        {
            return Context.IsWorldReady;
        }

        public bool UpdateContext()
        {
            if (IsReady())
            {
                foreach (var item in Game1.player.modData.Pairs)
                {
                    if (item.Key.StartsWith(ModEntry.MyModID) && item.Key != ModEntry.MyModID + "_IsNotebookObtained")
                    {
                        foreach (var chapter in ModEntry.Chapters)
                        {
                            for (int i = 0; i < chapter.Creatures.Length; i++)
                            {
                                if (chapter.FromContentPack.Manifest.UniqueID + "_" + chapter.CreatureNamePrefix + "_" + chapter.Creatures[i].ID == previousKey && item.Value != "null")
                                {
                                    b_Boolean = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                return a_Boolean != b_Boolean;
            }
            else
                return false;
        }

        /// <summary>Get the current values.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
        public IEnumerable<string> GetValues(string input)
        {
            if (IsReady())
            {
                foreach (var item in Game1.player.modData.Pairs)
                {
                    if (item.Key.StartsWith(ModEntry.MyModID) && item.Key != ModEntry.MyModID + "_IsNotebookObtained")
                    {
                        foreach (var chapter in ModEntry.Chapters)
                        {
                            for (int i = 0; i < chapter.Creatures.Length; i++)
                            {
                                if (chapter.FromContentPack.Manifest.UniqueID + "_" + chapter.CreatureNamePrefix + "_" + chapter.Creatures[i].ID == input && item.Value != null)
                                {
                                    a_Boolean = true;
                                    previousKey = input;
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
                yield return Convert.ToString(a_Boolean);
            }
            else
            {
                yield return null;
            }
        }
    }
    internal class IsSetFullyDiscovered
    {
        string previousValue;
        List<bool> checks = new();
        int number = 0;

        public bool AllowsInput()
        {
            return true;
        }
        public bool CanHaveMultipleValues(string input)
        {
            return false;
        }

        public bool IsReady()
        {
            return Context.IsWorldReady;
        }

        public bool UpdateContext()
        {
            checks = new List<bool>();
            foreach (var item in Game1.player.modData.Pairs)
            {
                if (item.Key.StartsWith(ModEntry.MyModID) && item.Key != ModEntry.MyModID + "_IsNotebookObtained")
                {
                    for (int i = 0; i < ModEntry.Chapters.Count; i++)
                    {
                        for (int x = 0; x < ModEntry.Chapters[i].Sets.Length; x++)
                        {
                            for (int f = 0; f < ModEntry.Chapters[i].Sets[x].CreaturesBelongingToThisSet.Length; f++)
                            {
                                if (previousValue == ModEntry.Chapters[i].Sets[x].InternalName && item.Key.Contains(ModEntry.Chapters[i].FromContentPack.Manifest.UniqueID + "." + ModEntry.Chapters[i].CreatureNamePrefix + "_" + ModEntry.Chapters[i].Sets[x].CreaturesBelongingToThisSet[f].ToString()) && item.Value != "null")
                                    checks.Add(true);
                                
                                else if (previousValue == ModEntry.Chapters[i].Sets[x].InternalName && (item.Key.Contains(ModEntry.Chapters[i].FromContentPack.Manifest.UniqueID + "." + ModEntry.Chapters[i].CreatureNamePrefix + "_" + ModEntry.Chapters[i].Sets[x].CreaturesBelongingToThisSet[f].ToString()) || item.Value == "null"))
                                    checks.Add(false);
                                
                                number = ModEntry.Chapters[i].Sets[x].CreaturesBelongingToThisSet.Length;
                            }
                        }
                    }
                }
            }
            int a = 0;
            for (int i = 0; i < checks.Count; i++)
            {
                if (checks[i])
                {
                    a++;
                }
            }
            return a != number;
        }

        /// <summary>Get the current values.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
        public IEnumerable<string> GetValues(string input)
        {
            checks = new List<bool>();
            foreach (var item in Game1.player.modData.Pairs)
            {
                if (item.Key.StartsWith(ModEntry.MyModID) && item.Key != ModEntry.MyModID + "_IsNotebookObtained")
                {
                    for (int i = 0; i < ModEntry.Chapters.Count; i++)
                    {
                        for (int x = 0; x < ModEntry.Chapters[i].Sets.Length; x++)
                        {
                            for (int f = 0; f < ModEntry.Chapters[i].Sets[x].CreaturesBelongingToThisSet.Length; f++)
                            {
                                if (input == ModEntry.Chapters[i].Sets[x].InternalName && item.Key.Contains(ModEntry.Chapters[i].FromContentPack.Manifest.UniqueID + "." + ModEntry.Chapters[i].CreatureNamePrefix + "_" + ModEntry.Chapters[i].Sets[x].CreaturesBelongingToThisSet[f].ToString()) && item.Value != "null")
                                    checks.Add(true);
                                else if (input == ModEntry.Chapters[i].Sets[x].InternalName && (item.Key.Contains(ModEntry.Chapters[i].FromContentPack.Manifest.UniqueID + "." + ModEntry.Chapters[i].CreatureNamePrefix + "_" + ModEntry.Chapters[i].Sets[x].CreaturesBelongingToThisSet[f].ToString()) || item.Value == "null"))
                                    checks.Add(false);
                                number = ModEntry.Chapters[i].Sets[x].CreaturesBelongingToThisSet.Length;
                            }
                        }
                    }
                }
            }
            int a = 0;
            for (int i = 0; i < checks.Count; i++)
            {
                if (checks[i])
                {
                    a++;
                }
            }
            previousValue = (a == number).ToString();
            yield return (a == number).ToString();
                 
        }
    }
}
