/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

namespace AtraShared.Menuing;
#warning - fix controller support.

/// <summary>
/// Shamelessly stolen from RSV: https://github.com/Rafseazz/Ridgeside-Village-Mod/blob/main/Ridgeside%20SMAPI%20Component%202.0/RidgesideVillage/DialogueMenu.cs.
/// </summary>
public class DialogueAndAction : DialogueBox
{
    private readonly List<Action?> actions;

    private readonly IInputHelper inputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogueAndAction"/> class.
    /// </summary>
    /// <param name="dialogue">Initial dialogue.</param>
    /// <param name="responses">List of responses.</param>
    /// <param name="actions">List of associated actions.</param>
    /// <param name="inputHelper">SMAPI's input helper.</param>
    public DialogueAndAction(string dialogue, List<Response> responses, List<Action?> actions, IInputHelper inputHelper)
        : base(dialogue, responses)
    {
        this.actions = actions;
        this.inputHelper = inputHelper;
    }

    /// <summary>
    /// Handles a key press.
    /// </summary>
    /// <param name="key">Key.</param>
    [UsedImplicitly]
    public override void receiveKeyPress(Keys key)
    {
        base.receiveKeyPress(key);
        if (this.safetyTimer > 0)
        {
            return;
        }
        for (int i = 0; i < this.responses.Count; i++)
        {
            if (this.responses[i].hotkey == key)
            {
                if (i < this.actions.Count)
                {
                    this.inputHelper.Suppress(key.ToSButton());
                    this.actions[i]?.Invoke();
                    this.closeDialogue();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Handles a left click.
    /// </summary>
    /// <param name="x">x location clicked.</param>
    /// <param name="y">y location clicked.</param>
    /// <param name="playSound">whether or not to play sounds.</param>
    [UsedImplicitly]
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (this.safetyTimer <= 0 && this.selectedResponse >= 0 && this.selectedResponse < this.actions.Count)
        {
            this.actions[this.selectedResponse]?.Invoke();
        }
        if (Game1.activeClickableMenu is not null)
        {
            base.receiveLeftClick(x, y, playSound);
        }
    }
}