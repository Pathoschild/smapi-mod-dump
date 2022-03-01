/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/SpecialOrdersExtended
**
*************************************************/

using System.Text;
using AtraShared;
using StardewModdingAPI.Utilities;
using AtraUtils = AtraShared.Utils.Utils;

namespace SpecialOrdersExtended.DataModels;

/// <summary>
/// Storage structure for previously seen dialogues.
/// </summary>
internal class DialogueLog : AbstractDataModel
{
    private const string IDENTIFIER = "_dialogue";
    private long multiplayerID;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogueLog"/> class.
    /// </summary>
    /// <param name="savefile">Save directory name. (Includes farm name + game random seed.).</param>
    /// <param name="multiplayerID">Unique multiplayer ID of player.</param>
    public DialogueLog(string savefile, long multiplayerID)
    : base(savefile) => this.multiplayerID = multiplayerID;

    /// <summary>
    /// Gets backing field that contains all the SeenDialogues.
    /// </summary>
    /// <remarks>Avoid using this directly; use the TryAdd/TryRemove/Contains methods instead if possible.</remarks>
    public Dictionary<string, List<string>> SeenDialogues { get; private set; } = new();

    /// <summary>
    /// Loads a dialogueLog.
    /// </summary>
    /// <param name="multiplayerID">Multiplayer ID of the player who requested it.</param>
    /// <returns>The dialogueLog in question.</returns>
    /// <exception cref="SaveNotLoadedError">Save not loaded.</exception>
    public static DialogueLog Load(long multiplayerID)
    {
        if (!Context.IsWorldReady)
        {
            throw new SaveNotLoadedError();
        }
        DialogueLog log = ModEntry.DataHelper.ReadGlobalData<DialogueLog>($"{Constants.SaveFolderName}{IDENTIFIER}{multiplayerID:X8}")
            ?? new DialogueLog(Constants.SaveFolderName, multiplayerID);
        log.multiplayerID = multiplayerID; // fix the multiplayer ID since ReadGlobalData will use the default zero-parameter constructor.
        return log;
    }

    /// <summary>
    /// Load from temporary storage if available. Load the usual dialogue log if not.
    /// </summary>
    /// <param name="multiplayerID">Unique ID per player.</param>
    /// <returns>A dialogueLog object.</returns>
    /// <exception cref="NotImplementedException">Not implemented, do not call.</exception>
    /// <remarks>NOT IMPLEMENTED YET.</remarks>
    public static DialogueLog LoadTempIfAvailable(long multiplayerID)
    {
        if (!Context.IsWorldReady)
        {
            throw new SaveNotLoadedError();
        }
        DialogueLog log = ModEntry.DataHelper.ReadGlobalData<DialogueLog>($"{Constants.SaveFolderName}{IDENTIFIER}{multiplayerID:X8}_temp_{SDate.Now().DaysSinceStart}");
        if (log is not null)
        {
            // Delete temporary file
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is a valid call, SMAPI just doesn't use nullable.
            ModEntry.DataHelper.WriteGlobalData<DialogueLog>($"{Constants.SaveFolderName}{IDENTIFIER}{multiplayerID:X8}_temp_{SDate.Now().DaysSinceStart}", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            log.multiplayerID = multiplayerID;
            return log;
        }
        log = ModEntry.DataHelper.ReadGlobalData<DialogueLog>($"{Constants.SaveFolderName}{IDENTIFIER}{multiplayerID:X8}")
            ?? new DialogueLog(Constants.SaveFolderName, multiplayerID);
        log.multiplayerID = multiplayerID;
        return log;
    }

    /// <summary>
    /// Saves a temporary DialogueLog file.
    /// </summary>
    public void SaveTemp() => base.SaveTemp(IDENTIFIER + this.multiplayerID.ToString("X8"));

    /// <summary>
    /// Saves the DialogueLog.
    /// </summary>
    public void Save() => base.Save(IDENTIFIER + this.multiplayerID.ToString("X8"));

    /// <summary>
    /// Whether or not the dialogueLog contains the key.
    /// </summary>
    /// <param name="dialoguekey">Exact dialogue key.</param>
    /// <param name="characterName">Which character to check.</param>
    /// <returns>True if found, false otheerwise.</returns>
    public bool Contains(string dialoguekey, string characterName)
    {
        if (this.SeenDialogues.TryGetValue(dialoguekey, out List<string>? characterList))
        {
            return characterList.Contains(characterName);
        }
        return false;
    }

    /// <summary>
    /// Tries to add a dialogue key to a character's SeenDialogues, if they haven't seen it before.
    /// </summary>
    /// <param name="dialoguekey">Dialogue key in question.</param>
    /// <param name="characterName">NPC name.</param>
    /// <returns>True if successfully added, false otherwise.</returns>
    public bool TryAdd(string dialoguekey, string characterName)
    {
        if (!this.SeenDialogues.TryGetValue(dialoguekey, out List<string>? characterList))
        {
            characterList = new();
            characterList.Add(characterName);
            this.SeenDialogues[dialoguekey] = characterList;
            return true;
        }
        else if (characterList.Contains(characterName))
        {
            return false;
        }
        else
        {
            characterList.Add(characterName);
            return true;
        }
    }

    /// <summary>
    /// Tries to remove a specific dialogue key for a character.
    /// </summary>
    /// <param name="dialoguekey">Dialogue key, exact.</param>
    /// <param name="characterName">Name of character.</param>
    /// <returns>True if successfully removed, false otherwise.</returns>
    public bool TryRemove(string dialoguekey, string characterName)
    {
        if (this.SeenDialogues.TryGetValue(dialoguekey, out List<string>? characterList))
        {
            return characterList.Remove(characterName);
        }
        return false;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append($"DialogueLog({this.Savefile}):");
        foreach (string key in AtraUtils.ContextSort(this.SeenDialogues.Keys))
        {
            stringBuilder.AppendLine().Append($"    {key}:").AppendJoin(", ", this.SeenDialogues[key]);
        }
        return stringBuilder.ToString();
    }
}
