/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

// ReSharper disable ClassNeverInstantiated.Global
namespace DynamicDialogues.Models;

internal class AfterSequenceBehavior
{
    public string Mail { get; }
    public bool ImmediateMail { get; }
    public string Flag { get; }
    public int Energy { get; }
    public int Health { get; }

    public AfterSequenceBehavior()
    { }

    public AfterSequenceBehavior(AfterSequenceBehavior a)
    {
        Mail = a.Mail;
        ImmediateMail = a.ImmediateMail;

        Flag = a.Flag;
        
        Energy = a.Energy;
        Health = a.Health;
    }
}
