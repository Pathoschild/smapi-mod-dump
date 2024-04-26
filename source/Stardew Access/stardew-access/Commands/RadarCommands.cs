/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using stardew_access.Features;

namespace stardew_access.Commands;

public class RadarCommands
{
    public static void CountFocus_rfcount(string[] args, bool fromChatBox = false)
    {
        Log.Info($"There are {Radar.Instance.Focus.Count} objects in the focus list in the radar feature.");
    }

    public static void ClearFocus_rfclear(string[] args, bool fromChatBox = false)
    {
        Radar.Instance.Focus.Clear();
        Log.Info($"Cleared the focus list in the radar feature.");
    }

    public static void ListAllFocus_rflist(string[] args, bool fromChatBox = false)
    {
        if (Radar.Instance.Focus.Count > 0)
        {
            string toPrint = "";
            for (int i = 0; i < Radar.Instance.Focus.Count; i++)
            {
                toPrint = $"{toPrint}\t{i + 1}): {Radar.Instance.Focus[i]}";
            }

            Log.Info(toPrint);
        }
        else
        {
            Log.Info("No objects found in the focus list.");
        }
    }

    public static void RemoveFromFocus_rfremove(string[] args, bool fromChatBox = false)
    {
        string? keyToAdd = null;

        for (int i = 0; i < args.Length; i++)
        {
            keyToAdd += " " + args[i];
        }

        if (keyToAdd != null)
        {
            keyToAdd = keyToAdd.Trim().ToLower();
            if (Radar.Instance.Focus.Contains(keyToAdd))
            {
                Radar.Instance.Focus.Remove(keyToAdd);
                Log.Info($"Removed {keyToAdd} key from focus list.");
            }
            else
            {
                Log.Info($"Cannot find {keyToAdd} key in focus list.");
            }
        }
        else
        {
            Log.Info("Unable to remove the key from focus list.");
        }
    }

    public static void AddToFocus_rfadd(string[] args, bool fromChatBox = false)
    {
        string? keyToAdd = null;

        for (int i = 0; i < args.Length; i++)
        {
            keyToAdd += " " + args[i];
        }

        if (keyToAdd != null)
        {
            keyToAdd = keyToAdd.Trim().ToLower();
            if (!Radar.Instance.Focus.Contains(keyToAdd))
            {
                Radar.Instance.Focus.Add(keyToAdd);
                Log.Info($"Added {keyToAdd} key to focus list.");
            }
            else
            {
                Log.Info($"{keyToAdd} key already present in the list.");
            }
        }
        else
        {
            Log.Info("Unable to add the key to focus list.");
        }
    }

    public static void CountExclusions_recount(string[] args, bool fromChatBox = false)
    {
        Log.Info($"There are {Radar.Instance.Exclusions.Count} exclusiond in the radar feature.");
    }

    public static void ClearExclusions_reclear(string[] args, bool fromChatBox = false)
    {
        Radar.Instance.Exclusions.Clear();
        Log.Info($"Cleared the focus list in the exclusions feature.");
    }

    public static void ListExclusions_relist(string[] args, bool fromChatBox = false)
    {
        if (Radar.Instance.Exclusions.Count > 0)
        {
            string toPrint = "";
            for (int i = 0; i < Radar.Instance.Exclusions.Count; i++)
            {
                toPrint = $"{toPrint}\t{i + 1}: {Radar.Instance.Exclusions[i]}";
            }

            Log.Info(toPrint);
        }
        else
        {
            Log.Info("No exclusions found.");
        }
    }

    public static void RemoveFromExclusions_reremove(string[] args, bool fromChatBox = false)
    {
        string? keyToAdd = null;

        for (int i = 0; i < args.Length; i++)
        {
            keyToAdd += " " + args[i];
        }

        if (keyToAdd != null)
        {
            keyToAdd = keyToAdd.Trim().ToLower();
            if (Radar.Instance.Exclusions.Contains(keyToAdd))
            {
                Radar.Instance.Exclusions.Remove(keyToAdd);
                Log.Info($"Removed {keyToAdd} key from exclusions list.");
            }
            else
            {
                Log.Info($"Cannot find {keyToAdd} key in exclusions list.");
            }
        }
        else
        {
            Log.Info("Unable to remove the key from exclusions list.");
        }
    }

    public static void AddToExclusions_readd(string[] args, bool fromChatBox = false)
    {
        string? keyToAdd = null;

        for (int i = 0; i < args.Length; i++)
        {
            keyToAdd += " " + args[i];
        }

        if (keyToAdd != null)
        {
            keyToAdd = keyToAdd.Trim().ToLower();
            if (!Radar.Instance.Exclusions.Contains(keyToAdd))
            {
                Radar.Instance.Exclusions.Add(keyToAdd);
                Log.Info($"Added {keyToAdd} key to exclusions list.");
            }
            else
            {
                Log.Info($"{keyToAdd} key already present in the list.");
            }
        }
        else
        {
            Log.Info("Unable to add the key to exclusions list.");
        }
    }

    public static void RRange(string[] args, bool fromChatBox = false)
    {
        string? rangeInString = null;

        if (args.Length > 0)
        {
            rangeInString = args[0];


            bool isParsable = int.TryParse(rangeInString, out int range);

            if (isParsable)
            {
                Radar.Instance.Range = range;
                if (range >= 2 && range <= 10)
                    Log.Info($"Range set to {Radar.Instance.Range}.");
                else
                    Log.Info($"Range should be atleast 2 and maximum 10.");
            }
            else
            {
                Log.Info("Invalid range amount, it can only be in numeric form.");
            }
        }
        else
        {
            Log.Info("Enter the range amount!");
        }
    }

    public static void RDelay(string[] args, bool fromChatBox = false)
    {
        string? delayInString = null;

        if (args.Length > 0)
        {
            delayInString = args[0];


            bool isParsable = int.TryParse(delayInString, out int delay);

            if (isParsable)
            {
                Radar.Instance.Delay = delay;
                if (delay >= 1000)
                    Log.Info($"Delay set to {Radar.Instance.Delay} milliseconds.");
                else
                    Log.Info($"Delay should be atleast 1 second or 1000 millisecond long.");
            }
            else
            {
                Log.Info("Invalid delay amount, it can only be in numeric form.");
            }
        }
        else
        {
            Log.Info("Enter the delay amount (in milliseconds)!");
        }
    }

    public static void RFocus(string[] args, bool fromChatBox = false)
    {
        bool focus = Radar.Instance.ToggleFocus();

        Log.Info("Focus mode is " + (focus ? "on" : "off"));
    }

    public static void RStereo(string[] args, bool fromChatBox = false)
    {
        MainClass.Config.RadarStereoSound = !MainClass.Config.RadarStereoSound;
        MainClass.ModHelper!.WriteConfig(MainClass.Config);

        Log.Info("Stereo sound is " + (MainClass.Config.RadarStereoSound ? "on" : "off"));
    }

    public static void RDebug(string[] args, bool fromChatBox = false)
    {
        Radar.RadarDebug = !Radar.RadarDebug;

        Log.Info("Radar debugging " + (Radar.RadarDebug ? "on" : "off"));
    }

    public static void RadarCommand_radar(string[] args, bool fromChatBox = false)
    {
        MainClass.Config.Radar = !MainClass.Config.Radar;
        MainClass.ModHelper!.WriteConfig(MainClass.Config);

        Log.Info("Radar " + (MainClass.Config.Radar ? "on" : "off"));
    }
}
