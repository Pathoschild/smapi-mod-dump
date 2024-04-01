/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StephHoel/ModsStardewValley
**
*************************************************/

namespace Utils.Config;

public class MachineConfig
{
    public string Name { get; set; }

    public int Time { get; set; } = 100;

    public bool UsePercent { get; set; } = true;

    public MachineConfig(string Name)
    {
        this.Name = Name;
    }
}