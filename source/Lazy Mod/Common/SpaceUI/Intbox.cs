/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

namespace Common.SpaceUI;

public class Intbox : Textbox
{
    /*********
     ** Accessors
     *********/
    public int Value
    {
        get => int.TryParse(String, out int value) ? value : 0;
        set => String = value.ToString();
    }

    public bool IsValid => int.TryParse(String, out _);


    /*********
     ** Protected methods
     *********/
    /// <inheritdoc />
    protected override void ReceiveInput(string str)
    {
        bool valid = true;
        for (int i = 0; i < str.Length; ++i)
        {
            char c = str[i];
            if (!char.IsDigit(c) && !(c == '-' && String == "" && i == 0))
            {
                valid = false;
                break;
            }
        }

        if (!valid)
            return;

        String += str;
        Callback?.Invoke(this);
    }
}