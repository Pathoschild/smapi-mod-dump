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

namespace SpecialOrdersExtended.DataModels;

/// <summary>
/// Storage structure for previously seen dialogues.
/// </summary>
internal class DialogueLog : AbstractDataModel
{
    private const string IDENTIFIER = "_dialogue";
    private readonly long multiplayerID;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogueLog"/> class.
    /// </summary>
    /// <param name="savefile">Save directory name. (Includes farm name + game random seed.).</param>
    /// <param name="multiplayerID">Unique multiplayer ID of player.</param>
    public DialogueLog(string savefile, long multiplayerID)
    : base(savefile) => this.multiplayerID = multiplayerID;

    /// <summary>
    /// Gets or sets backing field that contains all the SeenDialogues.
    /// </summary>
    public Dictionary<string, List<string>> SeenDialogues { get; set; } = new();

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
        return ModEntry.DataHelper.ReadGlobalData<DialogueLog>($"{Constants.SaveFolderName}{IDENTIFIER}{multiplayerID}")
            ?? new DialogueLog(Constants.SaveFolderName, multiplayerID);
    }

    /// <summary>
    /// Load from temporary storage.
    /// </summary>
    /// <param name="multiplayerID">Unique ID per player.</param>
    /// <returns>A dialogueLog object.</returns>
    /// <exception cref="NotImplementedException">Not implemented, do not call.</exception>
    /// <remarks>NOT IMPLEMENTED YET.</remarks>
    public static DialogueLog LoadTempIfAvailable(long multiplayerID)
    {
        throw new NotImplementedException();
    }

    public void SaveTemp() => base.SaveTemp(IDENTIFIER + this.multiplayerID.ToString());

    /// <summary>
    /// Saves the DialogueLog.
    /// </summary>
    public void Save() => base.Save(IDENTIFIER + this.multiplayerID.ToString());

    /// <summary>
    /// Whether or not the dialogueLog contains the key.
    /// </summary>
    /// <param name="dialoguekey">Exact dialogue key.</param>
    /// <param name="characterName">Which character to check.</param>
    /// <returns>True if found, false otheerwise.</returns>
    [Pure]
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
    [Pure]
    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append($"DialogueLog({this.Savefile}):");
        foreach (string key in Utilities.ContextSort(this.SeenDialogues.Keys))
        {
            stringBuilder.AppendLine().Append($"    {key}:").AppendJoin(", ", this.SeenDialogues[key]);
        }
        return stringBuilder.ToString();
    }
}
