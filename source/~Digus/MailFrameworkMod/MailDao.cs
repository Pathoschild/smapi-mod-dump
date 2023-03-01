/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace MailFrameworkMod
{
    public class MailDao
    {
        private static readonly List<Letter> Letters =  new List<Letter>();
        private static readonly List<string> RemovedLetterIds = new List<string>();
        private static bool _newLetter = false;

        /// <summary>
        /// Saves a letter on the repository.
        /// </summary>
        /// <param name="letter"> The letter to be saved.</param>
        public static void SaveLetter(Letter letter)
        {
            if (letter.Id == null)
            {
                MailFrameworkModEntry.ModMonitor.Log($"You can't add a letter with a null id. This letter will be ignored.", LogLevel.Error);
                MailFrameworkModEntry.ModMonitor.Log(letter.ToString(), LogLevel.Trace);
            }
            else
            {
                if (Game1.objectInformation == null)
                    throw new NotImplementedException("Can't add a letter before the game is launched.");

                if (Letters.Exists((l) => l.Id == letter.Id))
                {
                    Letters[Letters.FindIndex((l) => l.Id == letter.Id)] = letter;
                }
                else
                {
                    Letters.Add(letter);
                }
                _newLetter = true;
            }
        }

        /// <summary>
        /// Removes the letter from the repository.
        /// Comparison done by id.
        /// </summary>
        /// <param name="letter">The letter to be removed.</param>
        public static void RemoveLetter(Letter letter)
        {
            Letters.Remove(Letters.Find((l) => l.Id == letter.Id));
            RemovedLetterIds.Add(letter.Id);
        }

        /// <summary>
        /// Looks for the letter with the given id. Return the letter or null if nothing is found.
        /// </summary>
        /// <param name="id">the id of the letter</param>
        /// <returns>the letter</returns>
        public static Letter FindLetter(string id)
        {
            return Letters.FirstOrDefault(l => l.Id == id);
        }

        /// <summary>
        /// Validates the condition to show the letters and returns a list with all that matches.
        /// </summary>
        /// <returns>The list with all letter that matched their conditions</returns>
        internal static List<Letter> GetValidatedLetters()
        {
            return Letters.FindAll((l) =>
                {
                    var condition = false;
                    try
                    {
                        condition = l.Condition(l);
                    }
                    catch (Exception e)
                    {
                        MailFrameworkModEntry.ModMonitor.Log($"Error while validating letter '{l.Id}'. This letter will be ignored.", LogLevel.Error);
                        MailFrameworkModEntry.ModMonitor.Log($"Error: {e.Message}\n{e.StackTrace}", LogLevel.Trace);
                    }

                    return condition;
                })
                .GroupBy(l => l.GroupId != null? (object)l.GroupId : new object())
                .Select(g => 
                    (g.Key is string s && s.EndsWith(".Random",true,null)) ?
                    g.Skip((int)(new Random(SDate.Now().DaysSinceStart + g.Key.GetHashCode() + (int)Game1.uniqueIDForThisGame).NextDouble() * g.Count())).First() :
                    g.First())
                .ToList(); 
        }

        /// <summary>
        /// Returns an ReadOnlyCollection with all saved letters in the repository.
        /// </summary>
        /// <returns>The list with all saved letters.</returns>
        internal static ReadOnlyCollection<Letter> GetSavedLetters()
        {
            return Letters.AsReadOnly();
        }

        /// <summary>
        /// Returns the id of all letters that were removed from the repository.
        /// </summary>
        /// <returns>The list with all removed letter ids.</returns>
        internal static ReadOnlyCollection<string> GetRemovedLetterIds()
        {
            return RemovedLetterIds.AsReadOnly();
        }

        /// <summary>
        /// Returns if repository has changed since initialization or last time it was cleared.
        /// Check if letter were added or removed.
        /// </summary>
        /// <returns>boolean value indicating if the repository has changed.</returns>
        internal static bool HasRepositoryChanged()
        {
            return RemovedLetterIds.Count > 0 || _newLetter;
        }
        /// <summary>
        /// Clear the changing indications of the repository.
        /// Will clear the list of removed letter and the flag of new letter added.
        /// </summary>
        internal static void CleanDataToUpdate()
        {
            RemovedLetterIds.Clear();
            _newLetter = false;
        }
    }
}
